import React, { useState, useEffect } from 'react';
import { Form, Row, Col, Input, Checkbox, notification } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import CustomInput from './../../../components/Custom/CustomInput';
import CustomCancleButton from './../../../components/Custom/CustomCancelButton';
import ActionButton from '../../../components/ActionButton';
import styled from '@emotion/styled';
import EntityPicker from '../../../components/EntityPicker';

const { TextArea } = Input;

const StyledTextArea = styled(TextArea)`
  box-shadow: 0px 3px 10px #0000000d;
`;

const StyledCol = styled(Col)`
  color: rgba(0, 0, 0, 0.85);
`;
const StyledCheckbox = styled(Checkbox)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;

const StyledForm = styled(Form)`
  .ant-form-item {
    margin-bottom: 21px;
  }
  .ant-form-item-with-help {
    margin-bottom: 2px;
  }
`;

export type ICompanyFormProps = FormComponentProps<{}> & {
  onCompanyAddedOrUpdated?: () => void;
  onClose?: any;
  company?: any;
  isEdited?: boolean;
};

const CompanyForm: React.FC<ICompanyFormProps> = ({ form, onCompanyAddedOrUpdated, company, onClose, isEdited }) => {
  const [isSameAddress, setIsSameAddress] = useState(true);
  const { getFieldDecorator, validateFields } = form;

  useEffect(() => {
    let IsAddressSame =
      company && (company.shippingState || company.shippingStreet || company.shippingZipCode || company.shippingCity || company.shippingCountry)
        ? false
        : true;
    setIsSameAddress(IsAddressSame);
  }, [company]);

  return (
    <React.Fragment>
      <StyledForm>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item label="Company Name">
              {getFieldDecorator('companyName', {
                rules: [{ required: true, message: 'Please input company name!' }],
              })(<CustomInput placeholder="Company Name" />)}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="E-mail">
              {getFieldDecorator('email', {
                rules: [
                  {
                    required: true,
                    message: 'Please input your E-mail!',
                  },
                  {
                    pattern: /^(\s?[^\s,]+@[^\s,]+\.[^\s,]+\s?,)*(\s?[^\s,]+@[^\s,]+\.[^\s,]+)$/,
                    message: 'Enter valid emails separated with commas!',
                  },
                ],
              })(<CustomInput placeholder="E-mail" />)}
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={16}>
          <Col span={8}>
            <Form.Item label="First Name">{getFieldDecorator('firstName')(<CustomInput placeholder="First Name" />)}</Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="Middle Name">{getFieldDecorator('middleName')(<CustomInput placeholder="Middle Name" />)}</Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="Last Name">{getFieldDecorator('lastName')(<CustomInput placeholder="Last Name" />)}</Form.Item>
          </Col>
        </Row>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item label="Phone Number">{getFieldDecorator('phoneNumber')(<CustomInput placeholder="Phone Number" />)}</Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="Mobile Number">{getFieldDecorator('mobileNumber')(<CustomInput placeholder="Mobile Number" />)}</Form.Item>
          </Col>
        </Row>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item label="Payment Methods">
              {getFieldDecorator('paymentMethod', {
                rules: [{ required: true, message: 'Please select payment method!' }],
              })(
                <EntityPicker
                  url="/api/services/app/PaymentMethod/GetAll?MaxResultCount=25"
                  mapFun={(r) => ({ value: r.externalPaymentId, text: r.name })}
                />
              )}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="Terms">
              {getFieldDecorator('terms', {
                rules: [{ required: true, message: 'Please select term!' }],
              })(<EntityPicker url="/api/services/app/Term/GetAll?MaxResultCount=25" mapFun={(r) => ({ value: r.externalTermId, text: r.name })} />)}
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="Billing Address">
          <Row gutter={[16, 4]}>
            <Col span={16}>{getFieldDecorator('billingStreet')(<CustomInput placeholder="Street" />)}</Col>
            <Col span={8}>{getFieldDecorator('billingCity')(<CustomInput placeholder="City/Town" />)}</Col>
            <Col span={8}>{getFieldDecorator('billingState')(<CustomInput placeholder="State/Province" />)}</Col>
            <Col span={8}>{getFieldDecorator('billingZipCode')(<CustomInput placeholder="ZIP Code" />)}</Col>
            <Col span={8}>{getFieldDecorator('billingCountry')(<CustomInput placeholder="Country" />)}</Col>
          </Row>
        </Form.Item>
        <Form.Item>
          <Row type="flex" align="middle" gutter={16}>
            <StyledCol>Shipping Address:</StyledCol>
            <StyledCol>
              <StyledCheckbox checked={isSameAddress} onChange={(e) => setIsSameAddress(e.target.checked)}>
                Same as billing address
              </StyledCheckbox>
            </StyledCol>
          </Row>
          {!isSameAddress && (
            <Row gutter={[16, 4]}>
              <Col span={16}>{getFieldDecorator('shippingStreet')(<CustomInput placeholder="Street" />)}</Col>
              <Col span={8}>{getFieldDecorator('shippingCity')(<CustomInput placeholder="City/Town" />)}</Col>
              <Col span={8}>{getFieldDecorator('shippingState')(<CustomInput placeholder="State/Province" />)}</Col>
              <Col span={8}>{getFieldDecorator('shippingZipCode')(<CustomInput placeholder="ZIP Code" />)}</Col>
              <Col span={8}>{getFieldDecorator('shippingCountry')(<CustomInput placeholder="Country" />)}</Col>
            </Row>
          )}
        </Form.Item>
        <Form.Item label="Notes">
          {getFieldDecorator('notes')(<StyledTextArea maxLength={4000} allowClear rows={2} placeholder="Comment..." />)}
        </Form.Item>
      </StyledForm>

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
          permissions={[isEdited ? 'Company.Edit' : 'Company.Create']}
          method="Post"
          style={{ height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
          url={`/api/services/app/Intuit/${isEdited ? 'EditCompany' : 'CreateCompany'}`}
          onSuccess={(response: any) => {
            notification.open({
              message: 'Success',
              description: isEdited ? 'Updated Successfully' : 'Created Successfully',
            });
            onCompanyAddedOrUpdated && onCompanyAddedOrUpdated();
          }}
          onSubmit={({ setFormData, setIsReady }: any) => {
            validateFields((errors, values: any) => {
              if (!errors) {
                const {
                  billingCountry,
                  billingCity,
                  billingZipCode,
                  billingStreet,
                  billingState,
                  shippingCountry,
                  shippingCity,
                  shippingZipCode,
                  shippingStreet,
                  shippingState,
                  ...rest
                } = values;
                if (isSameAddress) {
                  setFormData({
                    externalCustomerId: company && company.externalCustomerId,
                    shippingCountry: billingCountry,
                    shippingCity: billingCity,
                    shippingZipCode: billingZipCode,
                    shippingStreet: billingStreet,
                    shippingState: billingState,
                    billingCountry,
                    billingCity,
                    billingZipCode,
                    billingStreet,
                    billingState,
                    ...rest,
                  });
                } else {
                  setFormData({
                    externalCustomerId: company && company.externalCustomerId,
                    billingCountry,
                    billingCity,
                    billingZipCode,
                    billingStreet,
                    billingState,
                    shippingCountry,
                    shippingCity,
                    shippingZipCode,
                    shippingStreet,
                    shippingState,
                    ...rest,
                  });
                }
                setIsReady(true);
              } else {
                setIsReady(false);
              }
            });
          }}
        >
          Save Company
        </ActionButton>
      </div>
    </React.Fragment>
  );
};

const WrappedCompanyForm = Form.create<ICompanyFormProps>({
  name: 'company_state',

  mapPropsToFields(props: any) {
    const { company } = props;
    if (!company) return;

    return {
      companyName: Form.createFormField({ value: company.companyName }),
      firstName: Form.createFormField({ value: company.firstName }),
      middleName: Form.createFormField({ value: company.middleName }),
      lastName: Form.createFormField({ value: company.lastName }),
      email: Form.createFormField({ value: company.email }),
      phoneNumber: Form.createFormField({ value: company.phoneNumber }),
      mobileNumber: Form.createFormField({ value: company.mobileNumber }),
      billingCountry: Form.createFormField({ value: company.billingCountry }),
      billingCity: Form.createFormField({ value: company.billingCity }),
      billingZipCode: Form.createFormField({ value: company.billingZipCode }),
      billingStreet: Form.createFormField({ value: company.billingStreet }),
      billingState: Form.createFormField({ value: company.billingState }),
      shippingCountry: Form.createFormField({ value: company.shippingCountry }),
      shippingCity: Form.createFormField({ value: company.shippingCity }),
      shippingZipCode: Form.createFormField({ value: company.shippingZipCode }),
      shippingStreet: Form.createFormField({ value: company.shippingStreet }),
      shippingState: Form.createFormField({ value: company.shippingState }),
      paymentMethod: Form.createFormField({ value: company.paymentMethod }),
      terms: Form.createFormField({ value: company.terms }),
      notes: Form.createFormField({ value: company.notes }),
    };
  },
})(CompanyForm);

export default WrappedCompanyForm;
