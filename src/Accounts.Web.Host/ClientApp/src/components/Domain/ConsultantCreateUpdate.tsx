import React from 'react';
import { Form, notification } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import _ from 'lodash';

import MaskedInput from 'antd-mask-input';
import ActionButton from '../ActionButton';
import CustomCancleButton from './../Custom/CustomCancelButton';
import CustomInput from './../Custom/CustomInput';

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
  consultant?: any;
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
          })(<CustomInput placeholder="FirstName" />)}
        </Form.Item>
        <Form.Item label="Last Name">
          {getFieldDecorator('lastName', {
            rules: [{ required: true, message: 'Please input last name!' }],
          })(<CustomInput placeholder="LastName" />)}
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
          })(<CustomInput />)}
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
          })(<MaskedInput style={{ boxShadow: ' 0px 3px 10px #0000000d' }} mask="(111) 111-1111" />)}
        </Form.Item>
      </Form>

      <div
        style={{
          position: 'absolute',
          left: 0,
          bottom: 0,
          width: '100%',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
        }}
      >
        <CustomCancleButton
          style={{ marginRight: 8 }}
          onClick={() => {
            onClose();
          }}
        >
          Cancel
        </CustomCancleButton>

        <ActionButton
          permissions={['Consultant.Create', 'Consultant.Update']}
          method={consultant && consultant.id ? 'Put' : 'Post'}
          style={{ height: '40px', boxShadow: '0px 3px 20px #2680EB66'}}
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
          Save Consultant
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
  },
})(ConsultantForm);

export default WrappedConsultantForm;
