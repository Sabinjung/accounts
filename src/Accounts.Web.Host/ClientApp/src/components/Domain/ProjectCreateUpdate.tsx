import React from 'react';
import { Form, DatePicker, Select, notification, Checkbox } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import EntityPicker from '../EntityPicker';
import ConnectedEntityPicker from '../ConnectedEntityPicker';
import ActionButton from '../ActionButton';
import moment from 'moment';
import _ from 'lodash';
import DiscountInput from './DiscountInput';
import styled from '@emotion/styled';
import CustomCancleButton from '../Custom/CustomCancelButton';
import CustomInput from './../Custom/CustomInput';

const {Option} = Select;

const formItemLayout = {
  labelCol: {
    xs: { span: 9 },
    sm: { span: 9 },
    md: { span: 9 },
    lg: { span: 9 },
    xl: { span: 9 },
    xxl: { span: 9 },
  },
  wrapperCol: {
    xs: { span: 15 },
    sm: { span: 15 },
    md: { span: 15 },
    lg: { span: 15 },
    xl: { span: 15 },
    xxl: { span: 15 },
  },
};

const StyledForm = styled(Form)`
  .ant-form-item {
    display: flex;
    align-items: center;
  }
`;

const StyledCheckbox = styled(Checkbox)`
  .ant-checkbox-inner {
    width: 20px;
    height: 20px;
    box-shadow: 0px 3px 10px #0000000d;
  }
`;
const StyledDatePicker = styled(DatePicker)`
  box-shadow: 0px 3px 10px #0000000d;
`;

export type IProjectFormProps = FormComponentProps<{}> & {
  onProjectAdded?: () => void;
  project?: any;
  onClose?: any;
  company?: any;
};

