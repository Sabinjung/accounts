import React, { useState, useEffect } from 'react';
import { Card, Row, Col, DatePicker, Button, Popover, Typography, Icon } from 'antd';
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
import CustomTable from '../../components/Custom/CustomTable';
import PredefinedQueryPills from '../../components/PredefinedQueryPills';

const { Text } = Typography;
type sortableType = ['descend', 'ascend', 'descend'];
const defaultSortOrder: sortableType = ['descend', 'ascend', 'descend'];

const StyledTable = styled(CustomTable)`
  .ant-table-title {
    border-bottom: none !important;
  }
  .ant-table-small {
    border: none;
  }
  .ant-table-column-has-actions {
    .ant-table-header-column {
      margin-top: 0px;
    }
  }
  .ant-table-header-column {
    margin-top: 22px;
  }
  .ant-table-tbody {
    .overdue {
      background: #f1dbdb !important;
    }
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
        <YAxis yAxisId="left" orientation="left" stroke="#748AA1" />
        <Tooltip content={<CustomTooltip />} />
        <Bar yAxisId="left" dataKey="monthAmount" fill="#1C3FAA" barSize={70} />
      </BarChart>
    </ResponsiveContainer>
  );
};

const AllInvoiceList = (props: any) => {
  const [companySearchText, setCompanySearchText] = useState(undefined);
  const [consultantSearchText, setConsultantSearchText] = useState(undefined);
  const [queryName, setQueryName] = useState('All');
  const [dateSearchText, setDateSearchText] = useState([]);
  const [order, setOrder] = useState('descend');
  const [columnKey, setColumnKey] = useState('overdueBy');
  const [currentPage, setCurrentPage] = useState(1);
  const { RangePicker } = DatePicker;
  const history = useHistory();
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Invoice/Search',
    params: {
      isActive: true,
      name: queryName,
      companyId: companySearchText,
      consultantId: consultantSearchText,
      startDate: dateSearchText[0] && moment(dateSearchText[0]).format('YYYY-MM-DD'),
      endDate: dateSearchText[1] && moment(dateSearchText[1]).format('YYYY-MM-DD'),
    },
  });
  const result = (data && data.result) || { results: [], recordCounts: [], totalCount: 0 };
  let predefinedQueries = (result.listItemDto && result.listItemDto.recordCounts) || [];
  predefinedQueries &&
    predefinedQueries.map((item: any) => {
      if (item.name === 'All') {
        item.count = null;
      }
    });

  useEffect(() => {
    const interval = setInterval(() => {
      setCompanySearchText(undefined);
      setConsultantSearchText(undefined);
      setDateSearchText([]);
      makeRequest({});
    }, 300000);
    return () => clearInterval(interval);
  }, []);

  const isOverdue = (record: any) => {
    let currentDate = moment().format('YYYY-MM-DD');
    let dueDate = moment(record.dueDate).format('YYYY-MM-DD');
    let isSame: boolean = moment(currentDate).isSame(dueDate);
    let isBefore: boolean = moment(currentDate).isBefore(dueDate);
    return isSame || record.balance === null || record.balance === 0 ? false : isBefore ? false : true;
  };

  const handleCompanySearch = (value: any) => {
    setCompanySearchText(value);
  };

  const handleConsultantSearch = (value: any) => {
    setConsultantSearchText(value);
  };

  const handleDateSearch = (date: any) => {
    setDateSearchText(date);
  };

  const CheckNull = (a: string, b: string, sortOrder: string) => {
    let initialConsultant: string = a ? a : sortOrder === 'ascend' ? 'z' : 'a';
    let nextConsultant: string = b ? b : sortOrder === 'ascend' ? 'z' : 'a';
    return [initialConsultant, nextConsultant];
  };

  const content = (item: any) => (
    <>
      {item.companyPhoneNumber && (
        <div>
          <Icon type="phone" theme="filled" style={{ color: '#1DA57A', marginRight: '5px' }} />
          <Text copyable>{item.companyPhoneNumber}</Text>
        </div>
      )}
      {item.companyEmail &&
        item.companyEmail.map((val: any) => (
          <div>
            <Icon type="mail" theme="filled" style={{ color: '#1DA57A', marginRight: '5px' }} />
            <Text copyable>{val}</Text>
          </div>
        ))}
    </>
  );

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
      width: 120,
      align: 'center' as const,
      sorter: (a: any, b: any) => a.id - b.id,
      sortDirections: defaultSortOrder,
    },
    {
      title: 'eTrans ID',
      key: 'qboInvoiceId',
      dataIndex: 'qboInvoiceId',
      width: 130,
      align: 'center' as const,
      sorter: (a: any, b: any) => a.qboInvoiceId - b.qboInvoiceId,
      sortDirections: defaultSortOrder,
    },
    {
      title: 'eInvoice ID',
      key: 'eInvoiceID',
      width: 130,
      align: 'center' as const,
      render: (val: any) =>
        val.eInvoiceId && (
          <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${val.qboInvoiceId}`} target="_blank">
            {val.eInvoiceId}
          </a>
        ),
      sorter: (a: any, b: any) => a.eInvoiceId - b.eInvoiceId,
      sortDirections: defaultSortOrder,
    },

    {
      title: 'Company',
      key: 'companyName',
      render: (item: any) => <Popover content={content(item)}>{item.companyName}</Popover>,
      sorter: (a: any, b: any, sortOrder: any) => {
        let values = CheckNull(a.companyName, b.companyName, sortOrder);
        return values[0].localeCompare(values[1]);
      },
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Consultant',
      key: 'consultantName',
      dataIndex: 'consultantName',
      sorter: (a: any, b: any, sortOrder: any) => {
        let values = CheckNull(a.consultantName, b.consultantName, sortOrder);
        return values[0].localeCompare(values[1]);
      },
      sortDirections: defaultSortOrder,
    },
    {
      title: 'End Client',
      key: 'endClientName',
      dataIndex: 'endClientName',
      sorter: (a: any, b: any, sortOrder: any) => {
        let values = CheckNull(a.endClientName, b.endClientName, sortOrder);
        return values[0].localeCompare(values[1]);
      },
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Issue Date',
      key: 'invoiceDate',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'invoiceDate',
      sorter: (a: any, b: any) => moment(a.invoiceDate).diff(b.invoiceDate),
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Due Date',
      key: 'dueDate',
      render: (val: string) => (val !== null ? moment(val).format('MM/DD/YYYY') : '--'),
      dataIndex: 'dueDate',
      sorter: (a: any, b: any) => moment(a.dueDate).diff(b.dueDate),
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Amount',
      key: 'total',
      render: (val: number) => '$ ' + val.toLocaleString('en-US'),
      dataIndex: 'total',
      sorter: (a: any, b: any) => a.total - b.total,
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Balance',
      key: 'balance',
      dataIndex: 'balance',
      render: (val: number) => (val === null ? null : '$ ' + val.toLocaleString('en-US')),
      sorter: (a: any, b: any) => a.balance - b.balance,
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Overdue By',
      key: 'overdueBy',
      render: (item: any) => {
        let currentDate = moment().format('YYYY-MM-DD');
        let dueDate = moment(item.dueDate).format('YYYY-MM-DD');
        let days = moment(currentDate).diff(dueDate, 'days');
        return !isOverdue(item) ? '' : days === 1 ? days + ' day' : days + ' days';
      },
      sorter: (a: any, b: any) => {
        let currentDate = moment().format('YYYY-MM-DD');
        if (a.balance === 0 || !a.balance) {
          return -1 - moment(currentDate).diff(moment(b.dueDate).format('YYYY-MM-DD'), 'days');
        } else if (b.balance === 0 || !b.balance) {
          return moment(currentDate).diff(moment(a.dueDate).format('YYYY-MM-DD'), 'days') + 1;
        } else {
          return (
            moment(currentDate).diff(moment(a.dueDate).format('YYYY-MM-DD'), 'days') -
            moment(currentDate).diff(moment(b.dueDate).format('YYYY-MM-DD'), 'days')
          );
        }
      },
      defaultSortOrder: 'descend' as 'descend',
      sortDirections: defaultSortOrder,
    },
  ];

  const headerRender = () => {
    return (
      <Row type="flex" justify="space-between" align="middle" style={{ height: 45 }}>
        <Col>
          <Row type="flex" align="middle">
            <Col>
              <PredefinedQueryPills
                selectedFilter={queryName}
                size="small"
                dataSource={predefinedQueries}
                onClick={(name: any) => setQueryName(name)}
              />
            </Col>
          </Row>
        </Col>
        <Col>
          <Row type="flex" align="middle" gutter={8}>
            <Col>
              <EntityPicker
                url="api/services/app/Company/Search"
                mapFun={(r) => ({ value: r.id, text: `${r.displayName}` })}
                style={{ width: '220px' }}
                value={companySearchText}
                placeholder="Search Company"
                onChange={handleCompanySearch}
              />
            </Col>
            <Col>
              <EntityPicker
                url="api/services/app/Consultant/Search"
                mapFun={(r) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
                style={{ width: '220px' }}
                value={consultantSearchText}
                placeholder="Search Consultant"
                onChange={handleConsultantSearch}
              />
            </Col>
            <Col>
              <RangePicker onChange={handleDateSearch} value={dateSearchText} />
            </Col>
          </Row>
        </Col>
      </Row>
    );
  };

  const refetch = () => {
    makeRequest({ params: { isActive: true } });
  };

  result.listItemDto && console.log(result.listItemDto.results);

  return (
    <>
      <Card>
        <h1>INVOICES</h1>
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
          dataSource={result.listItemDto && result.listItemDto.results}
          rowClassName={(record: any) => {
            return isOverdue(record) ? 'overdue' : '';
          }}
          columns={columns}
          size="small"
          pagination={{ pageSize: 10, total: 0, current: currentPage, size: 'default' }}
          loading={loading}
          tableLayout="auto"
          title={() => headerRender()}
          onRow={(record: any, rowIndex: any) => ({
            onDoubleClick: () => history.push(`/invoices/${record.id}`),
          })}
          onChange={(pagination: any, filters: any, sorter: any) => {
            setCurrentPage(pagination.current);
            if (sorter.order !== order || sorter.columnKey !== columnKey) {
              setOrder(sorter.order);
              setColumnKey(sorter.columnKey);
              setCurrentPage(1);
            }
          }}
        ></StyledTable>
        <Row type="flex" justify="end">
          <Col>{result.lastUpdated && <h4>Last Updated on : {moment(result.lastUpdated).format('MM/DD/YYYY hh:mm:ss A')} </h4>}</Col>
        </Row>
      </Card>

      <Portal>
        <RouteableDrawer path={[`/invoices/:invoiceId`]} width={'60vw'} title="Invoice Detail">
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
