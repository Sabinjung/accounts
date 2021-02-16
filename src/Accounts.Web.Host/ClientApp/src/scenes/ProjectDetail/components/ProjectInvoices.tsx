import React, { useState, useEffect } from 'react';
import { Row, Col, DatePicker, Card } from 'antd';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import useAxios from '../../../lib/axios/useAxios';
import { useHistory } from 'react-router';
import { Portal } from 'react-portal';
import { Get } from '../../../lib/axios';
import RouteableDrawer from '../../../components/RouteableDrawer';
import EntityPicker from '../../../components/EntityPicker';
import InvoiceDetail from '../../../components/Domain/InvoiceDetail';
import moment from 'moment';
import CustomTable from '../../../components/Custom/CustomTable';

const CustomTooltip = ({ payload, active, label }: any) => {
  if (active) {
    if (payload !== undefined && payload !== null) {
      const amount = payload[0].payload.monthAmount;
      return (
        <div className="custom-tooltip" style={{ background: 'rgba(255, 255, 255, 0.7)', padding: '10px', borderRadius: '10px' }}>
          <p className="label">{`Month : ${moment()
            .month(label - 1)
            .format('MMMM')}`}</p>
          <p className="intro">{`Amount : $ ${amount.toLocaleString()}`}</p>
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
        <YAxis yAxisId="left" orientation="left" stroke="#748AA1" label={{ value: 'Amount', angle: -90, position: 'insideLeft' }} />
        <Tooltip content={<CustomTooltip />} />
        <Bar yAxisId="left" dataKey="monthAmount" fill="#1C3FAA" barSize={70} />
      </BarChart>
    </ResponsiveContainer>
  );
};

const ProjectInvoices = (props: any) => {
  const history = useHistory();
  const url = props.match.url;
  const urlBreak = url.split('/');
  const projectId = urlBreak[2];
  const [searchId, setSearchId] = useState(undefined);
  const [dateSearchText, setDateSearchText] = useState('');
  const { RangePicker } = DatePicker;
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Invoice/Search',
    params: { Id: projectId, isActive: true, consultantId: searchId },
  });

  useEffect(() => {
    makeRequest({ params: { projectId, consultantId: searchId, startDate: dateSearchText[0], endDate: dateSearchText[1] } });
  }, [projectId, searchId, dateSearchText]);

  const result = data && data.result;

  const columns = [
    {
      title: 'QB Invoice ID',
      key: 'qboInvoiceId',
      dataIndex: 'qboInvoiceId',
      width: 130,
      align: 'center' as const,
    },
    {
      title: 'Company',
      key: 'companyName',
      dataIndex: 'companyName',
    },
    {
      title: 'Consultant',
      key: 'consultantName',
      dataIndex: 'consultantName',
    },
    {
      title: 'Issue Date',
      key: 'invoiceDate',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'invoiceDate',
    },
    {
      title: 'Amount',
      key: 'total',
      render: (val: number) => '$ ' + val.toLocaleString('en-US'),
      dataIndex: 'total',
    },
  ];

  const handleSearch = (value: any) => {
    setSearchId(value);
  };

  const handleDateSearch = (date: any, datestring: any) => {
    setDateSearchText(datestring);
  };

  const headerRender = () => {
    return (
      <Row type="flex" justify="start" align="middle" style={{ height: 45 }}>
        <Col lg={{ span: 4 }}>
          <EntityPicker
            url="api/services/app/Consultant/Search"
            mapFun={(r) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
            style={{ width: '220px' }}
            value={searchId}
            placeholder="Search Consultant"
            onChange={handleSearch}
          />
        </Col>
        <Col lg={{ span: 4 }}>
          <RangePicker onChange={handleDateSearch} style={{ width: 220 }} />
        </Col>
      </Row>
    );
  };

  const refetch = () => {
    makeRequest({ params: { isActive: true } });
  };

  const viewerRef = React.createRef<any>();

  return (
    <Card>
      <Get
        url="/api/services/app/Invoice/GetInvoicesByMonthReport"
        params={{ ProjectId: projectId, consultantId: searchId, startDate: dateSearchText[0], endDate: dateSearchText[1] }}
      >
        {({ error, data, isLoading }: any) => {
          return <RenderBarChart data={data && data.result} />;
        }}
      </Get>

      <CustomTable
        dataSource={result !== undefined ? result.listItemDto.results : []}
        columns={columns}
        loading={loading}
        pagination={{ pageSize: 10, total: 0, defaultCurrent: 1 }}
        title={() => headerRender()}
        onRow={(record: any, rowIndex: any) => ({
          onDoubleClick: () => history.push(`/invoices/${record.id}`),
        })}
      ></CustomTable>

      <Portal>
        <RouteableDrawer path={[`/invoices/:invoiceId`]} width={'50vw'} title="Invoice Detail" exact="true">
          {({
            match: {
              params: { invoiceId },
            },
            onClose,
          }: any) => {
            if (invoiceId) {
              return (
                <InvoiceDetail
                  invoiceId={invoiceId}
                  onClose={onClose}
                  onInvoiceSubmitted={() => {
                    refetch();
                    viewerRef.current && viewerRef.current.refetch();
                  }}
                />
              );
            } else {
              return null;
            }
          }}
        </RouteableDrawer>
      </Portal>
    </Card>
  );
};

export default ProjectInvoices;
