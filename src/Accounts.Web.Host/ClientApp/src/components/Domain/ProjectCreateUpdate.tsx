import React from 'react';
import { Form, Button, DatePicker, Input, notification, Checkbox } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import EntityPicker from '../EntityPicker';
import ConnectedEntityPicker from '../ConnectedEntityPicker';
import ActionButton from '../ActionButton';
import moment from 'moment';
import _ from 'lodash';
import DiscountInput from './DiscountInput';

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

export type IProjectFormProps = FormComponentProps<{}> & {
  onProjectAdded?: () => void;
  project?: any;
  onClose?: any;
};

const ProjectForm: React.FC<IProjectFormProps> = ({ form, onProjectAdded, project, onClose }) => {
  const { getFieldDecorator, validateFields } = form;

  const validateDiscount = (rule: any, value: any, callback: any) => {
    if (value.discountType && !value.discountValue) {
      callback('Please input the discount!');
    } else {
      callback();
    }
  };

  return (
    <React.Fragment>
      <Form {...formItemLayout}>
        <Form.Item label="Company">
          {getFieldDecorator('companyId', {
            rules: [{ required: true, message: 'Please input your Company!' }],
          })(<EntityPicker url="/api/services/app/Company/Search" mapFun={(r) => ({ value: r.id, text: r.displayName })} />)}
        </Form.Item>
        <Form.Item label="Consultant">
          {getFieldDecorator('consultantId', {
            rules: [{ required: true, message: 'Please input your Consultant!' }],
          })(
            <ConnectedEntityPicker
              loader={(store: any) => store.getConsultants()}
              selector={(store: any) => store.consultants}
              mapFunc={(r: any) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
              addUrl={'/projects/new/consultants/new'}
            />
          )}
        </Form.Item>
        <Form.Item label="End Client">
          {getFieldDecorator('endClientId')(
            <EntityPicker url="/api/services/app/EndClient/Search" mapFun={(r) => ({ value: r.id, text: r.clientName })} />
          )}
        </Form.Item>
        <Form.Item label="Term">
          {getFieldDecorator('termId', {
            rules: [{ required: true, message: 'Select correct Term' }],
          })(<EntityPicker url="/api/services/app/Term/GetAll" mapFun={(r) => ({ value: r.id, text: r.name })} />)}
        </Form.Item>
        <Form.Item label="Invoice Cycle">
          {getFieldDecorator('invoiceCycleId', {
            rules: [{ required: true, message: 'Select right invoice cycle!' }],
          })(<EntityPicker url="/api/services/app/InvoiceCycle/GetAll" mapFun={(r) => ({ value: r.id, text: r.name })} />)}
        </Form.Item>
        <Form.Item label="Start Date">
          {getFieldDecorator('startDt', {
            rules: [{ required: true, message: 'Please input Start Date!' }],
          })(<DatePicker />)}
        </Form.Item>
        <Form.Item label="End Date">{getFieldDecorator('endDt')(<DatePicker />)}</Form.Item>
        <Form.Item label="InvoiceCycle Start Date">
          {getFieldDecorator('invoiceCycleStartDt', {
            rules: [{ required: true, message: 'Please input InvoiceCycle Start Date!' }],
          })(<DatePicker />)}
        </Form.Item>
        <Form.Item label="Discount">
          {getFieldDecorator('discount', {
            rules: [{ validator: validateDiscount }],
          })(<DiscountInput />)}
        </Form.Item>
        <Form.Item label="Rate">
          {getFieldDecorator('rate', {
            rules: [{ required: true, message: 'Please input rate!' }],
          })(<Input style={{ width: '5em' }} />)}
        </Form.Item>
        <Form.Item label="Send Mail">{getFieldDecorator('isSendMail', { valuePropName: 'checked' })(<Checkbox></Checkbox>)}</Form.Item>
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
        <Button
          style={{ marginRight: 8 }}
          onClick={() => {
            onClose();
          }}
        >
          Cancel
        </Button>

        <ActionButton
          permissions={['Project.Create', 'Project.Update']}
          method={project && project.id ? 'Put' : 'Post'}
          url={`/api/services/app/Project/${project && project.id ? 'Update' : 'Create'}`}
          onSuccess={(response: any) => {
            notification.open({
              message: 'Success',
              description: project && project.id ? 'Project successfully updated.' : 'Project successfully created.',
            });
            onProjectAdded && onProjectAdded();
          }}
          onSubmit={({ setFormData, setIsReady }: any) => {
            validateFields((errors, values: any) => {
              if (!errors) {
                const { discount, invoiceCycleStartDt, startDt, endDt, ...rest } = values;
                setFormData({
                  id: project && project.id,
                  ...discount,
                  invoiceCycleStartDt: invoiceCycleStartDt.format('MM/DD/YYYY'),
                  startDt: startDt.format('MM/DD/YYYY'),
                  endDt: endDt && endDt.format('MM/DD/YYYY'),
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

const WrappedProjectForm = Form.create<IProjectFormProps>({
  name: 'project_state',

  mapPropsToFields(props: any) {
    const { project } = props;
    if (!project) return;

    // var map = {};
    // _.keys(project).forEach(key => {
    //   map[key] = Form.createFormField({ value: project[key] });
    // });
    return {
      companyId: Form.createFormField({ value: project.companyId }),
      termId: Form.createFormField({ value: project.termId }),
      invoiceCycleId: Form.createFormField({ value: project.invoiceCycleId }),
      startDt: Form.createFormField({ value: moment(project.startDt) }),
      endDt: Form.createFormField({ value: project.endDt && moment(project.endDt) }),
      consultantId: Form.createFormField({ value: project.consultantId }),
      rate: Form.createFormField({ value: project.rate }),
      isSendMail: Form.createFormField({ value: project.isSendMail }),
      endClientId: Form.createFormField({ value: project.endClientId }),
      discount: Form.createFormField({
        value: {
          discountType: project.discountType,
          discountValue: project.discountValue,
        },
      }),
      invoiceCycleStartDt: Form.createFormField({ value: project.invoiceCycleStartDt && moment(project.invoiceCycleStartDt) }),
    };
  },
  onFieldsChange(props, fields) {
    const { project } = props;
    project.companyId = _.get(fields, 'companyId.value');
    project.termId = _.get(fields, 'termId.value');
    project.invoiceCycleId = _.get(fields, 'invoiceCycleId.value');
    project.startDt = _.get(fields, 'startDt.value');
    project.endDt = _.get(fields, 'endDt.value');
    project.consultantId = _.get(fields, 'consultantId.value');
    project.rate = _.get(fields, 'rate.value');
    project.isSendMail = _.get(fields, 'isSendMail.value');
    project.discountType = _.get(fields, 'discount.value.discountType');
    project.discountValue = _.get(fields, 'discount.value.discountValue');
    project.invoiceCycleStartDt = _.get(fields, 'invoiceCycleStartDt.value');
    project.endClientId = _.get(fields, 'endClientId');
  },
})(ProjectForm);

export default WrappedProjectForm;
