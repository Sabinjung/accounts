import React from 'react';
import { Form, notification } from 'antd';
import ActionButton from '../../../components/ActionButton';
import { FormComponentProps } from 'antd/lib/form';
import CustomCancleButton from './../../../components/Custom/CustomCancelButton';
import CustomInput from './../../../components/Custom/CustomInput';

type EndClientCreateUpdateProps = FormComponentProps<{}> & {
  onEndClientAddedOrUpdated?: (data: any) => void;
  endClient?: any;
  onClose?: any;
};

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

const EndClientCreateUpdate: React.FC<EndClientCreateUpdateProps> = ({ form, onEndClientAddedOrUpdated, endClient, onClose }) => {
  const { getFieldDecorator, validateFields } = form;
  let permission: string = endClient ? 'Endclient.Update' : 'Endclient.Create';

  return (
    <>
      <Form {...formItemLayout} hideRequiredMark name="End Client">
        <Form.Item label="Client Name">
          {getFieldDecorator('clientName', {
            rules: [{ required: true, message: 'Please Enter Client Name!' }],
          })(<CustomInput />)}
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
          permissions={[permission]}
          method={endClient && endClient.id ? 'Put' : 'Post'}
          style={{ height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
          url={`api/services/app/EndClient/${endClient && endClient.id ? 'Update' : 'Create'}`}
          onSuccess={(response: any) => {
            notification.open({
              message: 'Success',
              description: endClient && endClient.id ? 'Updated Successfully!' : 'Created Successfully!',
            });
            onEndClientAddedOrUpdated && onEndClientAddedOrUpdated(response.data.result);
          }}
          onError={(err: any) => {
            console.log(err);
          }}
          onSubmit={({ setFormData, setIsReady }: any) => {
            validateFields((errors, values: any) => {
              if (!errors) {
                const { clientName } = values;
                setFormData({
                  Id: endClient && endClient.id,
                  ClientName: clientName,
                });
                setIsReady(true);
              } else {
                setIsReady(false);
              }
            });
          }}
        >
          Save End Client
        </ActionButton>
      </div>
    </>
  );
};

const WrappedEndClientCreateUpdate = Form.create<EndClientCreateUpdateProps>({
  name: 'EndClientCreateUpdate_state',

  mapPropsToFields(props: any) {
    const { endClient } = props;
    if (!endClient) return;

    return {
      clientName: Form.createFormField({ value: endClient.clientName }),
    };
  },
})(EndClientCreateUpdate);

export default WrappedEndClientCreateUpdate;
