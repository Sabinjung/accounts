/** @jsx jsx */
import React, { useImperativeHandle, forwardRef } from 'react';
import moment from 'moment';
import { PageHeader, Tabs, Descriptions, Typography, Steps, Icon, Button, Input, Form, Badge, Empty } from 'antd';
import { FormComponentProps } from 'antd/lib/form';

import { Get } from '../../../lib/axios';
import AttachmentViewer from './AttachmentViewer';
import HourEntries from './HourEntries';
import Expenses from './Expenses';

import ActionButton from '../../../components/ActionButton';
import ConfirmActionbutton from '../../../components/ConfirmActionButton';
import Condition from '../../../components/Condition';

import { jsx, css, ClassNames } from '@emotion/core';

import { useHistory } from 'react-router-dom';
import Authorize from '../../../components/Authorize';

const { TabPane } = Tabs;
const { Text } = Typography;
const { Step } = Steps;
const { TextArea } = Input;

export type ITimesheetViewerProps = FormComponentProps<{}> & {
  timesheetId: number;
  onTimesheetApproved?: () => void;
  onTimesheetDeleted?: () => void;
  path: string;
  handleRefetch?: any;
};

export type ITimesheetViewerHandles = {
  refetch: () => void;
};

const timesheetViewerStyles = css`
  background: white !important;
  &.ant-page-header {
    height: 100%;
    display: flex;
    flex-direction: column;

    .ant-page-header-footer {
      flex: 1;
      .ant-tabs {
        display: flex;
        height: 100%;
        flex-direction: column;
        .ant-tabs-content {
          flex: 1;
        }
      }
    }
  }

  .hour-entries-table {
    .ant-table-body {
      margin: 0 !important;
    }
  }

  .timesheet-timeline {
    margin-top: 25px;
    &:not(.ant-steps-label-vertical) .ant-steps-item-description {
      max-width: unset;
    }

    .ant-descriptions-row > td {
      padding-bottom: unset;
    }
  }

  th.is-holiday {
    background: #aaa !important;
  }
`;

