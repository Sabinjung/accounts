import * as React from 'react';

import { Checkbox, Form, Tabs } from 'antd';

import CheckboxGroup from 'antd/lib/checkbox/Group';
import { FormComponentProps } from 'antd/lib/form';
import FormItem from 'antd/lib/form/FormItem';
import { GetRoles } from '../../../services/user/dto/getRolesOuput';
import { L } from '../../../lib/abpUtility';
import rules from './createOrUpdateUser.validation';
import CustomModal from './../../../components/Custom/CustomModal';
import CustomInput from '../../../components/Custom/CustomInput';
import styled from '@emotion/styled';
import CustomCancleButton from './../../../components/Custom/CustomCancelButton';
import CustomButton from '../../../components/Custom/CustomButton';

const StyledCheckBox = styled(Checkbox)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;

const StyledCheckBoxGroup = styled(CheckboxGroup)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;

const TabPane = Tabs.TabPane;

export interface ICreateOrUpdateUserProps extends FormComponentProps {
  visible: boolean;
  onCancel: () => void;
  modalType: string;
  onCreate: () => void;
  roles: GetRoles[];
}

class CreateOrUpdateUser extends React.Component<ICreateOrUpdateUserProps> {
  state = {
    confirmDirty: false,
  };

  compareToFirstPassword = (rule: any, value: any, callback: any) => {
    const form = this.props.form;
    if (value && value !== form.getFieldValue('password')) {
      callback('Two passwords that you enter is inconsistent!');
    } else {
      callback();
    }
  };

  validateToNextPassword = (rule: any, value: any, callback: any) => {
    const form = this.props.form;
    if (value && this.state.confirmDirty) {
      form.validateFields(['confirm'], { force: true });
    }
    callback();
  };

  render() {
    const { roles } = this.props;

    const formItemLayout = {
      labelCol: {
        xs: { span: 8 },
        sm: { span: 8 },
        md: { span: 8 },
        lg: { span: 8 },
        xl: { span: 8 },
        xxl: { span: 8 },
      },
      wrapperCol: {
        xs: { span: 16 },
        sm: { span: 16 },
        md: { span: 16 },
        lg: { span: 16 },
        xl: { span: 16 },
        xxl: { span: 16 },
      },
    };
    const tailFormItemLayout = {
      labelCol: {
        xs: { span: 8 },
        sm: { span: 8 },
        md: { span: 8 },
        lg: { span: 8 },
        xl: { span: 8 },
        xxl: { span: 8 },
      },
      wrapperCol: {
        xs: { span: 16 },
        sm: { span: 16 },
        md: { span: 16 },
        lg: { span: 16 },
        xl: { span: 16 },
        xxl: { span: 16 },
      },
    };

    const { getFieldDecorator } = this.props.form;
    const { visible, onCancel, onCreate } = this.props;

    const options = roles.map((x: GetRoles) => {
      var test = { label: x.displayName, value: x.normalizedName };
      return test;
    });

    return (
      <CustomModal
        visible={visible}
        cancelText={<CustomCancleButton>Cancel</CustomCancleButton>}
        cancelButtonProps={{ style: { border: 'none', padding: '0', marginBottom: '20px' } }}
        okButtonProps={{ style: { padding: '0', border: 'none' } }}
        okText={<CustomButton type="primary">Save User</CustomButton>}
        onCancel={onCancel}
        onOk={onCreate}
        title={this.props.modalType === 'edit' ? 'Edit User' : 'Add New User'}
      >
        <Tabs defaultActiveKey={'userInfo'} size={'small'} tabBarGutter={64}>
          <TabPane tab={'User'} key={'user'}>
            <FormItem label={L('Name')} {...formItemLayout}>
              {getFieldDecorator('name', { rules: rules.name })(<CustomInput />)}
            </FormItem>
            <FormItem label={L('Surname')} {...formItemLayout}>
              {getFieldDecorator('surname', { rules: rules.surname })(<CustomInput />)}
            </FormItem>
            <FormItem label={L('UserName')} {...formItemLayout}>
              {getFieldDecorator('userName', { rules: rules.userName })(<CustomInput />)}
            </FormItem>
            <FormItem label={L('Email')} {...formItemLayout}>
              {getFieldDecorator('emailAddress', { rules: rules.emailAddress })(<CustomInput />)}
            </FormItem>
            {this.props.modalType === 'create' ? (
              <FormItem label={L('Password')} {...formItemLayout}>
                {getFieldDecorator('password', {
                  rules: [
                    {
                      required: true,
                      message: 'Please input your password!',
                    },
                    {
                      validator: this.validateToNextPassword,
                    },
                  ],
                })(<CustomInput type="password" />)}
              </FormItem>
            ) : null}
            {this.props.modalType === 'create' ? (
              <FormItem label={L('ConfirmPassword')} {...formItemLayout}>
                {getFieldDecorator('confirm', {
                  rules: [
                    {
                      required: true,
                      message: L('ConfirmPassword'),
                    },
                    {
                      validator: this.compareToFirstPassword,
                    },
                  ],
                })(<CustomInput type="password" />)}
              </FormItem>
            ) : null}
            <FormItem label={L('IsActive')} {...tailFormItemLayout}>
              {getFieldDecorator('isActive', { valuePropName: 'checked' })(<StyledCheckBox>Aktif</StyledCheckBox>)}
            </FormItem>
          </TabPane>
          <TabPane tab={L('Roles')} key={'rol'}>
            <FormItem {...tailFormItemLayout}>
              {getFieldDecorator('roleNames', { valuePropName: 'value' })(<StyledCheckBoxGroup options={options} />)}
            </FormItem>
          </TabPane>
        </Tabs>
      </CustomModal>
    );
  }
}

export default Form.create<ICreateOrUpdateUserProps>()(CreateOrUpdateUser);
