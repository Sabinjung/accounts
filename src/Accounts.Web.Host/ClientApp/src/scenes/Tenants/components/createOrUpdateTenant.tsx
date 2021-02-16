import * as React from 'react';

import { Checkbox, Col, Form } from 'antd';

import { FormComponentProps } from 'antd/lib/form';
import FormItem from 'antd/lib/form/FormItem';
import { L } from '../../../lib/abpUtility';
import rules from './createOrUpdateTenant.validation';
import CustomModal from './../../../components/Custom/CustomModal';
import CustomInput from './../../../components/Custom/CustomInput';

import styled from '@emotion/styled';
import CustomCancleButton from '../../../components/Custom/CustomCancelButton';
import CustomButton from '../../../components/Custom/CustomButton';

const StyledCheckBox = styled(Checkbox)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;

export interface ICreateOrUpdateTenantProps extends FormComponentProps {
  visible: boolean;
  modalType: string;
  onCreate: () => void;
  onCancel: () => void;
}

class CreateOrUpdateTenant extends React.Component<ICreateOrUpdateTenantProps> {
  render() {
    const formItemLayout = {
      labelCol: {
        xs: { span: 10 },
        sm: { span: 10 },
        md: { span: 10 },
        lg: { span: 10 },
        xl: { span: 10 },
        xxl: { span: 10 },
      },
      wrapperCol: {
        xs: { span: 14 },
        sm: { span: 14 },
        md: { span: 14 },
        lg: { span: 14 },
        xl: { span: 14 },
        xxl: { span: 14 },
      },
    };

    const tailFormItemLayout = {
      labelCol: {
        xs: { span: 10 },
        sm: { span: 10 },
        md: { span: 10 },
        lg: { span: 10 },
        xl: { span: 10 },
        xxl: { span: 10 },
      },
      wrapperCol: {
        xs: { span: 14 },
        sm: { span: 14 },
        md: { span: 14 },
        lg: { span: 14 },
        xl: { span: 14 },
        xxl: { span: 14 },
      },
    };

    const { getFieldDecorator } = this.props.form;
    const { visible, onCancel, onCreate } = this.props;

    return (
      <CustomModal
        visible={visible}
        cancelText={<CustomCancleButton>Cancel</CustomCancleButton>}
        cancelButtonProps={{ style: { border: 'none', padding: '0', marginBottom: '20px' } }}
        okButtonProps={{ style: { padding: '0', border: 'none' } }}
        okText={<CustomButton type="primary">Save</CustomButton>}
        onCancel={onCancel}
        onOk={onCreate}
        title={this.props.modalType === 'create' ? 'Add New Tenant' : 'Edit Tenant'}
        width={550}
      >
        <Form>
          <FormItem label={L('TenancyName')} {...formItemLayout}>
            {this.props.form.getFieldDecorator('tenancyName', { rules: rules.tenancyName })(<CustomInput />)}
          </FormItem>
          <FormItem label={L('Name')} {...formItemLayout}>
            {getFieldDecorator('name', { rules: rules.name })(<CustomInput />)}
          </FormItem>
          {this.props.modalType === 'create' ? (
            <FormItem label={L('AdminEmailAddress')} {...formItemLayout}>
              {getFieldDecorator('adminEmailAddress', { rules: rules.adminEmailAddress })(<CustomInput />)}
            </FormItem>
          ) : null}
          {this.props.modalType === 'create' ? (
            <FormItem label={L('DatabaseConnectionString')} {...formItemLayout}>
              {getFieldDecorator('connectionString')(<CustomInput />)}
            </FormItem>
          ) : null}
          <FormItem label={L('IsActive')} {...tailFormItemLayout}>
            {getFieldDecorator('isActive', { valuePropName: 'checked' })(<StyledCheckBox />)}
          </FormItem>
          <Col>{L('Default password is  123qwe')}</Col>
        </Form>
      </CustomModal>
    );
  }
}

export default Form.create<ICreateOrUpdateTenantProps>()(CreateOrUpdateTenant);
