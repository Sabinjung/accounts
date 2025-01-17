/** @jsx jsx */
import React, { useState, useImperativeHandle, forwardRef } from 'react';
import moment from 'moment';
import { PageHeader, Tabs, Descriptions, Typography, Steps, Icon, Input, Form, Badge, Empty, notification, Spin } from 'antd';
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
import CustomButton from '../../../components/Custom/CustomButton';
import styled from '@emotion/styled';

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

const StyledSteps = styled(Steps)`
  margin-top: 25px;
  .ant-steps-item-finish .ant-steps-item-icon {
    border-color: #7034bd;
    .ant-steps-icon {
      color: #7034bd;
    }
  }
  .ant-steps-item-finish > .ant-steps-item-container > .ant-steps-item-content > .ant-steps-item-title::after {
    background-color: #7034bd;
  }
  &:not(.ant-steps-label-vertical) .ant-steps-item-description {
    max-width: unset;
  }

  .ant-descriptions-row > td {
    padding-bottom: unset;
  }
}
`;

const timesheetViewerStyles = css`
  background: white !important;
  &.ant-page-header {
    height: 100%;
    padding: 40px;
    margin-left: 10px;
    border-radius: 20px;
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
    th {
      text-align: center;
    }
    td {
      text-align: center;
    }
    th.is-holiday {
      background: #aaa !important;
    }
    td.column-sum {
      background-color: #7034bd !important;
      color: white;
    }
    .ant-table-tbody > tr > td {
      border: none !important;
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
`;

const StyledSpin = styled(Spin)`
  position: absolute;
  left: 50%;
  top: 40%;
  transform: translate(-50%, -50%);
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
  const [disable, setDisable] = useState<boolean>(true);

  const handleTextLength = (e: any) => {
    const disable = e.target.value && e.target.value.length > 0 ? false : true;
    setDisable(disable);
  }

  const handleVisibleChange = (e: any) => {
    if(e){
      setDisable(true);
    }
  }

  return (
    <Get url="/api/services/app/Timesheet/Detail" params={{ timesheetId }}>
      {({ data, refetch, loading }: any) => {
        const DeleteButton = (
          <ConfirmActionbutton
            url="/api/services/app/Timesheet/Delete"
            params={{ id: timesheetId }}
            method="Delete"
            style={{ background: '#FF0000', height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
            type="danger"
            disable={disable}
            onVisibleChange={handleVisibleChange}
            onSuccess={() => {
              notification.open({
                message: 'Success',
                description: 'Successfully Deleted.',
              });
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
                      })(<TextArea onChange={handleTextLength} />)}
                    </Form.Item>
                  </Form>
                </div>
              );
            }}
          </ConfirmActionbutton>
        );
        handleRefetch(refetch);

        refetchFn = refetch;
        if (data && data.result && !loading) {
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
            invoiceCompanyName,
            invoiceGeneratedDate,
            invoiceGeneratedByUserName,
            project: { consultantName, startDt: projectStartDt, endDt: projectEndDt },
          } = data.result;
          return (
            <ClassNames>
              {({ css }) => (
                <PageHeader
                  title={consultantName}
                  subTitle={invoiceCompanyName}
                  extra={[
                    <Condition val={statusId == 1}>
                      <ActionButton
                        url="/api/services/app/Timesheet/Approve"
                        params={{ timesheetId }}
                        style={{ height: '40px', boxShadow: '0px 3px 20px #2680EB66' }}
                        onSuccess={() => {
                          notification.open({
                            message: 'Success',
                            description: 'Successfully Approved.',
                          });
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
                        <CustomButton
                          type="primary"
                          onClick={() => {
                            history.push(`${path}/generate`);
                          }}
                        >
                          Generate Invoice
                        </CustomButton>
                      </Authorize>
                      {DeleteButton}
                    </Condition>,
                    <Condition val={statusId == 5}>
                      <Authorize permissions={['Timesheet.GenerateInvoice']}>
                        <CustomButton type="primary" onClick={() => history.push(`${path}/invoice/${invoiceId}`)}>
                          {' '}
                          Submit Invoice
                        </CustomButton>
                      </Authorize>
                    </Condition>,
                    <Condition val={statusId == 4}>
                      <Authorize permissions={['Timesheet']}>
                        <CustomButton onClick={() => history.push(`${path}/invoice/${invoiceId}`)} type="primary">
                          View Invoice
                        </CustomButton>
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
                    <StyledSteps
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
                                eTrans ID:
                                {qbInvoiceId ? (
                                  <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${qbInvoiceId}`} target="_blank">
                                    {' ' + qbInvoiceId}
                                  </a>
                                ) : null}
                              </Text>
                            )}
                          </div>
                        }
                      />
                    </StyledSteps>
                  </div>
                </PageHeader>
              )}
            </ClassNames>
          );
        } else {
          return <StyledSpin size="default" />
        }
      }}
    </Get>
  );
};

const wrappedForm = Form.create<ITimesheetViewerProps>()(forwardRef(TimesheetViewer));

export default wrappedForm;
