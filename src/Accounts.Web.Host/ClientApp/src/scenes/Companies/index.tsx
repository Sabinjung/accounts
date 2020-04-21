import React, { useState, useEffect } from 'react';

import { Table, Card, Row, Col, Input, notification } from 'antd';

import useAxios from '../../lib/axios/useAxios';
import { L } from '../../lib/abpUtility';
import ActionButton from '../../components/ActionButton';
import { AxiosError } from 'axios';
import AppConsts from '../../lib/appconst';

const Search = Input.Search;

const columns = [
  {
    title: 'Name',
    dataIndex: 'displayName',
  },
  {
    title: 'Email',
    dataIndex: 'email',
  },
  {
    title: 'Phone Number',
    dataIndex: 'phoneNumber',
  },
  {
    title: 'Term',
    dataIndex: 'termName',
  },
];

export default () => {
  const [searchText, setSearchText] = useState('');
  const [skipCount, setSkipCount] = useState(0);
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Company/GetAll',
  });

  useEffect(() => {
    makeRequest({ params: { maxResultCount: 10, skipCount: skipCount, keyword: searchText } });
  }, [searchText, skipCount]);

  const {
    result: { items, totalCount },
  } = data || { result: { items: [], totalCount: 0 } };
  return (
    <Card>
      <Row type="flex" justify="space-between">
        <Col>
          <h2>{L('Companies')}</h2>
        </Col>
        <Col>
          <ActionButton
            permissions={['Company.Sync']}
            method="GET"
            url={`/api/services/app/Intuit/SyncTerms`}
            style={{ marginRight: 15 }}
            onSuccess={(response: any) => {
              notification.open({
                message: 'Success',
                description: 'Terms successfully synced.',
              });
            }}
            onError={(err: AxiosError) => {
              if (err && err.response && err.response.status == 403) {
                window.location.href = `${AppConsts.remoteServiceBaseUrl}/Intuit/Login?returnUrl=${window.location.href}`;
              }
            }}
          >
            Sync Terms
          </ActionButton>
          <ActionButton
            method="GET"
            permissions={['Company.Sync']}
            url={`/api/services/app/Intuit/SyncCompanies`}
            onSuccess={(response: any) => {
              notification.open({
                message: 'Success',
                description: 'Companies successfully synced.',
              });
              makeRequest({});
            }}
            onError={(err: AxiosError) => {
              if (err && err.response && err.response.status == 403) {
                window.location.href = `${AppConsts.remoteServiceBaseUrl}/Intuit/Login?returnUrl=${window.location.href}`;
              }
            }}
          >
            Sync Companies
          </ActionButton>
        </Col>
      </Row>
      <Row>
        <Col sm={{ span: 10, offset: 0 }}>
          <Search placeholder={L('Filter')} onSearch={setSearchText} allowClear />
        </Col>
      </Row>
      <Row style={{ marginTop: 20 }}>
        <Col>
          <Table
            loading={loading}
            dataSource={items}
            columns={columns}
            pagination={{ pageSize: 10, total: data === undefined ? 0 : totalCount, defaultCurrent: 1 }}
            onChange={(pagination: any) => {
              const skipCount = (pagination.current - 1) * 10;
              setSkipCount(skipCount);
            }}
          />
        </Col>
      </Row>
    </Card>
  );
};
