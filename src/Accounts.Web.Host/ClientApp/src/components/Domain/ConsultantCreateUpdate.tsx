import React from 'react';
import { Form, Input, Button, notification } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import _ from 'lodash';

import MaskedInput from 'antd-mask-input';
import ActionButton from '../ActionButton';

const formItemLayout = {
  labelCol: {
    xs: { span: 24 },
    sm: { span: 8 },
  },
  wrapperCol: {
    xs: { span: 24 },
    sm: { span: 16 },
  },
};

export type IConsultantFormProps = FormComponentProps<{}> & {
  onConsultantAddedOrUpdated?: (data: any) => void;
  consultant?:any
  onClose?: any;
};

const ConsultantForm: React.FC<IConsultantFormProps> = ({ form, onConsultantAddedOrUpdated, consultant, onClose }) => {
  const { getFieldDecorator, validateFields } = form;

  return (
    <React.Fragment>
      <Form {...formItemLayout}>
        <Form.Item label="First Name">
          {getFieldDecorator('firstName', {
            rules: [{ required: true, message: 'Please input first name!' }],
          })(<Input placeholder="FirstName" />)}
        </Form.Item>
        <Form.Item label="Last Name">
          {getFieldDecorator('lastName', {
            rules: [{ required: true, message: 'Please input last name!' }],
          })(<Input placeholder="LastName" />)}
        </Form.Item>
        <Form.Item label="E-mail">
          {getFieldDecorator('email', {
            rules: [
              {
                type: 'email',
                message: 'The input is not valid E-mail!',
              },
              {
                required: true,
                message: 'Please input your E-mail!',
              },
            ],
          })(<Input />)}
        </Form.Item>
        <Form.Item label="Phone Number">
          {getFieldDecorator('phoneNumber', {
            rules: [
              { required: true, message: 'Please input your phone number!' },
              {
                pattern: /((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}/,
                message: 'Please enter valid phone number',
              },
            ],
          })(<MaskedInput mask="(111) 111-1111" />)}
        </Form.Item>
      </Form>

      <div
        style={{
          position: 'absolute',
          left: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
        }}
      >
        <Button style={{ marginRight: 8 }} onClick={() => { onClose() }}>Cancel</Button>

        <ActionButton
          permissions={['Consultant.Create', 'Consultant.Update']}
          method={consultant && consultant.id ? 'Put' : 'Post'}
          url={`/api/services/app/Consultant/${consultant && consultant.id ? 'Update' : 'Create'}`}
          onSuccess={(response: any) => {
            notification.open({
              message: 'Success',
              description: 'Operation successful!',
            });
            onConsultantAddedOrUpdated && onConsultantAddedOrUpdated(response.data.result);
          }}
          onSubmit={({ setFormData, setIsReady }: any) => {
            validateFields((errors, values: any) => {
              if (!errors) {
                const { firstName, lastName, email, phoneNumber, ...rest } = values;
                setFormData({
                  id: consultant && consultant.id,
                  firstName: firstName,
                  lastName: lastName,
                  email: email,
                  phoneNumber: phoneNumber,
                  ...rest,
                });
                setIsReady(true);
              } else {
                setIsReady(false);
              }
            });
          }}
        >
          Save
        </ActionButton>
      </div>
    </React.Fragment>
  );
};

const WrappedConsultantForm = Form.create<IConsultantFormProps>({
  name: 'consultant_state',

mapPropsToFields(props: any) {
  const { consultant } = props;
  if (!consultant) return;

  return {
    firstName: Form.createFormField({ value: consultant.firstName }),
    lastName: Form.createFormField({ value: consultant.lastName }),
    email: Form.createFormField({ value: consultant.email }),
    phoneNumber: Form.createFormField({ value: consultant.phoneNumber }),
  };
}
})(ConsultantForm);


export default WrappedConsultantForm;
