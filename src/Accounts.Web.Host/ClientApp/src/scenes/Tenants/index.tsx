import * as React from 'react';

import { Card, Col, Modal, Row } from 'antd';
import { inject, observer } from 'mobx-react';

import AppComponentBase from '../../components/AppComponentBase';
import CreateOrUpdateTenant from './components/createOrUpdateTenant';
import { EntityDto } from '../../services/dto/entityDto';
import { L } from '../../lib/abpUtility';
import Stores from '../../stores/storeIdentifier';
import TenantStore from '../../stores/tenantStore';
import CustomSearch from '../../components/Custom/CustomSearch';
import CustomTable from './../../components/Custom/CustomTable';
import CustomButton from '../../components/Custom/CustomButton';
import styled from '@emotion/styled';
import CustomEditButton from './../../components/Custom/CustomEditButton';
import CustomDeleteButton from '../../components/Custom/CustomDeleteButton';

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

export interface ITenantProps {
  tenantStore: TenantStore;
}

export interface ITenantState {
  modalVisible: boolean;
  maxResultCount: number;
  skipCount: number;
  tenantId: number;
  filter: string;
}

const confirm = Modal.confirm;

@inject(Stores.TenantStore)
@observer
class Tenant extends AppComponentBase<ITenantProps, ITenantState> {
  formRef: any;

  state = {
    modalVisible: false,
    maxResultCount: 10,
    skipCount: 0,
    tenantId: 0,
    filter: '',
  };

  async componentDidMount() {
    await this.getAll();
  }

  async getAll() {
    await this.props.tenantStore.getAll({ maxResultCount: this.state.maxResultCount, skipCount: this.state.skipCount, keyword: this.state.filter });
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
      this.props.tenantStore.createTenant();
    } else {
      await this.props.tenantStore.get(entityDto);
    }

    this.setState({ tenantId: entityDto.id });
    this.Modal();

    if (entityDto.id !== 0) {
      this.formRef.props.form.setFieldsValue({
        ...this.props.tenantStore.tenantModel,
      });
    } else {
      this.formRef.props.form.resetFields();
    }
  }

  delete(input: EntityDto) {
    const self = this;
    confirm({
      title: 'Do you Want to delete these items?',
      onOk() {
        self.props.tenantStore.delete(input);
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
        if (this.state.tenantId === 0) {
          await this.props.tenantStore.create(values);
        } else {
          await this.props.tenantStore.update({ id: this.state.tenantId, ...values });
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
    const { tenants } = this.props.tenantStore;
    const columns = [
      { title: L('TenancyName'), dataIndex: 'tenancyName', key: 'tenancyName', width: 150, render: (text: string) => <div>{text}</div> },
      { title: L('Name'), dataIndex: 'name', key: 'name', width: 150, render: (text: string) => <div>{text}</div> },
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
            <h1>{L('TENANTS')}</h1>
          </Col>
          <Col>
            <CustomButton type="primary" icon="plus" onClick={() => this.createOrUpdateModalOpen({ id: 0 })}>
              Add Tenant
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
              pagination={{ pageSize: this.state.maxResultCount, total: tenants === undefined ? 0 : tenants.totalCount, defaultCurrent: 1 }}
              columns={columns}
              loading={tenants === undefined ? true : false}
              dataSource={tenants === undefined ? [] : tenants.items}
              onChange={this.handleTableChange}
            />
          </Col>
        </Row>
        <CreateOrUpdateTenant
          wrappedComponentRef={this.saveFormRef}
          visible={this.state.modalVisible}
          onCancel={() =>
            this.setState({
              modalVisible: false,
            })
          }
          modalType={this.state.tenantId === 0 ? 'create' : 'edit'}
          onCreate={this.handleCreate}
        />
      </Card>
    );
  }
}

export default Tenant;
