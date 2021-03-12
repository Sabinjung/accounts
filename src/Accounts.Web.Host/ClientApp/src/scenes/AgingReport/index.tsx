import React, { useState } from 'react';
import { Table, Card, Spin, Icon, Row, Col } from 'antd';
import moment from 'moment';
import styled from '@emotion/styled';
import useAxios from '../../lib/axios/useAxios';
import EntityPicker from '../../components/EntityPicker';
const StyledDays = styled.span`
  font-weight: 700;
`;
const StyledSpan = styled.span`
  font-weight: 600;
`;
const StyledH2 = styled.h2`
  font-size: 30px;
`;

const StyledSpin = styled(Spin)`
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
`;

const StyledIcon = styled(Icon)`
  color: #7f7f7f;
  font-size: 15px;
  margin-right: 5px;
  :hover {
    color: #3db389;
  }
`;

const StyledTable = styled(Table)`
  .changed {
    td:not(:first-child) {
      border-top: 2px solid #a9a9a9;
    }
  }
`;

const StyledCard = styled(Card)`
  height: 100%;
`;

type DisplayContentProps = {
  data: any;
  loading: boolean;
  setCompanyId: any;
  companyId: any;
};

const DisplayContent: React.FC<DisplayContentProps> = ({ data, loading, setCompanyId, companyId }) => {
  let allKeys: any = [];
  let filteredData: any = [];

  const getChildren = (val: any, newChildren: any, index: number) => {
    return [
      ...newChildren,
      {
        key: index,
        companyName: val.companyName,
        eInvoiceId: val.eInvoiceId,
        consultantName: val.consultantName,
        balance: val.balance,
        totalBalance: val.totalBalance,
        invoiceDate: val.invoiceDate,
        dueDate: val.dueDate,
        total: val.total,
        qboInvoiceId: val.qboInvoiceId,
      },
    ];
  };
  data &&
    data.result.map((item: any) => {
      allKeys = [...allKeys, item.key];
      let newChildren: any = [];
      let initalComapany = '';
      item.children &&
        item.children.map((val: any, index: number) => {
          if (index === 0) {
            initalComapany = val.companyName;
            newChildren = getChildren(val, newChildren, index);
          } else {
            if (val.companyName === initalComapany) {
              newChildren = [
                ...newChildren,
                {
                  key: index,
                  eInvoiceId: val.eInvoiceId,
                  consultantName: val.consultantName,
                  balance: val.balance,
                  invoiceDate: val.invoiceDate,
                  dueDate: val.dueDate,
                  total: val.total,
                  qboInvoiceId: val.qboInvoiceId,
                },
              ];
            } else {
              initalComapany = val.companyName;
              newChildren = getChildren(val, newChildren, index);
            }
          }
          return newChildren;
        });
      return (filteredData = [...filteredData, { key: item.key, days: item.days, children: newChildren }]);
    });
  const [expandedRows, setExpandedRows] = useState(allKeys);
  const handleRowExpand = (record: any) => {
    if (expandedRows.includes(record.key)) {
      let rows = expandedRows.filter((key: number) => key !== record.key);
      setExpandedRows([...rows]);
    } else {
      setExpandedRows([...expandedRows, record.key]);
    }
  };

  const handleExpandAll = () => {
    expandedRows.length === 0 ? setExpandedRows(allKeys) : setExpandedRows([]);
  };

  const columns = [
    {
      title: (
        <>
          {expandedRows.length === 0 ? (
            <StyledIcon type="plus-square" onClick={handleExpandAll} />
          ) : (
            <StyledIcon type="minus-square" onClick={handleExpandAll} />
          )}
          Days
        </>
      ),
      dataIndex: 'days',
      key: 'days',
      render: (val: any) => <StyledDays>{val}</StyledDays>,
    },
    {
      title: 'Company',
      dataIndex: 'companyName',
      key: 'companyName',
      render: (val: any) => <StyledSpan>{val}</StyledSpan>,
    },
    {
      title: 'eInvoice ID',
      key: 'eInvoiceId',
      render: (val: any) =>
        val.eInvoiceId && (
          <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${val.qboInvoiceId}`} target="_blank">
            {val.eInvoiceId}
          </a>
        ),
    },
    {
      title: 'Consultant',
      dataIndex: 'consultantName',
      key: 'consultantName',
    },
    {
      title: 'Issue Date',
      dataIndex: 'invoiceDate',
      key: 'invoiceDate',
      render: (val: any) => val && moment(val).format('MM/DD/YYYY'),
    },
    {
      title: 'Due Date',
      dataIndex: 'dueDate',
      key: 'dueDate',
      render: (val: any) => val && moment(val).format('MM/DD/YYYY'),
    },
    {
      title: 'Amount',
      dataIndex: 'total',
      key: 'total',
      render: (val: number) => val && '$ ' + val.toLocaleString('en-US'),
    },
    {
      title: 'Balance',
      dataIndex: 'balance',
      key: 'balance',
      render: (val: number) => val && '$ ' + val.toLocaleString('en-US'),
    },
    {
      title: 'Total Balance',
      dataIndex: 'totalBalance',
      key: 'totalBalance',
      render: (val: number) => val && <StyledSpan>{'$ ' + val.toLocaleString('en-US')}</StyledSpan>,
    },
  ];
  return (
    <StyledCard>
      <Row type="flex" justify="space-between">
        <Col>
          <StyledH2>AGING REPORT</StyledH2>
        </Col>
        <Col>
          <EntityPicker
            placeholder="Search Company"
            style={{ width: '250px' }}
            size="large"
            value={companyId}
            url="/api/services/app/Company/Search"
            mapFun={(r) => ({ value: r.id, text: r.displayName })}
            onChange={(val: any) => {
              setCompanyId(val);
            }}
          />
        </Col>
      </Row>
      {loading ? (
        <StyledSpin size="large" />
      ) : (
        <StyledTable
          rowClassName={(record: any, index: number) => (index && record.companyName ? 'changed' : '')}
          scroll={{ y: 720 }}
          pagination={false}
          columns={loading ? [] : columns}
          dataSource={filteredData}
          onExpand={(expanded, record) => handleRowExpand(record)}
          expandedRowKeys={expandedRows}
        />
      )}
    </StyledCard>
  );
};

const AgingReport: React.FC = () => {
  const [companyId, setCompanyId] = useState();

  const [{ data, loading }] = useAxios({
    url: '/api/services/app/Invoice/GetAgeingReport',
    params: { companyId },
  });

  if (!data) {
    return <StyledSpin size="large" />;
  } else {
    return <DisplayContent data={data} loading={loading} setCompanyId={setCompanyId} companyId={companyId} />;
  }
};

export default AgingReport;
