import React, { useState } from 'react';
import { Row, Col, Card, Table, Button, Input } from 'antd';
import useAxios from '../../lib/axios/useAxios';
import styled from '@emotion/styled';
import EndClientCreateUpdate from './components/EndClientCreateUpdate';
import ConfirmActionButton from '../../components/ConfirmActionButton';
import Authorize from '../../components/Authorize';
import { useHistory } from 'react-router';
import { Portal } from 'react-portal';
import { Get } from '../../lib/axios';
import RouteableDrawer from '../../components/RouteableDrawer';

const { Search } = Input;

type DisplayContentProps = {
  data: any;
  loading: boolean;
  setSearchText: any;
  makeRequest: any;
};

const StyledSearch = styled(Search)`
  width: 500px;
  margin-bottom: 20px;
`;
const StyledEditButton = styled(Button)`
  margin-right: 8px;
`;

const DisplayContent: React.FC<DisplayContentProps> = ({ data, loading, setSearchText, makeRequest }) => {
  const history = useHistory();
  const results: any = data && data.result.results;
  const columns = [
    {
      title: 'Client Name',
      key: 'clientName',
      dataIndex: 'clientName',
    },
    {
      title: 'Actions',
      key: 'action',
      render: (record: any) => (
        <>
          <Authorize permissions={['Endclient.Update']}>
            <StyledEditButton icon="edit" type="primary" onClick={() => history.push(`/endClients/${record.id}/edit`)} />
          </Authorize>
          <ConfirmActionButton
            url="/api/services/app/EndClient/DeleteClient"
            params={{ id: record.id }}
            method="Delete"
            type="danger"
            icon="delete"
            placement="top"
            onSuccess={() => {
              makeRequest({});
            }}
            permissions={['Endclient.Delete']}
          >
            {() => {
              return <div>Do you want to delete this End Client?</div>;
            }}
          </ConfirmActionButton>
        </>
      ),
    },
  ];

  return (
    <Card>
      <Row type="flex" justify="space-between">
        <Col>
          <h2>End Clients</h2>
        </Col>
        <Authorize permissions={['Endclient.Create']}>
          <Col>
            <Button type="primary" shape="circle" icon="plus" onClick={() => history.push('/endClients/new')}></Button>
          </Col>
        </Authorize>
      </Row>
      <StyledSearch placeholder="Filter" onSearch={setSearchText} allowClear />
      <Table dataSource={results} columns={columns} loading={loading} />
      <Portal>
        <Authorize permissions={['Endclient.Create', 'Endclient.Update']}>
          <RouteableDrawer path={[`/endClients/new`]} width={'25vw'} title="EndClient">
            {({ onClose }: any) => {
              return (
                <EndClientCreateUpdate
                  onClose={onClose}
                  onEndClientAddedOrUpdated={() => {
                    onClose();
                    makeRequest({});
                  }}
                />
              );
            }}
          </RouteableDrawer>
        </Authorize>
        <Authorize permissions={['Endclient.Update']}>
          <RouteableDrawer path={['/endClients/:endClientId/edit']} width={'25vw'} title="EndClient" exact={true}>
            {({
              match: {
                params: { endClientId },
              },
              onClose,
            }: any) => {
              return (
                <Get url="/api/services/app/EndClient/Get" params={{ id: endClientId }}>
                  {({ error, data, isLoading }: any) => {
                    return (
                      <EndClientCreateUpdate
                        onClose={onClose}
                        endClient={data && data.result}
                        onEndClientAddedOrUpdated={() => {
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

const EndClients = () => {
  const [searchText, setSearchText] = useState('');

  const [{ data, loading }, makeRequest] = useAxios({
    url: '/api/services/app/EndClient/Search',
    params: { SearchText: searchText },
  });

  return <DisplayContent data={data} loading={loading} setSearchText={setSearchText} makeRequest={makeRequest} />;
};

export default EndClients;