const TimesheetViewer: React.RefForwardingComponent<ITimesheetViewerHandles, ITimesheetViewerProps> = (
  { timesheetId, onTimesheetApproved, onTimesheetDeleted, path, form, handleRefetch },
  ref
) => {
  const history = useHistory();
  let refetchFn: any = null;
  useImperativeHandle(ref, () => ({
    refetch: () => {
      refetchFn && refetchFn();
    },
  }));
  const { getFieldDecorator, validateFields } = form;

  return (
    <Get url="/api/services/app/Timesheet/Detail" params={{ timesheetId }}>
      {({ data, refetch }: any) => {
        const DeleteButton = (
          <ConfirmActionbutton
            url="/api/services/app/Timesheet/Delete"
            params={{ id: timesheetId }}
            method="Delete"
            type="danger"
            onSuccess={() => {
              refetch();
              onTimesheetDeleted && onTimesheetDeleted();
            }}
            permissions={['Timesheet.Delete']}
            title="Delete"
            onSubmit={({ setFormData }: any) => {
              let shouldProceed = false;
              validateFields((errors: any, values: any) => {
                if (!errors) {
                  const { noteText } = values;
                  setFormData({
                    noteText: noteText,
                  });
                  shouldProceed = true;
                } else {
                  shouldProceed = false;
                }
              });
              return shouldProceed;
            }}
          >
            {() => {
              return (
                <div>
                  Do you want to delete this timesheet?
                  <Form>
                    <Form.Item>
                      {getFieldDecorator('noteText', {
                        rules: [{ required: true, message: 'Please enter the reason for deletion?' }],
                      })(<TextArea />)}
                    </Form.Item>
                  </Form>
                </div>
              );
            }}
          </ConfirmActionbutton>
        );
        handleRefetch(refetch);

        refetchFn = refetch;
        if (data && data.result) {
          const {
            attachments,
            hourLogEntries,
            startDt,
            endDt,
            totalHrs,
            createdDt,
            statusId,
            createdByUserName,
            approvedDate,
            approvedByUserName,
            invoiceId,
            qbInvoiceId,
            invoiceGeneratedDate,
            invoiceGeneratedByUserName,
            project: { consultantName, companyName, startDt: projectStartDt, endDt: projectEndDt },
          } = data.result;
          return (
            <ClassNames>
              {({ css }) => (
                <PageHeader
                  title={consultantName}
                  subTitle={companyName}
                  extra={[
                    <Condition val={statusId == 1}>
                      <ActionButton
                        url="/api/services/app/Timesheet/Approve"
                        params={{ timesheetId }}
                        onSuccess={() => {
                          onTimesheetApproved && onTimesheetApproved();
                          refetch();
                        }}
                        permissions={['Timesheet.Approve']}
                      >
                        Approve
                      </ActionButton>
                      {DeleteButton}
                    </Condition>,
                    <Condition val={statusId == 2}>
                      <Authorize permissions={['Timesheet.GenerateInvoice']}>
                        <Button
                          type="primary"
                          onClick={() => {
                            history.push(`${path}/generate`);
                          }}
                        >
                          Generate Invoice
                        </Button>
                      </Authorize>
                      {DeleteButton}
                    </Condition>,
                    <Condition val={statusId == 5}>
                      <Authorize permissions={['Timesheet.GenerateInvoice']}>
                        <Button onClick={() => history.push(`${path}/invoice/${invoiceId}`)}> Submit Invoice</Button>
                      </Authorize>
                    </Condition>,
                    <Condition val={statusId == 4}>
                      <Authorize permissions={['Timesheet']}>
                        <Button onClick={() => history.push(`${path}/invoice/${invoiceId}`)} type="primary">
                          View Invoice
                        </Button>
                      </Authorize>
                    </Condition>,
                  ]}
                  className={css(timesheetViewerStyles)}
                  footer={
                    <Tabs defaultActiveKey="1">
                      <TabPane
                        tab={
                          <Badge count={data.result.attachments.length} offset={[10, -3]}>
                            Attachments
                          </Badge>
                        }
                        key="1"
                      >
                        {data.result.attachments.length !== 0 ? (
                          <AttachmentViewer attachments={attachments} />
                        ) : (
                          <Empty description="No Attachments" />
                        )}
                      </TabPane>
                      <TabPane tab="Timesheet History" key="3">
                        Coming Soon
                      </TabPane>
                      {(statusId === 1 || statusId === 2) && (
                        <TabPane tab="Expense" key="2">
                          <Expenses path={path} timesheetId={timesheetId} />
                        </TabPane>
                      )}
                    </Tabs>
                  }
                >
                  <div>
                    <Descriptions column={3}>
                      <Descriptions.Item label="Project">
                        <div>
                          <Text>{moment(projectStartDt).format('MM/DD/YYYY')}</Text> -{' '}
                          <Text>{projectEndDt && moment(projectEndDt).format('MM/DD/YYYY')}</Text>
                        </div>
                      </Descriptions.Item>
                      <Descriptions.Item
                        label="Timesheet Period"
                        className={css`
                          text-align: center;
                        `}
                      >
                        <div style={{ textAlign: 'right', fontWeight: 'bolder', fontSize: '1em' }}>
                          <Text>{moment(startDt).format('MM/DD/YYYY')}</Text> - <Text>{moment(endDt).format('MM/DD/YYYY')}</Text>
                        </div>
                      </Descriptions.Item>
                      <Descriptions.Item>
                        <span></span>
                      </Descriptions.Item>
                    </Descriptions>

                    <HourEntries hourLogEntries={hourLogEntries} startDt={startDt} endDt={endDt} totalHrs={totalHrs} />
                    <Steps
                      initial={1}
                      current={statusId == 5 ? 3 : statusId}
                      size="small"
                      className="timesheet-timeline"
                      status={statusId == 5 ? 'wait' : 'finish'}
                    >
                      <Step
                        title="Started"
                        description={
                          <div style={{ display: 'flex', flexDirection: 'column' }}>
                            <Text>{createdByUserName}</Text>
                            <Text>{moment(createdDt).format('MM/DD/YYYY')}</Text>
                          </div>
                        }
                      />
                      <Step
                        title="Approved"
                        description={
                          <div style={{ display: 'flex', flexDirection: 'column' }}>
                            <Text>{approvedByUserName}</Text>
                            <Text>{approvedDate && moment(approvedDate).format('MM/DD/YYYY')}</Text>
                          </div>
                        }
                      />
                      <Step
                        title="Invoiced"
                        status={statusId == 5 ? 'process' : undefined}
                        icon={statusId == 5 && <Icon type="loading" />}
                        description={
                          <div style={{ display: 'flex', flexDirection: 'column' }}>
                            <Text>{invoiceGeneratedByUserName}</Text>
                            <Text>{invoiceGeneratedDate && moment(invoiceGeneratedDate).format('MM/DD/YYYY')}</Text>
                            {qbInvoiceId && (
                              <Text>
                                Intuit Id:
                                {qbInvoiceId ? (
                                  <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${qbInvoiceId}`} target="_blank">
                                    {qbInvoiceId}
                                  </a>
                                ) : null}
                              </Text>
                            )}
                          </div>
                        }
                      />
                    </Steps>
                  </div>
                </PageHeader>
              )}
            </ClassNames>
          );
        } else {
          return null;
        }
      }}
    </Get>
  );
};

const wrappedForm = Form.create<ITimesheetViewerProps>()(forwardRef(TimesheetViewer));

export default wrappedForm;
