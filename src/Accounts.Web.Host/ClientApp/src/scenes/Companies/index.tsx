import React, { useState, useEffect } from 'react';
import { Card, Row, Col, notification, Tooltip } from 'antd';
import useAxios from '../../lib/axios/useAxios';
import { L } from '../../lib/abpUtility';
import ActionButton from '../../components/ActionButton';
import { AxiosError } from 'axios';
import AppConsts from '../../lib/appconst';
import { useHistory } from 'react-router';
import CustomTable from '../../components/Custom/CustomTable';
import CustomSearch from '../../components/Custom/CustomSearch';
import CustomButton from './../../components/Custom/CustomButton';
import { Portal } from 'react-portal';
import RouteableDrawer from './../../components/RouteableDrawer';
import CompanyCreateUpdate from './components/CompanyCreateUpdate';
import Authorize from '../../components/Authorize';
import CustomEditButton from './../../components/Custom/CustomEditButton';
import { Get } from './../../lib/axios/index';
import styled from '@emotion/styled';

const StyledSpan = styled.span`
  display: block;
  width: 200px;
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
`;

export default () => {
  const history = useHistory();
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

  const columns = [
    {
      title: 'Name',
      dataIndex: 'displayName',
      key: 'displayName',
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: 'Phone Number',
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
    },
    {
      title: 'Term',
      dataIndex: 'termName',
      key: 'termName',
    },
    {
      title: 'Invoice Cycle',
      dataIndex: 'invoiceCycleName',
      key: 'invoiceCycleName',
    },
    {
      title: 'Notes',
      dataIndex: 'notes',
      key: 'notes',
      render: (val: string) => (
        <Tooltip title={val}>
          <StyledSpan>{val}</StyledSpan>
        </Tooltip>
      ),
    },
    {
      title: 'Actions',
      key: 'action',
      render: (record: any) => (
        <Authorize permissions={['Endclient.Update']}>
          <CustomEditButton icon="edit" type="primary" onClick={() => history.push(`/companies/${record.externalCustomerId}/edit`)} />
        </Authorize>
      ),
    },
  ];

  return (
    <Card>
      <Row type="flex" justify="space-between">
        <Col>
          <h1>{L('COMPANIES')}</h1>
        </Col>
        <Col>
          <ActionButton
            permissions={['Company.Sync']}
            method="GET"
            url="/api/services/app/Intuit/SyncPaymentMethods"
            style={{ marginRight: 15, height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
            onSuccess={(response: any) => {
              notification.open({
                message: 'Success',
                description: 'Payment methods successfully synced.',
              });
            }}
            onError={(err: AxiosError) => {
              if (err && err.response && err.response.status == 403) {
                window.location.href = `${AppConsts.remoteServiceBaseUrl}/Intuit/Login?returnUrl=${window.location.href}`;
              }
            }}
          >
            Sync Payment Methods
          </ActionButton>
          <ActionButton
            permissions={['Company.Sync']}
            method="GET"
            url={`/api/services/app/Intuit/SyncTerms`}
            style={{ marginRight: 15, height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
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
            style={{ marginRight: 15, height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
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
          <Authorize permissions={['Company.Create']}>
            <CustomButton type="primary" icon="plus" onClick={() => history.push('/companies/new')}>
              Add Company
            </CustomButton>
          </Authorize>
        </Col>
      </Row>
      <Row>
        <Col sm={{ span: 8, offset: 0 }}>
          <CustomSearch placeholder={L('Search')} onSearch={setSearchText} />
        </Col>
      </Row>
      <Row style={{ marginTop: 20 }}>
        <Col>
          <CustomTable
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
      <Portal>
        <Authorize permissions={['Company.Create']}>
          <RouteableDrawer path={['/companies/new']} width={'40vw'} title="Add New Company">
            {({ onClose }: any) => {
              return (
                <CompanyCreateUpdate
                  onClose={onClose}
                  onCompanyAddedOrUpdated={() => {
                    onClose();
                    makeRequest({});
                  }}
                />
              );
            }}
          </RouteableDrawer>
        </Authorize>
        <RouteableDrawer path={['/companies/:externalCustomerId/edit']} width={'40vw'} title="Edit Company" exact={true}>
          {({
            match: {
              params: { externalCustomerId },
            },
            onClose,
          }: any) => {
            return (
              <Get url="/api/services/app/Intuit/GetCompany" params={{ externalCustomerId: externalCustomerId }}>
                {({ error, data, isLoading }: any) => {
                  return (
                    <CompanyCreateUpdate
                      isEdited={true}
                      onClose={onClose}
                      company={data && data.result}
                      onCompanyAddedOrUpdated={() => {
                        onClose();
                        makeRequest({});
                      }}
                    />
                  );
                }}
              </Get>
            );
          }}
        </RouteableDrawer>
      </Portal>
    </Card>
  );
};
