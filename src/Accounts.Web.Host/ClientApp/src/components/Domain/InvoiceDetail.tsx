/** @jsx jsx */
import React, { useState } from 'react';
import moment from 'moment';
import { Get } from '../../lib/axios';
import { Descriptions, Button, List, Tag, Popconfirm, Input } from 'antd';
import { jsx, css } from '@emotion/core';
import { AxiosError } from 'axios';
import AppConsts from '../../lib/appconst';
import ActionButton from '../ActionButton';
import Authorize from '../Authorize';
import useAxios from '../../lib/axios/useAxios';
import { isGranted } from '../../lib/abpUtility';

const { TextArea } = Input;

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

const InvoiceDetail = ({ invoice, onClose, onInvoiceSubmitted }: any) => {
  const [qbInvoiceId, setQbInvoiceId] = useState('');
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
  } = invoice;

  return (
    <React.Fragment>
      <Descriptions layout="vertical" column={4} size="small">
        <Descriptions.Item label="Customer">{companyName}</Descriptions.Item>
        <Descriptions.Item label="Customer Email">{companyEmail}</Descriptions.Item>
        <Descriptions.Item label="Billing Address" span={2}>
          <code> Populated from Intuit</code>
        </Descriptions.Item>
        <Descriptions.Item label="Term">{termName}</Descriptions.Item>
        <Descriptions.Item label="Invoice Date">{moment(invoiceDate).format('MM/DD/YYYY')}</Descriptions.Item>
        <Descriptions.Item label="Due Date">{moment(dueDate).format('MM/DD/YYYY')}</Descriptions.Item>
        <Descriptions.Item label="Consultant">{consultantName}</Descriptions.Item>
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
          <td>{description}</td>
          <td>{totalHours}</td>
          <td>${rate}</td>
          <td>${serviceTotal}</td>
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
          <td>${subTotal}</td>
        </tr>
        <tr>
          <td colSpan={3} style={{ textAlign: 'right', padding: 5, fontWeight: 'bold' }}>
            {discountType == 1 ? 'Discount Percentage' : 'Discount Value'}
          </td>
          <td>{discountValue && (discountType == 1 ? `${discountValue}%` : `$${discountValue}`)}</td>
          <td>{discountAmount && `-$${discountAmount}`}</td>
        </tr>
        <tr>
          <td colSpan={4} style={{ textAlign: 'right', padding: 5, fontWeight: 'bold' }}>
            Total :
          </td>
          <td>${total}</td>
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

        {!qboInvoiceId && invoice.isSendMail && isGranted('Invoicing.SubmitAndMail') ? (
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
      </div>
    </React.Fragment>
  );
};

const ConnectedInvoiceDetail = ({ invoiceId, onClose, onInvoiceSubmitted }: any) => {
  return (
    <Get url={'/api/services/app/Invoice/Get'} params={{ id: invoiceId }}>
      {({ error, data, isLoading }: any) => {
        const result = data && data.result;
        return <InvoiceDetail invoice={result} onClose={onClose} onInvoiceSubmitted={onInvoiceSubmitted} />;
      }}
    </Get>
  );
};

export const InvoiceDetailForm = InvoiceDetail;

export default ConnectedInvoiceDetail;
