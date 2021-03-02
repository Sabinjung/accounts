import * as React from 'react';

import { Card, Col, Modal, Row, notification } from 'antd';
import { inject, observer } from 'mobx-react';

import AppComponentBase from '../../components/AppComponentBase';
import CreateOrUpdateUser from './components/createOrUpdateUser';
import { EntityDto } from '../../services/dto/entityDto';
import { L } from '../../lib/abpUtility';
import Stores from '../../stores/storeIdentifier';
import UserStore from '../../stores/userStore';
import CustomButton from './../../components/Custom/CustomButton';
import CustomSearch from './../../components/Custom/CustomSearch';
import CustomTable from './../../components/Custom/CustomTable';
import CustomEditButton from './../../components/Custom/CustomEditButton';
import CustomDeleteButton from './../../components/Custom/CustomDeleteButton';
import styled from '@emotion/styled';

const StyledYesTag = styled.div`
  padding: 8px;
  width: 56px;
  color: #2680eb;
  background: #2680eb1a;
  text-align: center;
  border-radius: 5px;
`;
const StyledNoTag = styled.div`
  padding: 8px;
  width: 56px;
  color: #ff0000;
  background: #ff00001a;
  text-align: center;
  border-radius: 5px;
`;

export interface IUserProps {
  userStore: UserStore;
}

export interface IUserState {
  modalVisible: boolean;
  maxResultCount: number;
  skipCount: number;
  userId: number;
  filter: string;
  loading: boolean;
}

const confirm = Modal.confirm;

@inject(Stores.UserStore)
@observer
class User extends AppComponentBase<IUserProps, IUserState> {
  formRef: any;

  state = {
    modalVisible: false,
    maxResultCount: 10,
    skipCount: 0,
    userId: 0,
    filter: '',
    loading: true,
  };

  async componentDidMount() {
    await this.getAll();
  }

  async getAll() {
    await this.props.userStore.getAll({ maxResultCount: this.state.maxResultCount, skipCount: this.state.skipCount, keyword: this.state.filter });
    this.setState({
      loading: false,
    });
  }

  handleTableChange = (pagination: any) => {
    this.setState({ skipCount: (pagination.current - 1) * this.state.maxResultCount! }, async () => await this.getAll());
  };

  Modal = () => {
    this.setState({
      modalVisible: !this.state.modalVisible,
    });
  };

  async createOrUpdateModalOpen(entityDto: EntityDto) {
    if (entityDto.id === 0) {
      await this.props.userStore.createUser();
      await this.props.userStore.getRoles();
    } else {
      await this.props.userStore.get(entityDto);
      await this.props.userStore.getRoles();
    }

    this.setState({ userId: entityDto.id });
    this.Modal();

    this.formRef.props.form.setFieldsValue({ ...this.props.userStore.editUser, roleNames: this.props.userStore.editUser.roleNames });
  }

  delete(input: EntityDto) {
    const self = this;
    confirm({
      title: 'Do you Want to delete these User?',
      async onOk() {
        await self.props.userStore.delete(input);
        notification.open({
          message: 'Success',
          description: 'User Deleted Successfully.',
        });
      },
      onCancel() {
        console.log('Cancel');
      },
    });
  }

  handleCreate = () => {
    const form = this.formRef.props.form;

    form.validateFields(async (err: any, values: any) => {
      if (err) {
        return;
      } else {
        this.setState({ modalVisible: false });
        this.setState({
          loading: true,
        });
        if (this.state.userId === 0) {
          await this.props.userStore.create(values);
          notification.open({
            message: 'Success',
            description: 'User Created Successfully.',
          });
        } else {
          await this.props.userStore.update({ id: this.state.userId, ...values });
          notification.open({
            message: 'Success',
            description: 'User Updated Successfully.',
          });
        }
      }

      await this.getAll();

      form.resetFields();
    });
  };

  saveFormRef = (formRef: any) => {
    this.formRef = formRef;
  };

  handleSearch = (value: string) => {
    this.setState({
      loading: true,
    });
    this.setState({ filter: value }, async () => await this.getAll());
  };

  public render() {
    const { users } = this.props.userStore;
    const columns = [
      { title: L('UserName'), dataIndex: 'userName', key: 'userName', width: 150, render: (text: string) => <div>{text}</div> },
      { title: L('FullName'), dataIndex: 'name', key: 'name', width: 150, render: (text: string) => <div>{text}</div> },
      { title: L('EmailAddress'), dataIndex: 'emailAddress', key: 'emailAddress', width: 150, render: (text: string) => <div>{text}</div> },
      {
        title: L('IsActive'),
        dataIndex: 'isActive',
        key: 'isActive',
        width: 150,
        render: (text: boolean) => (text === true ? <StyledYesTag>{L('Yes')}</StyledYesTag> : <StyledNoTag>{L('No')}</StyledNoTag>),
      },
      {
        title: L('Actions'),
        width: 150,
        render: (item: any) => (
          <>
            <CustomEditButton onClick={() => this.createOrUpdateModalOpen({ id: item.id })} />
            <CustomDeleteButton onClick={() => this.delete({ id: item.id })} />
          </>
        ),
      },
    ];

    return (
      <Card>
        <Row type="flex" justify="space-between" align="middle">
          <Col>
            <h1>{L('USERS')}</h1>
          </Col>
          <Col>
            <CustomButton type="primary" icon="plus" onClick={() => this.createOrUpdateModalOpen({ id: 0 })}>
              Add User
            </CustomButton>
          </Col>
        </Row>
        <Row>
          <Col sm={{ span: 8, offset: 0 }}>
            <CustomSearch placeholder={this.L('Search')} onSearch={this.handleSearch} />
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
          >
            <CustomTable
              rowKey={(record) => record.id}
              size={'default'}
              columns={columns}
              pagination={{ pageSize: 10, total: users === undefined ? 0 : users.totalCount, defaultCurrent: 1 }}
              loading={this.state.loading}
              dataSource={users === undefined ? [] : users.items}
              onChange={this.handleTableChange}
            />
          </Col>
        </Row>
        <CreateOrUpdateUser
          wrappedComponentRef={this.saveFormRef}
          visible={this.state.modalVisible}
          onCancel={() =>
            this.setState({
              modalVisible: false,
            })
          }
          modalType={this.state.userId === 0 ? 'create' : 'edit'}
          onCreate={this.handleCreate}
          roles={this.props.userStore.roles}
        />
      </Card>
    );
  }
}

export default User;
