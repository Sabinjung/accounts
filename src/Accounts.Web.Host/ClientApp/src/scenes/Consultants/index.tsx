import React, { useEffect, useState } from 'react';
import { Portal } from 'react-portal';

import { Card, Row, Col } from 'antd';

import RouteableDrawer from '../../components/RouteableDrawer';
import ConsultantCreateUpdate from '../../components/Domain/ConsultantCreateUpdate';
import { useHistory } from 'react-router';
import useAxios from '../../lib/axios/useAxios';
import { L } from '../../lib/abpUtility';
import Authorize from '../../components/Authorize';
import { Get } from '../../lib/axios';
import CustomSearch from './../../components/Custom/CustomSearch';
import CustomButton from '../../components/Custom/CustomButton';
import CustomTable from './../../components/Custom/CustomTable';
import CustomEditButton from './../../components/Custom/CustomEditButton';

export default () => {
  const history = useHistory();
  const [searchText, setSearchText] = useState('');
  const [skipCount, setSkipCount] = useState(0);
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Consultant/GetAll',
  });

  useEffect(() => {
    makeRequest({ params: { maxResultCount: 10, skipCount: skipCount, keyword: searchText } });
  }, [searchText, skipCount]);

  const {
    result: { items, totalCount },
  } = data || { result: { items: [], totalCount: 0 } };

  const columns = [
    {
      title: 'First Name',
      dataIndex: 'firstName',
    },
    {
      title: 'Last Name',
      dataIndex: 'lastName',
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
      title: 'Actions',
      render: (record: any) => <CustomEditButton onClick={() => history.push(`/consultants/${record.id}/edit`)} />,
    },
  ];

  return (
    <Card>
      <Row type="flex" justify="space-between" align="middle">
        <Col>
          <h1>{L('CONSULTANTS')}</h1>
        </Col>
        <Col>
          <CustomButton type="primary" icon="plus" onClick={() => history.push('/consultants/new')}>
            Add Consultant
          </CustomButton>
        </Col>
      </Row>
      <Row>
        <Col sm={{ span: 8, offset: 0 }}>
          <CustomSearch placeholder={L('Search')} onSearch={setSearchText} />
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
        <Authorize permissions={['Consultant.Create', 'Consultant.Update']}>
          <RouteableDrawer path={[`/consultants/new`]} width={'25vw'} title="Add New Consultant">
            {({ onClose }: any) => {
              return (
                <ConsultantCreateUpdate
                  onClose={onClose}
                  onConsultantAddedOrUpdated={() => {
                    onClose();
                    makeRequest({});
                  }}
                />
              );
            }}
          </RouteableDrawer>
        </Authorize>
        <Authorize permissions={['Consultant.Update']}>
          <RouteableDrawer path={['/consultants/:consultantId/edit']} width={'25vw'} title="Edit Consultant" exact={true}>
            {({
              match: {
                params: { consultantId },
              },
              onClose,
            }: any) => {
              return (
                <Get url="api/services/app/Consultant/Get" params={{ id: consultantId }}>
                  {({ error, data, isLoading }: any) => {
                    return (
                      <ConsultantCreateUpdate
                        onClose={onClose}
                        consultant={data && data.result}
                        onConsultantAddedOrUpdated={() => {
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
        </Authorize>
      </Portal>
    </Card>
  );
};
