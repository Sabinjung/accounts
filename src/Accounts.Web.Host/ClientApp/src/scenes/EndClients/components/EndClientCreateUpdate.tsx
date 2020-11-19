import React from 'react';
import { Input, Form, notification } from 'antd';
import ActionButton from '../../../components/ActionButton';
import { FormComponentProps } from 'antd/lib/form';
import styled from '@emotion/styled';

type EndClientCreateUpdateProps = FormComponentProps<{}> & {
  rowData: any;
  makeRequest: any;
  setVisible: any;
};

const StyledForm = styled(Form)`
  .ant-form-item-label > label {
    font-size: 18px;
  }
`;

const EndClientCreateUpdate: React.FC<EndClientCreateUpdateProps> = ({ form, rowData, makeRequest, setVisible }) => {
  const { getFieldDecorator, validateFields } = form;
  let permission: string = rowData ? 'Endclient.Update' : 'Endclient.Create';

  return (
    <>
      <StyledForm hideRequiredMark name="End Client">
        <Form.Item label="Client Name">
          {getFieldDecorator('clientName', {
            rules: [{ required: true, message: 'Please Enter Client Name!' }],
          })(<Input size={'large'} />)}
        </Form.Item>
      </StyledForm>
      <ActionButton
        permissions={[permission]}
        method={rowData ? 'Put' : 'Post'}
        url={`api/services/app/EndClient/${rowData ? 'Update' : 'Create'}`}
        style={{
          width: '100px',
          height: '38px',
        }}
        onSuccess={(response: any) => {
          notification.open({
            message: 'Success',
            description: rowData ? 'Updated Successfully!' : 'Created Successfully!',
          });
          setVisible(false);
          makeRequest({});
        }}
        onError={(err: any) => {
          notification.open({
            message: 'Error',
            description: err.message,
          });
        }}
        onSubmit={({ setFormData, setIsReady }: any) => {
          validateFields((errors, values: any) => {
            if (!errors) {
              const { clientName } = values;
              setFormData({
                Id: rowData.id,
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
    </>
  );
};

const WrappedEndClientCreateUpdate = Form.create<EndClientCreateUpdateProps>({
  name: 'EndClientCreateUpdate_state',

  mapPropsToFields(props: any) {
    const { rowData } = props;
    if (!rowData) return;
    return {
      clientName: Form.createFormField({ value: rowData.clientName }),
    };
  },
})(EndClientCreateUpdate);

export default WrappedEndClientCreateUpdate;
