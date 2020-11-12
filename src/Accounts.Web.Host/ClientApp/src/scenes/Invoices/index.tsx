import React, { useState, useEffect } from 'react';
import { Card, Row, Col, Table, DatePicker, Button } from 'antd';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import useAxios from '../../lib/axios/useAxios';
import moment from 'moment';
import { useHistory } from 'react-router';
import { Portal } from 'react-portal';
import RouteableDrawer from '../../components/RouteableDrawer';
import EntityPicker from '../../components/EntityPicker';
import InvoiceDetail from '../../components/Domain/InvoiceDetail';
import { Get } from '../../lib/axios';
import styled from '@emotion/styled';

const StyledTable = styled(Table)`
  .ant-table-tbody > tr.ant-table-row:hover > td {
    background: unset;
  }
  .overdue {
    background: #f1dbdb !important;
  }
`;

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
      <BarChart width={900} height={250} data={props.data} margin={{ top: 20, bottom: 20 }}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis
          dataKey="monthName"
          tickFormatter={(label) =>
            `${moment()
              .month(label - 1)
              .format('MMM')}`
          }
        />
        <YAxis yAxisId="left" orientation="left" stroke="#8884d8" />
        <Tooltip content={<CustomTooltip />} />
        <Bar yAxisId="left" dataKey="monthAmount" fill="#82ca9d" barSize={70} />
      </BarChart>
    </ResponsiveContainer>
  );
};

const AllInvoiceList = (props: any) => {
  const [companySearchText, setCompanySearchText] = useState(undefined);
  const [consultantSearchText, setConsultantSearchText] = useState(undefined);
  const [dateSearchText, setDateSearchText] = useState([]);
  const { RangePicker } = DatePicker;
  const history = useHistory();
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Invoice/Search',
    params: {
      isActive: true,
      companyId: companySearchText,
      consultantId: consultantSearchText,
      startDate: dateSearchText[0] && moment(dateSearchText[0]).format('YYYY-MM-DD'),
      endDate: dateSearchText[1] && moment(dateSearchText[1]).format('YYYY-MM-DD'),
    },
  });
  const result = (data && data.result) || { results: [], recordCounts: [], totalCount: 0 };

  useEffect(() => {
    const interval = setInterval(() => {
      setCompanySearchText(undefined);
      setConsultantSearchText(undefined);
      setDateSearchText([]);
      makeRequest({});
    }, 300000);
    return () => clearInterval(interval);
  }, []);

  const columns = [
    {
      title: 'Invoice ID',
      key: 'id',
      render: (val: number) => (
        <Button type="link" onClick={() => history.push(`/invoices/${val}`)}>
          {val}
        </Button>
      ),
      dataIndex: 'id',
      width: 90,
      align: 'center' as const,
    },
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
      title: 'End Client',
      key: 'endClientName',
      dataIndex: 'endClientName',
    },
    {
      title: 'Issue Date',
      key: 'invoiceDate',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'invoiceDate',
    },
    {
      title: 'Due Date',
      key: 'dueDate',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'dueDate',
    },
    {
      title: 'Amount',
      key: 'total',
      render: (val: number) => '$ ' + val.toLocaleString('en-US'),
      dataIndex: 'total',
    },
    {
      title: 'Balance',
      key: 'balance',
      dataIndex: 'balance',
    },
  ];

  const handleCompanySearch = (value: any) => {
    setCompanySearchText(value);
  };

  const handleConsultantSearch = (value: any) => {
    setConsultantSearchText(value);
  };

  const handleDateSearch = (date: any) => {
    setDateSearchText(date);
  };

  const headerRender = () => {
    return (
      <Row type="flex" justify="start" align="middle" style={{ height: 45 }}>
        <Col lg={{ span: 4 }}>
          <EntityPicker
            url="api/services/app/Company/Search"
            mapFun={(r) => ({ value: r.id, text: `${r.displayName}` })}
            style={{ width: '220px' }}
            value={companySearchText}
            placeholder="Search Company"
            onChange={handleCompanySearch}
          />
        </Col>
        <Col lg={{ span: 4 }}>
          <EntityPicker
            url="api/services/app/Consultant/Search"
            mapFun={(r) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
            style={{ width: '220px' }}
            value={consultantSearchText}
            placeholder="Search Consultant"
            onChange={handleConsultantSearch}
          />
        </Col>
        <Col lg={{ span: 4 }}>
          <RangePicker onChange={handleDateSearch} value={dateSearchText} />
        </Col>
      </Row>
    );
  };

  const refetch = () => {
    makeRequest({ params: { isActive: true } });
  };

  const viewerRef = React.createRef<any>();

  return (
    <>
      <Card>
        <h2>Invoices</h2>
        <Get
          url="/api/services/app/Invoice/GetInvoicesByMonthReport"
          params={{
            isActive: true,
            companyId: companySearchText,
            consultantId: consultantSearchText,
            startDate: dateSearchText[0] && moment(dateSearchText[0]).format('YYYY-MM-DD'),
            endDate: dateSearchText[1] && moment(dateSearchText[1]).format('YYYY-MM-DD'),
          }}
        >
          {({ error, data, isLoading }: any) => {
            return <RenderBarChart data={data && data.result} />;
          }}
        </Get>

        <StyledTable
          dataSource={result !== undefined ? result.results : []}
          rowClassName={(record: any) => {
            let isSame: boolean = moment(moment().format('YYYY-MM-DD')).isSame(record.dueDate);
            let isBefore: boolean = moment().isBefore(record.dueDate);
            return isSame || record.balance === null || record.balance === 0 ? '' : isBefore ? '' : 'overdue';
          }}
          columns={columns}
          bordered
          pagination={{ pageSize: 10, total: 0, defaultCurrent: 1 }}
          loading={loading}
          size="small"
          tableLayout="auto"
          title={() => headerRender()}
          onRow={(record: any, rowIndex: any) => ({
            onDoubleClick: () => history.push(`/invoices/${record.id}`),
          })}
        ></StyledTable>
      </Card>

      <Portal>
        <RouteableDrawer path={[`/invoices/:invoiceId`]} width={'50vw'} title="Invoice Detail">
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
    </>
  );
};

export default AllInvoiceList;