const ProjectForm: React.FC<IProjectFormProps> = ({ form, onProjectAdded, project, company, onClose }) => {
const { getFieldDecorator, validateFields } = form;

const handleCompanyChange = (value: any) => {
  if(value){
    const filteredArray = company.filter((x: { id: any; })=>x.id === value);
    form.setFieldsValue({
      termId:filteredArray[0].termId,
      invoiceCycleId: filteredArray[0].invoiceCycleId
    })
  }
}

  const validateDiscount = (rule: any, value: any, callback: any) => {
    if (value.discountType && !value.discountValue) {
      callback('Please input the discount!');
    } else {
      callback();
    }
  };

  const handleRate = (val: any, prevVal: any) => {
    let strVal = val && val.toString();
    if (val >= 1000 || (val && strVal.includes('.') && strVal.split('.')[1].length > 2)) {
      return prevVal;
    } else {
      return val;
    }
  };

  const checkRate = (rule: any, value: any, callback: any) => {
    if (value > 0) {
      return callback();
    }
    callback('Rate must be greater than 0!');
  };

  const handleCharacters = (e: any) => {
    const invalidChars = ['-', '+', 'e'];
    if (invalidChars.includes(e.key) || e.keyCode === 38 || e.keyCode === 40) {
      e.preventDefault();
    }
  };

  return (
    <React.Fragment>
      <StyledForm {...formItemLayout}>
        <Form.Item label="Company">
          {getFieldDecorator('companyId', {
            rules: [{ required: true, message: 'Please input your Company!' }],
          })(<Select showSearch optionFilterProp="children" getPopupContainer={(trigger: any) => trigger.parentNode} onChange={handleCompanyChange} allowClear>
            {company.map((company: any, index: any) => (
              <Option value={company.id} key={index}>{company.displayName}</Option>
            ))}
          </Select>)}
        </Form.Item>
        <Form.Item label="Consultant">
          {getFieldDecorator('consultantId', {
            rules: [{ required: true, message: 'Please input your Consultant!' }],
          })(
            <ConnectedEntityPicker
              loader={(store: any) => store.getConsultants()}
              selector={(store: any) => store.consultants}
              mapFunc={(r: any) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
              addUrl={project && project.id ? `/projects/${project.id}/edit/consultants/new` : '/projects/new/consultants/new'}
              disabled={project && project.id ? true : false}
            />
          )}
        </Form.Item>
        <Form.Item label="End Client">
          {getFieldDecorator('endClientId', {
            rules: [{ required: true, message: 'Please input the end client!' }],
          })(
            <ConnectedEntityPicker
              loader={(store: any) => store.getEndClients()}
              selector={(store: any) => store.endClients}
              mapFunc={(r: any) => ({ value: r.id, text: r.clientName })}
              addUrl={project && project.id ? `/projects/${project.id}/edit/endClients/new` : '/projects/new/endClients/new'}
            />
          )}
        </Form.Item>
        <Form.Item label="Term">
          {getFieldDecorator('termId', {
            rules: [{ required: true, message: 'Please input term' }],
          })(<EntityPicker url="/api/services/app/Term/GetAll?MaxResultCount=25" mapFun={(r) => ({ value: r.id, text: r.name })} />)}
        </Form.Item>

        <Form.Item label="InvoiceCycle">
          {getFieldDecorator('invoiceCycleId', {
            rules: [{ required: true, message: 'Please input invoice cycle' }],
          })(<EntityPicker url="/api/services/app/InvoiceCycle/GetAll" mapFun={(r) => ({ value: r.id, text: r.name })} />)}
        </Form.Item>

        <Form.Item label="Project Memo">{getFieldDecorator('memo')(<CustomInput maxLength={31} />)}</Form.Item>
        <Form.Item label="Start Date">
          {getFieldDecorator('startDt', {
            rules: [{ required: true, message: 'Please input Start Date!' }],
          })(<StyledDatePicker allowClear={false} getCalendarContainer={(trigger: any) => trigger.parentNode} />)}
        </Form.Item>
        <Form.Item label="End Date">{getFieldDecorator('endDt')(<DatePicker getCalendarContainer={(trigger: any) => trigger.parentNode} />)}</Form.Item>
        <Form.Item label="InvoiceCycle Start Date">
          {getFieldDecorator('invoiceCycleStartDt', {
            rules: [{ required: true, message: 'Please input InvoiceCycle Start Date!' }],
          })(<StyledDatePicker getCalendarContainer={(trigger: any) => trigger.parentNode} />)}
        </Form.Item>
        <Form.Item label="Discount">
          {getFieldDecorator('discount', {
            rules: [{ validator: validateDiscount }],
          })(<DiscountInput />)}
        </Form.Item>
        <Form.Item label="Rate">
          {getFieldDecorator('rate', {
            rules: [{ required: true, message: ' ' }, { validator: checkRate }],
            normalize: handleRate,
          })(<CustomInput type="number" style={{ width: '90px' }} onKeyDown={handleCharacters} />)}
          </Form.Item>
        <Form.Item label="Send Mail">{getFieldDecorator('isSendMail', { valuePropName: 'checked' })(<StyledCheckbox></StyledCheckbox>)}</Form.Item>
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
          permissions={['Project.Create', 'Project.Update']}
          method={project && project.id ? 'Put' : 'Post'}
          style={{ height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
          url={`/api/services/app/Project/${project && project.id ? 'Update' : 'Create'}`}
          onSuccess={(response: any) => {
            onClose();
            notification.open({
              message: 'Success',
              description: project && project.id ? 'Project successfully updated.' : 'Project successfully created.',
            });
            project = {};
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
          Save Project
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
      termId: Form.createFormField({ value: project.termId}),
      invoiceCycleId: Form.createFormField({ value: project.invoiceCycleId}),
      memo: Form.createFormField({ value: project.memo }),
      startDt: Form.createFormField({ value: moment(project.startDt) }),
      endDt: Form.createFormField({ value: project.endDt && moment(project.endDt) }),
      consultantId: Form.createFormField({ value: project.consultantId }),
      rate: Form.createFormField({ value: project.rate }),
      isSendMail: Form.createFormField({ value: project.isSendMail === undefined ? true : project.isSendMail }),
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
    const returnVal = (val: string) => (_.get(fields, `${val}.value`) ? _.get(fields, `${val}.value`) : project[val]);
    const { project } = props;
    project.companyId = returnVal('companyId');
    project.termId = returnVal('termId');
    project.invoiceCycleId = returnVal('invoiceCycleId');
    project.memo = returnVal('memo');
    project.startDt = returnVal('startDt');
    project.endDt = returnVal('endDt');
    project.consultantId = returnVal('consultantId');
    project.rate = returnVal('rate');
    project.isSendMail = _.get(fields, 'isSendMail.value');
    project.discountType = _.get(fields, 'discount.value.discountType') ? _.get(fields, 'discount.value.discountType') : project.discountType;
    project.discountValue = _.get(fields, 'discount.value.discountValue') ? _.get(fields, 'discount.value.discountValue') : project.discountValue;
    project.invoiceCycleStartDt = returnVal('invoiceCycleStartDt');
    project.endClientId = returnVal('endClientId');
  },
})(ProjectForm);

export default WrappedProjectForm;
