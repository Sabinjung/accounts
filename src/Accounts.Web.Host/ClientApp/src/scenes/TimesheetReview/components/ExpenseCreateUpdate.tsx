import React from 'react';
import { Form, Input, InputNumber, Button, notification, DatePicker } from 'antd';
import { FormComponentProps } from 'antd/lib/form';

import ActionButton from '../../../components/ActionButton';
import EntityPicker from '../../../components/EntityPicker';
import _ from 'lodash';
import moment from 'moment';

const { TextArea } = Input;

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

export type IExpenseFormProps = FormComponentProps<{}> & {
  onExpenseAdded?: () => void;
  onClose?: any;
  timesheetId?: any;
  expense?: any;
};

const ExpenseForm: React.FC<IExpenseFormProps> = ({ form, onExpenseAdded, onClose, timesheetId, expense }) => {
  const { getFieldDecorator, validateFields } = form;

  const checkAmount = (rule: any, value: any, callback: any) => {
    if (value > 0) {
      return callback();
    }
    callback('Amount must be greater than 0!');
  };
  return (
    <React.Fragment>
      <Form {...formItemLayout}>
        <Form.Item label="ExpenseType">
          {getFieldDecorator('expenseTypeId', {
            rules: [{ required: true, message: 'Please Input Expense Type!' }],
          })(<EntityPicker url="/api/services/app/ExpenseType/Search" placeholder="Type" mapFun={(r) => ({ value: r.id, text: r.name })} />)}
        </Form.Item>
        <Form.Item label="Date">
          {getFieldDecorator('reportDt', {
            rules: [{ required: true, message: 'Please Input Date!' }],
          })(<DatePicker disabledDate={(current: any) => current > moment()} placeholder="Date" />)}
        </Form.Item>
        <Form.Item label="Amount">
          {getFieldDecorator('amount', {
            rules: [{ required: true, message: ' ' }, { validator: checkAmount }],
          })(
            <InputNumber
              placeholder="Amount"
              formatter={(value) => `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
              parser={(value) => value!.replace(/\$\s?|(,*)/g, '')}
            />
          )}
        </Form.Item>
        <Form.Item label="Comment">{getFieldDecorator('comment')(<TextArea rows={2} placeholder="Comment..." />)}</Form.Item>
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
        <ActionButton
          permissions={['Expense.Create', 'Expense.Update']}
          method={expense && expense.id ? 'Put' : 'Post'}
          url={`/api/services/app/Expense/${expense && expense.id ? 'Update' : 'Create'} `}
          onSuccess={() => {
            notification.open({
              message: 'Success',
              description: expense && expense.id ? 'Expense successfully updated.' : 'Expense successfully added.',
            });
            onExpenseAdded && onExpenseAdded();
          }}
          onSubmit={({ setFormData, setIsReady }: any) => {
            validateFields((errors, values: any) => {
              if (!errors) {
                const { expenseTypeId, reportDt, amount, comment } = values;
                setFormData({
                  id: expense && expense.id,
                  expenseTypeId: expenseTypeId,
                  reportDt: reportDt.format('MM/DD/YYYY'),
                  amount: amount,
                  comment: comment,
                  timesheetId: timesheetId,
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
        <Button
          type="danger"
          style={{ marginLeft: 8 }}
          onClick={() => {
            onClose();
          }}
        >
          Cancel
        </Button>
      </div>
    </React.Fragment>
  );
};

const WrappedExpenseForm = Form.create<IExpenseFormProps>({
  name: 'Expense_state',

  mapPropsToFields(props: any) {
    const { expense } = props;
    if (!expense) return;
    return {
      expenseTypeId: Form.createFormField({ value: expense.expenseTypeId }),
      reportDt: Form.createFormField({ value: moment(expense.reportDt) }),
      amount: Form.createFormField({ value: expense.amount }),
      comment: Form.createFormField({ value: expense.comment }),
    };
  },
})(ExpenseForm);

export default WrappedExpenseForm;
