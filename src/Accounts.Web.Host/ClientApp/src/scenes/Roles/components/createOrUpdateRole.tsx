import * as React from 'react';

import { Form, Tabs } from 'antd';

import CheckboxGroup from 'antd/lib/checkbox/Group';
import { FormComponentProps } from 'antd/lib/form';
import FormItem from 'antd/lib/form/FormItem';
import { GetAllPermissionsOutput } from '../../../services/role/dto/getAllPermissionsOutput';
import { L } from '../../../lib/abpUtility';
import RoleStore from '../../../stores/roleStore';
import rules from './createOrUpdateRole.validation';
import CustomModal from '../../../components/Custom/CustomModal';
import CustomInput from './../../../components/Custom/CustomInput';
import styled from '@emotion/styled';
import CustomCancleButton from '../../../components/Custom/CustomCancelButton';
import CustomButton from '../../../components/Custom/CustomButton';
const StyledCheckBoxGroup = styled(CheckboxGroup)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;

const TabPane = Tabs.TabPane;

export interface ICreateOrUpdateRoleProps extends FormComponentProps {
  roleStore: RoleStore;
  visible: boolean;
  onCancel: () => void;
  modalType: string;
  onOk: () => void;
  permissions: GetAllPermissionsOutput[];
}

class CreateOrUpdateRole extends React.Component<ICreateOrUpdateRoleProps> {
  state = {
    confirmDirty: false,
  };

  render() {
    const { permissions } = this.props;

    const options = permissions.map((x: GetAllPermissionsOutput) => {
      return { label: x.displayName, value: x.name };
    });

    const formItemLayout = {
      labelCol: {
        xs: { span: 6 },
        sm: { span: 6 },
        md: { span: 6 },
        lg: { span: 6 },
        xl: { span: 6 },
        xxl: { span: 6 },
      },
      wrapperCol: {
        xs: { span: 18 },
        sm: { span: 18 },
        md: { span: 18 },
        lg: { span: 18 },
        xl: { span: 18 },
        xxl: { span: 18 },
      },
    };

    const tailFormItemLayout = {
      labelCol: {
        xs: { span: 6 },
        sm: { span: 6 },
        md: { span: 6 },
        lg: { span: 6 },
        xl: { span: 6 },
        xxl: { span: 6 },
      },
      wrapperCol: {
        xs: { span: 18 },
        sm: { span: 18 },
        md: { span: 18 },
        lg: { span: 18 },
        xl: { span: 18 },
        xxl: { span: 18 },
      },
    };

    const { getFieldDecorator } = this.props.form;

    return (
      <CustomModal
        visible={this.props.visible}
        cancelText={<CustomCancleButton>Cancel</CustomCancleButton>}
        cancelButtonProps={{ style: { border: 'none', padding: '0', marginBottom: '20px' } }}
        okButtonProps={{ style: { padding: '0', border: 'none' } }}
        okText={<CustomButton type="primary">Save Role</CustomButton>}
        onCancel={this.props.onCancel}
        title={this.props.modalType === 'create' ? 'Add New Role' : 'Edit Role'}
        onOk={this.props.onOk}
      >
        <Tabs defaultActiveKey={'role'} size={'small'} tabBarGutter={64}>
          <TabPane tab={L('Role Details')} key={'role'}>
            <FormItem label={L('RoleName')} {...formItemLayout}>
              {getFieldDecorator('name', { rules: rules.name })(<CustomInput />)}
            </FormItem>
            <FormItem label={L('DisplayName')} {...formItemLayout}>
              {getFieldDecorator('displayName', { rules: rules.displayName })(<CustomInput />)}
            </FormItem>
            <FormItem label={L('Description')} {...formItemLayout}>
              {getFieldDecorator('description')(<CustomInput />)}
            </FormItem>
          </TabPane>
          <TabPane tab={L('Role Permission')} key={'permission'}>
            <FormItem {...tailFormItemLayout}>
              {getFieldDecorator('grantedPermissions', { valuePropName: 'value' })(<StyledCheckBoxGroup options={options} />)}
            </FormItem>
          </TabPane>
        </Tabs>
      </CustomModal>
    );
  }
}

export default Form.create<ICreateOrUpdateRoleProps>()(CreateOrUpdateRole);
