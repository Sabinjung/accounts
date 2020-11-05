import React, { useState } from 'react';
import { Row, Col, Card, Table, Button, Modal, Input } from 'antd';
import useAxios from '../../lib/axios/useAxios';
import styled from '@emotion/styled';
import EndClientCreateUpdate from './components/EndClientCreateUpdate';
import ConfirmActionButton from '../../components/ConfirmActionButton';
import Authorize from '../../components/Authorize';

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

let rowData: any;
const DisplayContent: React.FC<DisplayContentProps> = ({ data, loading, setSearchText, makeRequest }) => {
  const [visible, setVisible] = useState(false);
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
            <StyledEditButton icon="edit" type="primary" onClick={() => (setVisible(true), (rowData = record))} />
          </Authorize>
          <ConfirmActionButton
            url="/api/services/app/EndClient/Delete"
            params={{ Id: record.id }}
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
            <Button type="primary" shape="circle" icon="plus" onClick={() => (setVisible(true), (rowData = ''))}></Button>
          </Col>
        </Authorize>
      </Row>
      <StyledSearch placeholder="Filter" onSearch={setSearchText} allowClear />
      <Table dataSource={results} columns={columns} loading={loading} />
      <Modal title="End Client" visible={visible} footer={false} destroyOnClose={true} onCancel={() => setVisible(false)}>
        <EndClientCreateUpdate setVisible={setVisible} makeRequest={makeRequest} rowData={rowData} />
      </Modal>
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
