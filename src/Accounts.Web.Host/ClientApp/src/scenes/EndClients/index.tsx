import React, { useState } from 'react';
import { Row, Col, Card, notification, Form, Input } from 'antd';
// import { Row, Col, Card, Table, Button, Input } from 'antd';
import useAxios from '../../lib/axios/useAxios';
import EndClientCreateUpdate from './components/EndClientCreateUpdate';
import { FormComponentProps } from 'antd/lib/form';
import ConfirmActionButton from '../../components/ConfirmActionButton';
import Authorize from '../../components/Authorize';
import CustomSearch from './../../components/Custom/CustomSearch';
import CustomTable from './../../components/Custom/CustomTable';
import CustomButton from './../../components/Custom/CustomButton';
import CustomEditButton from './../../components/Custom/CustomEditButton';
import { useHistory } from 'react-router';
import { Portal } from 'react-portal';
import { Get } from '../../lib/axios';
import RouteableDrawer from '../../components/RouteableDrawer';
import styled from '@emotion/styled';

type DisplayContentProps = FormComponentProps<{}> & {
  data: any;
  loading: boolean;
  setSearchText: any;
  makeRequest: any;
};

const { TextArea } = Input;

const StyledDiv = styled.div`
textarea.ant-input {
  margin-top: -20px;
}
}
`;

const StyledInnerDiv = styled.div`
 height: 110px;
`;

const DisplayContent: React.FC<DisplayContentProps> = ({ data, loading, setSearchText, makeRequest, form }) => {
  const history = useHistory();
  const results: any = data && data.result.results;
  const { getFieldDecorator, validateFields } = form;
  const [disable, setDisable] = useState<boolean>(true);
  const handleTextLength = (e: any) => {
    const disable = e.target.value && e.target.value.length > 0 ? false : true;
    setDisable(disable);
  };

  const handleVisibleChange = (e: any) => {
    if(e){
      form.resetFields();
      setDisable(true);
    }
  }
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
        <StyledDiv>
          <Authorize permissions={['Endclient.Update']}>
            <CustomEditButton icon="edit" type="primary" onClick={() => history.push(`/endClients/${record.id}/edit`)} />
          </Authorize>
          <ConfirmActionButton
            style={{ border: 'none', background: '#FF00001A', color: '#FF0000' }}
            url="/api/services/app/EndClient/DeleteClient"
            params={{ id: record.id }}
            disable={disable}
            onVisibleChange = {handleVisibleChange}
            method="Delete"
            type="danger"
            icon="delete"
            placement="bottom"
            onSuccess={() => {
              notification.open({
                message: 'Success',
                description: 'Deleted successfully.',
              });
              form.resetFields();
              makeRequest({});
            }}
            permissions={['Endclient.Delete']}
            onSubmit={({ setFormData }: any) => {
              let shouldProceed = false;
              validateFields((errors: any, values: any) => {
                if (!errors) {
                  const { noteText } = values;
                  setFormData({
                    noteText: noteText,
                  });
                  shouldProceed = true;
                } else {
                  shouldProceed = false;
                }
              });
              return shouldProceed;
            }}
          >
            {() => {
              return (
                <StyledInnerDiv>
                  Do you want to delete this end client?
                  <Form>
                    <Form.Item>
                      {getFieldDecorator('noteText', {
                        rules: [{ required: true, message: 'Please enter the reason for deletion?' }],
                      })(<TextArea onChange={handleTextLength} />)}
                    </Form.Item>
                  </Form>
                </StyledInnerDiv>
              );
            }}
          </ConfirmActionButton>
        </StyledDiv>
      ),
    },
  ];

  return (
    <Card>
      <Row type="flex" justify="space-between">
        <Col>
          <h1>END CLIENTS</h1>
        </Col>
        <Authorize permissions={['Endclient.Create']}>
          <Col>
            <CustomButton type="primary" icon="plus" onClick={() => history.push('/endClients/new')}>
              Add End Client
            </CustomButton>
          </Col>
        </Authorize>
      </Row>
      <Row style={{ marginBottom: 20 }}>
        <Col sm={{ span: 8, offset: 0 }}>
          <CustomSearch placeholder="Search" onSearch={setSearchText} />
        </Col>
      </Row>
      <CustomTable dataSource={results} columns={columns} loading={loading} />
      <Portal>
        <Authorize permissions={['Endclient.Create', 'Endclient.Update']}>
          <RouteableDrawer path={[`/endClients/new`]} width={'25vw'} title="Add New End Client">
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
          <RouteableDrawer path={['/endClients/:endClientId/edit']} width={'25vw'} title="Edit End Client" exact={true}>
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

const WrappedDisplayContent = Form.create<DisplayContentProps>({
  name: 'endClient_state',
})(DisplayContent);

const EndClients = () => {
  const [searchText, setSearchText] = useState('');

  const [{ data, loading }, makeRequest] = useAxios({
    url: '/api/services/app/EndClient/Search',
    params: { SearchText: searchText },
  });

  return <WrappedDisplayContent data={data} loading={loading} setSearchText={setSearchText} makeRequest={makeRequest} />;
};

export default EndClients;
