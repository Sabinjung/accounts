import React, { useState, useEffect } from 'react';
import { Card, Col, Row } from 'antd';
import { L } from '../../../lib/abpUtility';
import styled from '@emotion/styled';
import useAxios from '../../../lib/axios/useAxios';
import { Portal } from 'react-portal';
import { useHistory } from 'react-router';
import { Get } from '../../../lib/axios';
import HourLogYearlyReport from './HourLogYearlyReport';
import RouteableDrawer from '../../../components/RouteableDrawer';
import CustomTable from '../../../components/Custom/CustomTable';
import CustomSearch from '../../../components/Custom/CustomSearch';
import Authorize from '../../../components/Authorize';

type sortableType = ['descend', 'ascend', 'descend'];
const defaultSortOrder: sortableType = ['descend', 'ascend', 'descend'];

const StyledCard = styled(Card)`
  .wideRangePicker {
    width: 552px;
  }
  .ant-calendar-input {
    text-align: center;
  }
  .ant-calendar-range-middle {
    margin-left: 75px;
  }
`;

const HourLogReport = () => {
  const [filteredData, setFilteredData] = useState([]);
  const history = useHistory();

  const [{ data, loading }] = useAxios({
    url: 'api/services/app/HourLogEntry/GetHourLogReport',
  });
  
  const dataSource = data && data.result;

  useEffect(() => {
    dataSource && setFilteredData(dataSource);
  }, [loading])

  const handleSearch = (searchText: string) => {
    const filteredData = dataSource && dataSource.filter((obj: any) => Object.keys(obj).some(() => obj.displayName.toLowerCase().includes(searchText.toLowerCase())));
    setFilteredData(filteredData);
  };

  const CheckNull = (a: string, b: string, sortOrder: string) => {
    let initialName: string = a ? a : sortOrder === 'ascend' ? 'z' : 'a';
    let nextName: string = b ? b : sortOrder === 'ascend' ? 'z' : 'a';
    return [initialName, nextName];
  };

  const columns = [
    {
      title: 'Consultant',
      dataIndex: 'displayName',
      key: 'displayName',
      width: '35%',
      sorter: (a: any, b: any, sortOrder: any) => {
        let values = CheckNull(a.displayName, b.displayName, sortOrder);
        return values[0].localeCompare(values[1]);
      },
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Total Invoiced Hours',
      dataIndex: 'totalInvoicedHours',
      key: 'totalInvoicedHours',
      sorter: (a: any, b: any) => a.totalInvoicedHours - b.totalInvoicedHours,
      sortDirections: defaultSortOrder,
    },
    {
      title: 'Total Unassociated Hours',
      dataIndex: 'totalNonInvoicedHours',
      key: 'totalNonInvoicedHours',
      sorter: (a: any, b: any) => a.totalNonInvoicedHours - b.totalNonInvoicedHours,
      sortDirections: defaultSortOrder,
    },
  ];
  return (
    <>
      <StyledCard>
        <Row>
          <Col>
            <h1>{L('HOUR LOGS REPORT')}</h1>
          </Col>
        </Row>
        <Row>
          <Col sm={{ span: 8, offset: 0 }}>
            <CustomSearch placeholder={L('Search')} onSearch={handleSearch} />
          </Col>
        </Row>
        <Row style={{ marginTop: 20 }}>
          <Col
            xs={{ span: 24, offset: 0 }}
            sm={{ span: 24, offset: 0 }}
            md={{ span: 24, offset: 0 }}
            lg={{ span: 24, offset: 0 }}
            xl={{ span: 24, offset: 0 }}
            xxl={{ span: 24, offset: 0 }}
          ></Col>
          <Col>
            <CustomTable
              loading={loading}
              dataSource={filteredData}
              columns={columns}
              onRow={(record: any, rowIndex: any) => ({
                onDoubleClick: () => history.push(`/hourlogsReport/${record.projectId}/unassociatedhours`),
              })}
              pagination={{ pageSize: 10, total: filteredData === undefined ? 0 : filteredData.length, defaultCurrent: 1 }}
            />
          </Col>
        </Row>
      </StyledCard>
      <Portal>
      <Authorize permissions={['HourLog.Report']}>
        <RouteableDrawer path={['/hourlogsReport/:projectId/unassociatedhours']} width={'50vw'} title="Unassociated Hour Logs Report">
          {({
            match: {
              params: { projectId },
            },
            onClose,
          }: any) => {
            return (
              <Get url="api/services/app/HourLogEntry/GetHourLogReportDetails" params={{ projectId: projectId }}>
                {({ error, data, loading }: any) => {
                  const result = data && data.result;
                  return (
                    <HourLogYearlyReport
                      onClose={() => {
                        onClose();
                      }}
                      dataSource={result}
                      loading={loading}
                    />
                  );
                }}
              </Get>
            );
          }}
        </RouteableDrawer>
        </Authorize>
      </Portal>
    </>
  );
};

export default HourLogReport;
