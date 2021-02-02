import React from 'react';
import { Input, Form, notification, Button } from 'antd';
import ActionButton from '../../../components/ActionButton';
import { FormComponentProps } from 'antd/lib/form';
import styled from '@emotion/styled';

type EndClientCreateUpdateProps = FormComponentProps<{}> & {
  onEndClientAddedOrUpdated?: (data: any) => void;
  endClient?: any;
  onClose?: any;
};

const StyledForm = styled(Form)`
  .ant-form-item-label > label {
    font-size: 18px;
  }
`;

const EndClientCreateUpdate: React.FC<EndClientCreateUpdateProps> = ({ form, onEndClientAddedOrUpdated, endClient, onClose }) => {
  const { getFieldDecorator, validateFields } = form;
  let permission: string = endClient ? 'Endclient.Update' : 'Endclient.Create';

  return (
    <>
      <StyledForm hideRequiredMark name="End Client">
        <Form.Item label="Client Name">
          {getFieldDecorator('clientName', {
            rules: [{ required: true, message: 'Please Enter Client Name!' }],
          })(<Input size={'large'} allowClear />)}
        </Form.Item>
      </StyledForm>
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
        <Button
          style={{ marginRight: 8 }}
          onClick={() => {
            onClose();
          }}
        >
          Cancel
        </Button>
        <ActionButton
          permissions={[permission]}
          method={endClient && endClient.id ? 'Put' : 'Post'}
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
          Submit
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
