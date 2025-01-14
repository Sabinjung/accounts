import * as React from 'react';

import { Card, Col, Modal, Row, notification } from 'antd';
import { inject, observer } from 'mobx-react';

import AppComponentBase from '../../components/AppComponentBase';
import CreateOrUpdateRole from './components/createOrUpdateRole';
import { EntityDto } from '../../services/dto/entityDto';
import { FormComponentProps } from 'antd/lib/form';
import { L } from '../../lib/abpUtility';
import RoleStore from '../../stores/roleStore';
import Stores from '../../stores/storeIdentifier';
import CustomButton from '../../components/Custom/CustomButton';
import CustomTable from './../../components/Custom/CustomTable';
import CustomSearch from '../../components/Custom/CustomSearch';
import CustomEditButton from './../../components/Custom/CustomEditButton';
import CustomDeleteButton from './../../components/Custom/CustomDeleteButton';

export interface IRoleProps extends FormComponentProps {
  roleStore: RoleStore;
}

export interface IRoleState {
  modalVisible: boolean;
  maxResultCount: number;
  skipCount: number;
  roleId: number;
  filter: string;
  loading: boolean;
}

const confirm = Modal.confirm;

@inject(Stores.RoleStore)
@observer
class Role extends AppComponentBase<IRoleProps, IRoleState> {
  formRef: any;

  state = {
    modalVisible: false,
    maxResultCount: 10,
    skipCount: 0,
    roleId: 0,
    filter: '',
    loading: true,
  };

  async componentDidMount() {
    await this.getAll();
  }

  async getAll() {
    await this.props.roleStore.getAll({ maxResultCount: this.state.maxResultCount, skipCount: this.state.skipCount, keyword: this.state.filter });
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
      this.props.roleStore.createRole();
      await this.props.roleStore.getAllPermissions();
    } else {
      await this.props.roleStore.getRoleForEdit(entityDto);
      await this.props.roleStore.getAllPermissions();
    }

    this.setState({ roleId: entityDto.id });
    this.Modal();

    this.formRef.props.form.setFieldsValue({
      ...this.props.roleStore.roleEdit.role,
      grantedPermissions: this.props.roleStore.roleEdit.grantedPermissionNames,
    });
  }

  delete(input: EntityDto) {
    const self = this;
    confirm({
      title: 'Do you Want to delete these items?',
      async onOk() {
        await self.props.roleStore.delete(input);
        notification.open({
          message: 'Success',
          description: 'User Deleted Successfully.',
        });
      },
      onCancel() {},
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
        if (this.state.roleId === 0) {
          await this.props.roleStore.create(values);
          notification.open({
            message: 'Success',
            description: 'Role Created Successfully.',
          });
        } else {
          await this.props.roleStore.update({ id: this.state.roleId, ...values });
          notification.open({
            message: 'Success',
            description: 'Role Updated Successfully.',
          });
        }
      }

      await this.getAll();
      this.setState({ modalVisible: false });
      form.resetFields();
    });
  };

  saveFormRef = (formRef: any) => {
    this.formRef = formRef;
  };

  handleSearch = (value: string) => {
    this.setState({ filter: value }, async () => await this.getAll());
  };

  public render() {
    const { allPermissions, roles } = this.props.roleStore;
    const columns = [
      { title: L('RoleName'), dataIndex: 'name', key: 'name', width: 150, render: (text: string) => <div>{text}</div> },
      { title: L('DisplayName'), dataIndex: 'displayName', key: 'displayName', width: 150, render: (text: string) => <div>{text}</div> },
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
            <h1>{L('ROLES')}</h1>
          </Col>
          <Col>
            <CustomButton type="primary" icon="plus" onClick={() => this.createOrUpdateModalOpen({ id: 0 })}>
              Add Role
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
              rowKey="id"
              size={'default'}
              pagination={{ pageSize: this.state.maxResultCount, total: roles === undefined ? 0 : roles.totalCount, defaultCurrent: 1 }}
              columns={columns}
              loading={this.state.loading}
              dataSource={roles === undefined ? [] : roles.items}
              onChange={this.handleTableChange}
            />
          </Col>
        </Row>

        <CreateOrUpdateRole
          wrappedComponentRef={this.saveFormRef}
          visible={this.state.modalVisible}
          onCancel={() =>
            this.setState({
              modalVisible: false,
            })
          }
          modalType={this.state.roleId === 0 ? 'create' : 'edit'}
          onOk={this.handleCreate}
          permissions={allPermissions}
          roleStore={this.props.roleStore}
        />
      </Card>
    );
  }
}

export default Role;
