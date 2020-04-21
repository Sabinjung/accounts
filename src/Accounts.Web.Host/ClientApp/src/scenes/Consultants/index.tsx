import React, { useEffect, useState } from 'react';
import { Portal } from 'react-portal';

import { Button, Table, Card, Row, Col, Input, Dropdown, Icon, Menu } from 'antd';

import RouteableDrawer from '../../components/RouteableDrawer';
import ConsultantCreateUpdate from '../../components/Domain/ConsultantCreateUpdate';
import { useHistory } from 'react-router';
import useAxios from '../../lib/axios/useAxios';
import { L } from '../../lib/abpUtility';
import Authorize from '../../components/Authorize';
import { Get } from '../../lib/axios';
import { NavLink } from 'react-router-dom';

const Search = Input.Search;

const createConsultantMenu = (consultantId: number) => (
  <Menu>
    <Menu.Item>
      <NavLink to={`/consultants/${consultantId}/edit`}>Edit</NavLink>
    </Menu.Item>
  </Menu>
);

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
    width: 30,
    title: '',
    dataIndex: '',
    render: (data: any, record: any) => {
      return (
        <Dropdown overlay={createConsultantMenu(record.id)}>
          <Icon type="ellipsis" rotate={90} style={{ cursor: 'pointer' }} />
        </Dropdown>
      );
    },
  },
];

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
  return (
    <Card>
      <Row>
        <Col
          xs={{ span: 4, offset: 0 }}
          sm={{ span: 4, offset: 0 }}
          md={{ span: 4, offset: 0 }}
          lg={{ span: 2, offset: 0 }}
          xl={{ span: 2, offset: 0 }}
          xxl={{ span: 2, offset: 0 }}
        >
          <h2>{L('Consultants')}</h2>
        </Col>
        <Col
          xs={{ span: 14, offset: 0 }}
          sm={{ span: 15, offset: 0 }}
          md={{ span: 15, offset: 0 }}
          lg={{ span: 1, offset: 21 }}
          xl={{ span: 1, offset: 21 }}
          xxl={{ span: 1, offset: 21 }}
        >
          <Button type="primary" shape="circle" icon="plus" onClick={() => history.push('/consultants/new')}></Button>
        </Col>
      </Row>
      <Row>
        <Col sm={{ span: 10, offset: 0 }}>
          <Search placeholder={L('Filter')} onSearch={setSearchText} allowClear />
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
      <Portal>
        <Authorize permissions={['Consultant.Create', 'Consultant.Update']}>
          <RouteableDrawer path={[`/consultants/new`]} width={'25vw'} title="Consultant">
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
          <RouteableDrawer path={['/consultants/:consultantId/edit']} width={'25vw'} title="Consultant" exact={true}>
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
