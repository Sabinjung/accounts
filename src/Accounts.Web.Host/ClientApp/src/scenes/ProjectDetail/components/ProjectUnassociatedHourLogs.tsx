import React, { useEffect, useState } from 'react';
import './index.less';
import { Card, Row, Col, Select } from 'antd';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import useAxios from '../../../lib/axios/useAxios';
import styled from '@emotion/styled';
import moment from 'moment';
import CustomTable from '../../../components/Custom/CustomTable';

const { Option } = Select;
const StyledSelect = styled(Select)`
  width: 200px;
`;

const CustomTooltip = ({ payload, active, label }: any) => {
  if (active) {
    if (payload !== undefined && payload !== null) {
      const year = payload[0].payload.year;
      return (
        <div className="custom-tooltip" style={{ background: 'rgba(255, 255, 255, 0.7)', padding: '10px', borderRadius: '10px' }}>
          <p className="label">{`Month : ${moment()
            .month(label - 1)
            .format('MMMM')}`}</p>
          <p className="label">{`Year : ${moment().year(year).format('YYYY')}`}</p>
          <p className="intro">{`Hours : ${payload[0].payload.totalHours}`}</p>
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
          dataKey="monthName"
          tickFormatter={(label) =>
            `${moment()
              .month(label - 1)
              .format('MMM')}`
          }
        />
        <YAxis yAxisId="left" orientation="left" stroke="#748AA1" label={{ value: 'Hours', angle: -90, position: 'insideLeft' }} />
        <Tooltip content={<CustomTooltip />} />
        <Bar yAxisId="left" dataKey="totalHours" fill="#1C3FAA" barSize={70} />
      </BarChart>
    </ResponsiveContainer>
  );
};

const ProjectUnassociatedHourLogs = (props: any) => {
  const url = props.match.url;
  const urlBreak = url.split('/');
  const projectId = urlBreak[2];
  const [selectedYear, setSelectedYear] = useState<any>('All Years');
  const [unassociatedData, setUnassociatedDate] = useState<any>();

  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Project/GetUnAssociatedHourLogReport',
    params: { projectId },
  });

  useEffect(() => {
    makeRequest({ params: { projectId } });
  }, [projectId]);

  useEffect(() => {
    setUnassociatedDate(dataSource);
  }, [data]);

  const dataSource = data && data.result.unassociatedProjectHourReportDtos;
  let yearList: any[] = [];
  if (dataSource) {
    const yearArray = dataSource.map((item: any, index: any) => (item = dataSource[index].year));
    yearList = yearArray.reduce((unique: any, item: any) => (unique.includes(item) ? unique : [...unique, item]), []);
    yearList = ['All Years', ...yearList];
  }
  const handleYearChange = (value: any) => {
    if (data) {
      if (!value || value === 'All Years') {
        setSelectedYear('All Years');
        setUnassociatedDate(dataSource);
      } else {
        const yearBasedData = dataSource.filter((obj: any) => obj.year === value);
        setUnassociatedDate(yearBasedData);
        setSelectedYear(value);
      }
    }
  };
  const columns = [
    {
      title: 'S.N',
      key: 'index',
      dataIndex: 'index',
      width: 130,
      render: (text: any, record: any, index: any) => `${index + 1}`,
    },
    {
      title: 'Month ',
      key: 'month',
      dataIndex: 'monthName',
      render: (label: any) =>
      `${moment()
        .month(label - 1)
        .format('MMMM')}`,
    },
    {
      title: 'Year',
      key: 'year',
      dataIndex: 'year',
    },
    {
      title: 'Total Hours',
      key: 'totalHours',
      dataIndex: 'totalHours',
    },
  ];

  return (
    <Card>
      <Row>
        <Col offset={21}>
          <StyledSelect showSearch allowClear value={selectedYear} placeholder="Select a year" onChange={handleYearChange}>
            {yearList.map((item: any, index: any) => (
              <Option value={item} key={index}>
                {item}
              </Option>
            ))}
          </StyledSelect>
        </Col>
      </Row>
      <RenderBarChart data={unassociatedData} />
      <CustomTable dataSource={unassociatedData} columns={columns} loading={loading} pagination={{ pageSize: 10, total: 0, defaultCurrent: 1 }} />
    </Card>
  );
};

export default ProjectUnassociatedHourLogs;
