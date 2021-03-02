import React, { useState, useEffect } from 'react';
import './index.less';
import { Card, Row, Col, DatePicker, Select } from 'antd';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import useAxios from '../../../lib/axios/useAxios';
import { Get } from '../../../lib/axios';
import moment from 'moment';
import CustomTable from '../../../components/Custom/CustomTable';

const CustomTooltip = ({ payload, active, label }: any) => {
  if (active) {
    if (payload !== undefined && payload !== null) {
      return (
        <div className="custom-tooltip" style={{ background: 'rgba(255, 255, 255, 0.7)', padding: '10px', borderRadius: '10px' }}>
          <p className="label">{`Month : ${moment()
            .month(label - 1)
            .format('MMMM')}`}</p>
          <p className="intro">{`Hours : ${payload[0].payload.value}`}</p>
        </div>
      );
    } else {
      return null;
    }
  }
  return null;
};

const RenderBarChart = (props: any) => {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <BarChart width={1200} height={250} data={props.data} margin={{ top: 20, bottom: 20 }}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis
          dataKey="month"
          tickFormatter={(label) =>
            `${moment()
              .month(label - 1)
              .format('MMM')}`
          }
        />
        <YAxis yAxisId="left" orientation="left" stroke="#748AA1" label={{ value: 'Hours', angle: -90, position: 'insideLeft' }} />
        <Tooltip content={<CustomTooltip />} />
        <Bar yAxisId="left" dataKey="value" fill="#1C3FAA" barSize={70} />
      </BarChart>
    </ResponsiveContainer>
  );
};

const ProjectTimesheets = (props: any) => {
  const { RangePicker } = DatePicker;
  const { Option } = Select;
  const [dateSearchText, setDateSearchText] = useState('');
  const [statusId, setStatusId] = useState([]);
  const url = props.match.url;
  const urlBreak = url.split('/');
  const projectId = urlBreak[2];
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Timesheet/GetTimesheets',
    params: { projectId, statusId },
  });

  useEffect(() => {
    makeRequest({ params: { projectId, startTime: dateSearchText[0], endTime: dateSearchText[1], statusId } });
  }, [projectId, dateSearchText, statusId]);

  const result = data && data.result.results;
  let dataSource = result;

  const columns = [
    {
      title: 'ID',
      key: 'id',
      dataIndex: 'id',
      width: 90,
      align: 'center' as const,
    },
    {
      title: 'Start Date',
      key: 'startDt',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'startDt',
    },
    {
      title: 'End Date',
      key: 'endDt',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'endDt',
    },
    {
      title: 'Status',
      key: 'statusId',
      render: (val: number) => (val === 1 ? 'Pending Approval' : val === 2 ? 'Approved' : 'Invoiced'),
      dataIndex: 'statusId',
    },
    {
      title: 'Hours',
      key: 'totalHrs',
      dataIndex: 'totalHrs',
    },
  ];

  const handleDateSearch = (date: any, datestring: any) => {
    setDateSearchText(datestring);
  };

  const handleStateId = (value: any) => {
    setStatusId(value);
  };

  const headerRender = () => {
    return (
      <Row>
        <Col lg={{ span: 4 }}>
          <RangePicker onChange={handleDateSearch} style={{ width: 220 }} />
        </Col>
        <Col lg={{ span: 6 }}>
          <Select mode="multiple" style={{ width: '100%' }} onChange={handleStateId} placeholder="Select Status">
            <Option value="1">Pending Approval</Option>
            <Option value="2">Approved</Option>
            <Option value="4">Invoiced</Option>
          </Select>
        </Col>
      </Row>
    );
  };

  return (
    <Card>
      <Get
        url="/api/services/app/Timesheet/GetMonthlyHourReport"
        params={{ projectId, startTime: dateSearchText[0], endTime: dateSearchText[1], statusId }}
      >
        {({ error, data, isLoading }: any) => {
          return <RenderBarChart data={data && data.result} />;
        }}
      </Get>

      <CustomTable
        dataSource={dataSource}
        columns={columns}
        loading={loading}
        pagination={{ pageSize: 10, total: 0, defaultCurrent: 1 }}
        title={() => headerRender()}
      />
    </Card>
  );
};

export default ProjectTimesheets;
