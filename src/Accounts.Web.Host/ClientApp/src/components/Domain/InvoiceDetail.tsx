/** @jsx jsx */
import React, { useState } from 'react';
import moment from 'moment';
import { Get } from '../../lib/axios';
import { Descriptions, Button, List, Tag, Popconfirm, Input, Row, Col, Alert, notification, Select } from 'antd';
import { jsx, css } from '@emotion/core';
import styled from '@emotion/styled';
import { AxiosError } from 'axios';
import AppConsts from '../../lib/appconst';
import ActionButton from '../ActionButton';
import Authorize from '../Authorize';
import useAxios from '../../lib/axios/useAxios';
import { isGranted } from '../../lib/abpUtility';
import EditHourlog from './EditHourlog';

const { TextArea } = Input;
const { Option } = Select;

const tableStyles = css`
  width: 100%;
  border-radius: 5px;
  border: 1px solid #aaa;
  margin-top: 15px;
  th,
  td {
    padding: 5px;
  }
  th {
    background: #eee;
  }
`;

const StyledRow = styled(Row)`
  margin-bottom: 10px;
`;

const StyledInput = styled(Input)`
  width: 40px !important;
`;

const StyledSelect = styled(Select)`
  width: 107px !important;
`;
let originalHours: any;
const InvoiceDetail = ({ invoice, onClose, onInvoiceSubmitted, hourEntries }: any) => {
  const [qbInvoiceId, setQbInvoiceId] = useState('');
  const [isEdit, setIsEdit] = useState(false);
  const [form, setForm]: any = useState({ rate: 0, discountValue: 0 });
  const [disType, setDisType] = useState(null);
  const [logedHours, setLogedHours]: any = useState();

  const [{}, makeRequest] = useAxios(
    {
      url: '/api/services/app/Invoice/GenerateAndSave',
      method: 'POST',
      params: {
        timesheetId: invoice && invoice.timesheetId,
        qbInvoiceId,
      },
    },
    {
      isReady: false,
      onSuccess: () => {
        onClose && onClose();
        setTimeout(() => onInvoiceSubmitted && onInvoiceSubmitted());
      },
    }
  );

  if (!invoice) return null;
  const {
    timesheetId,
    companyName,
    companyEmail,
    termName,
    invoiceDate,
    dueDate,
    consultantName,
    description,
    totalHours,
    rate,
    serviceTotal,
    subTotal,
    total,
    discountType,
    discountValue,
    discountAmount,
    lineItems,
    attachments,
    qboInvoiceId,
    endClientName,
    isSendMail,
    id,
  } = invoice;

  let initialAmount = serviceTotal;
  let TotalAmount = total;
  let discount = discountAmount;
  let totalHrs = totalHours;
  let sTotal = subTotal;
  let totalExpense: number = 0;

  lineItems.length > 0 && lineItems.map((val: any) => (totalExpense += val.amount));

  const handleEdit = () => {
    originalHours = hourEntries && hourEntries.result.hourLogEntries;
    originalHours.map((item: any) => !item.hours && (item.hours = 0));
    setForm({ rate, discountValue });
    setDisType(discountType ? discountType : 2);
    setLogedHours(originalHours);
    setIsEdit((prevState: boolean) => !prevState);
  };

  const updateField = (e: any) => {
    let rx = /^\d*\.?\d{0,2}$/;
    setForm({
      ...form,
      [e.target.name]: rx.test(e.target.value) ? e.target.value : form[e.target.name],
    });
  };

  if (form.rate || form.discountValue || disType || logedHours) {
    totalHrs = 0;
    logedHours.map((item: any) => (totalHrs += item.hours));
    initialAmount = !form.rate ? 0 : (parseFloat(form.rate) * totalHrs).toFixed(2);
    sTotal = parseFloat(initialAmount + totalExpense).toFixed(2);
    discount = !form.discountValue
      ? 0
      : disType === 1
      ? parseFloat((sTotal * (parseFloat(form.discountValue) / 100)).toFixed(2))
      : parseFloat(form.discountValue).toFixed(2);
    TotalAmount = (sTotal - discount).toFixed(2);
  }

  return (
    <React.Fragment>
      {!timesheetId && qboInvoiceId && isGranted('Invoicing.Edit') && (
        <StyledRow type="flex" justify="end">
          <Col>
            <Button type="primary" onClick={handleEdit}>
              Edit
            </Button>
          </Col>
        </StyledRow>
      )}
      <Descriptions layout="vertical" column={4} size="small">
        <Descriptions.Item label="Customer">{companyName}</Descriptions.Item>
        <Descriptions.Item label="Customer Email">{companyEmail}</Descriptions.Item>
        <Descriptions.Item label="Billing Address">
          <code> Populated from Intuit</code>
        </Descriptions.Item>
        <Descriptions.Item label="Term">{termName}</Descriptions.Item>
        <Descriptions.Item label="Invoice Date">{moment(invoiceDate).format('MM/DD/YYYY')}</Descriptions.Item>
        <Descriptions.Item label="Due Date">{moment(dueDate).format('MM/DD/YYYY')}</Descriptions.Item>
        <Descriptions.Item label="Consultant">{consultantName}</Descriptions.Item>
        {endClientName && <Descriptions.Item label="End Client">{endClientName}</Descriptions.Item>}
      </Descriptions>
      <table css={tableStyles}>
        <tr>
          <th>Product/Service</th>
          <th>Description</th>
          <th>QTY</th>
          <th>Rate</th>
          <th>Amount</th>
        </tr>
        <tr>
          <td>Services</td>
          <td>
            {isEdit ? (
              <EditHourlog description={description} logedHours={logedHours} setLogedHours={setLogedHours} originalHours={originalHours} />
            ) : (
              description
            )}
          </td>
          <td>{totalHrs}</td>
          <td>{isEdit ? <StyledInput name="rate" size="small" value={form.rate} onChange={updateField} /> : `$${rate}`}</td>
          <td>${initialAmount}</td>
        </tr>
        {lineItems &&
          lineItems.map((l: any) => (
            <tr>
              <td>{l.expenseTypeName}</td>
              <td>
                {' '}
                {l.description} <Tag>{moment(l.serviceDt).format('MM/DD/YYYY')}</Tag>{' '}
              </td>
              <td></td>
              <td></td>
              <td>${l.amount}</td>
            </tr>
          ))}
        <tr>
          <td colSpan={4} style={{ textAlign: 'right', padding: 5, fontWeight: 'bold' }}>
            Sub total :
          </td>
          <td>${sTotal}</td>
        </tr>
        <tr>
          <td colSpan={3} style={{ textAlign: 'right', padding: 5, fontWeight: 'bold' }}>
            {isEdit ? (
              <Row type="flex" gutter={4} align="middle" justify="end">
                <Col>Discount</Col>
                <Col>
                  <StyledSelect size="small" value={disType} onChange={(val: any) => setDisType(val)}>
                    <Option value={1}>Percentage</Option>
                    <Option value={2}>Value</Option>
                  </StyledSelect>
                </Col>
              </Row>
            ) : discountType == 1 ? (
              'Discount Percentage'
            ) : (
              'Discount Value'
            )}
          </td>
          <td>
            {isEdit ? (
              <StyledInput name="discountValue" size="small" value={form.discountValue} onChange={updateField} />
            ) : (
              discountValue && (discountType == 1 ? `${discountValue}%` : `$${discountValue}`)
            )}
          </td>
          <td>-${discount}</td>
        </tr>
        <tr>
          <td colSpan={4} style={{ textAlign: 'right', padding: 5, fontWeight: 'bold' }}>
            Total :
          </td>
          <td>${TotalAmount}</td>
        </tr>
      </table>
      <List
        header={<b>Attachments</b>}
        dataSource={attachments}
        renderItem={(item: any) => (
          <List.Item
            css={css`
              padding: 5px 0;
            `}
          >
            <a href={`${AppConsts.remoteServiceBaseUrl}/Attachment/Index/${item.id}`} target="_blank">
              {item.originalName}
            </a>
          </List.Item>
        )}
      />
      {isEdit && !parseInt(form.rate) && <Alert message="Rate can't be null or 0" type="error" />}

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
        <Button style={{ marginRight: 8 }} onClick={onClose}>
          Cancel
        </Button>
        {!qboInvoiceId && (
          <Authorize permissions={['Invoicing.Submit', 'Invoicing.SubmitAndMail']}>
            <Popconfirm
              title={
                <div>
                  Enter the QB Invoice Id
                  <TextArea value={qbInvoiceId} onChange={(e: any) => setQbInvoiceId(e.target.value)} />
                </div>
              }
              okText="Save"
              cancelText="Cancel"
              placement="topRight"
              onConfirm={() => {
                makeRequest({});
              }}
            >
              <Button type="primary" style={{ marginRight: 8 }}>
                Save
              </Button>
            </Popconfirm>
          </Authorize>
        )}

        {!qboInvoiceId && isSendMail && isGranted('Invoicing.SubmitAndMail') ? (
          <ActionButton
            url="/api/services/app/Invoice/GenerateAndMailInvoice"
            params={{ timesheetId }}
            onSuccess={() => {
              onClose && onClose();
              setTimeout(() => onInvoiceSubmitted && onInvoiceSubmitted());
            }}
            onError={(err: AxiosError) => {
              if (err && err.response && err.response.status == 403) {
                window.location.href = `${AppConsts.remoteServiceBaseUrl}/Intuit/Login?returnUrl=${window.location.href}`;
              }
            }}
          >
            Submit and Mail
          </ActionButton>
        ) : (
          !qboInvoiceId && (
            <ActionButton
              url="/api/services/app/Invoice/GenerateAndSubmit"
              params={{ timesheetId }}
              style={{ marginRight: 8 }}
              onSuccess={() => {
                onClose && onClose();
                setTimeout(() => onInvoiceSubmitted && onInvoiceSubmitted());
              }}
              onError={(err: AxiosError) => {
                if (err && err.response && err.response.status == 403) {
                  window.location.href = `${AppConsts.remoteServiceBaseUrl}/Intuit/Login?returnUrl=${window.location.href}`;
                }
              }}
              permissions={['Invoicing.Submit']}
            >
              Submit Invoice
            </ActionButton>
          )
        )}
        {isEdit && (
          <ActionButton
            url="/api/services/app/Invoice/UpdateInvoice"
            permissions={['Invoicing.Submit', 'Invoicing.SubmitAndMail']}
            method="Put"
            onSuccess={() => {
              notification.open({
                message: 'Success',
                description: 'Updated and Submitted successfully.',
              });
              onClose && onClose();
              setTimeout(() => onInvoiceSubmitted && onInvoiceSubmitted());
            }}
            onSubmit={({ setFormData, setIsReady }: any) => {
              if (parseInt(form.rate) && totalHrs) {
                setFormData({
                  invoice: {
                    totalHours: totalHrs,
                    rate: form.rate,
                    discountValue: form.discountValue,
                    discountAmount: discount,
                    discountType: disType,
                    serviceTotal: initialAmount,
                    subTotal: sTotal,
                    total: TotalAmount,
                    isSendMail: isGranted('Invoicing.SubmitAndMail') && isSendMail,
                    id,
                  },
                  updatedHourLogEntries: logedHours,
                });
                setIsReady(true);
              } else {
                setIsReady(false);
              }
            }}
          >
            {isGranted('Invoicing.SubmitAndMail') && isSendMail ? 'Submit and Mail' : 'Submit Invoice'}
          </ActionButton>
        )}
      </div>
    </React.Fragment>
  );
};

const ConnectedInvoiceDetail = ({ invoiceId, onClose, onInvoiceSubmitted }: any) => {
  const [{ data: hourEntries }] = useAxios({
    url: '/api/services/app/HourLogEntry/GetInvoicedHourLogs',
    params: { invoiceId },
  });

  return (
    <Get url={'/api/services/app/Invoice/Get'} params={{ id: invoiceId }}>
      {({ error, data, isLoading }: any) => {
        const result = data && data.result;
        return <InvoiceDetail invoice={result} onClose={onClose} onInvoiceSubmitted={onInvoiceSubmitted} hourEntries={hourEntries} />;
      }}
    </Get>
  );
};

export const InvoiceDetailForm = InvoiceDetail;

export default ConnectedInvoiceDetail;
