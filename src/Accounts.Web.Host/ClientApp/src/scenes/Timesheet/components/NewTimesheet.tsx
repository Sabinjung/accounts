import React, { useState, useEffect } from 'react';
import { Button, Typography, Spin, Descriptions, Checkbox, Row, Col, Popconfirm, Input, notification, Alert, DatePicker } from 'antd';
import moment from 'moment';
import _ from 'lodash';
import { inject, observer } from 'mobx-react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip } from 'recharts';
import { AutoSizer } from 'react-virtualized';
import { useHistory } from 'react-router-dom';
import Stores from '../../../stores/storeIdentifier';
import HourLogEntryModel from '../../../models/Timesheet/hourLogEntryModel';
import AttachmentModel from '../../../models/Timesheet/attachmentModel';
import styled from '@emotion/styled';
import CustomCancleButton from '../../../components/Custom/CustomCancelButton';
import CustomButton from './../../../components/Custom/CustomButton';

const { Title, Text } = Typography;
const { TextArea } = Input;

const StyledDiscription = styled(Descriptions)`
  .ant-descriptions-row {
    vertical-align: baseline;
  }
`;

const StyledSpin = styled(Spin)`
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
`;

export default inject(
  Stores.TimesheetStore,
  Stores.ProjectStore
)(
  observer(({ visible, onClose, onSave, timesheetStore, projectStore, projectId }: any) => {
    const [isLoading, setLoading] = useState(false);

    useEffect(() => {
      if (projectId) {
        setLoading(true);
        timesheetStore.getUpcomingTimesheetInfo(projectId);
        projectStore.getAttachments(projectId);
        setLoading(false);
      }
    }, [projectId]);

    const { upcomingTimesheet } = timesheetStore;

    const history = useHistory();
    const { attachments } = projectStore;
    const attachmentList =
      (attachments && attachments.map((a: AttachmentModel) => ({ label: a.originalName, value: a.id, checked: a.isSelected }))) || [];

    const selectedAttachmentList = (attachments && attachments.filter((x: AttachmentModel) => x.isSelected).map((a: AttachmentModel) => a.id)) || [];

    async function onCreateTimesheet() {
      await timesheetStore.createTimesheet();
      onClose && onClose();
      onSave && onSave();
      notification.open({
        message: 'Success',
        description: 'Timesheet successfully created.',
        onClick: () => {
          console.log('Notification Clicked!');
        },
      });
    }

    function onAttachmentSelection(checkedValues: Array<any>) {
      const selectAttachment = (isSelect: boolean) => (i: any) => {
        if (attachments) {
          const selectedAttachment = attachments.find((a: AttachmentModel) => a.id == i);
          if (selectedAttachment) {
            selectedAttachment.select(isSelect);
          }
        }
      };
      checkedValues && checkedValues.forEach(selectAttachment(true));
      attachments
        .filter((a: AttachmentModel) => !checkedValues.includes(a.id))
        .map((x: AttachmentModel) => x.id)
        .forEach(selectAttachment(false));
    }

    const handleDateChange = (date: any) => {
      upcomingTimesheet.endDt = date.format('YYYY-MM-DD');
    };

    const disabledDate: any = (current: any) => {
      let startDate = upcomingTimesheet && upcomingTimesheet.originalStartDt;
      let endDate = upcomingTimesheet && upcomingTimesheet.originalEndDt;
      console.log('EndDate', endDate);
      return current < moment(startDate) || current >= moment(endDate).endOf('days');
    };

    return (
      <React.Fragment>
        <StyledSpin size="large" spinning={isLoading || upcomingTimesheet == null}>
          {upcomingTimesheet && (
            <React.Fragment>
              <Title level={4}>Project Info</Title>
              <Descriptions>
                <Descriptions.Item label="Consultant" span={3}>
                  {upcomingTimesheet.project.consultantName}
                </Descriptions.Item>
                <Descriptions.Item label="Company" span={3}>
                  {upcomingTimesheet.project.companyName}
                </Descriptions.Item>
              </Descriptions>

              <StyledDiscription column={2}>
                <Descriptions.Item label="Start Date">
                  <Text>{moment(upcomingTimesheet.project.startDt).format('MM/DD/YYYY')}</Text>
                </Descriptions.Item>
                <Descriptions.Item label="End Date">
                  <Text>{(upcomingTimesheet.project.endDt && moment(upcomingTimesheet.project.endDt).format('MM/DD/YYYY')) || ''}</Text>
                </Descriptions.Item>
              </StyledDiscription>

              <Title level={4}>
                Total Hours (<Text>{upcomingTimesheet.totalHrs}</Text> hrs)
              </Title>

              <StyledDiscription column={2}>
                <Descriptions.Item label="Start Date">
                  <Text>{moment(upcomingTimesheet.startDt).format('MM/DD/YYYY')}</Text>
                </Descriptions.Item>
                <Descriptions.Item label="End Date">
                  <DatePicker
                    allowClear={false}
                    disabledDate={disabledDate}
                    style={{ width: '115px' }}
                    value={moment(upcomingTimesheet.endDt)}
                    format="MM/DD/YYYY"
                    size="small"
                    onChange={handleDateChange}
                  />
                </Descriptions.Item>
              </StyledDiscription>

              <AutoSizer disableHeight>
                {({ width }) => (
                  <BarChart
                    width={width}
                    height={250}
                    data={_.orderBy(upcomingTimesheet.filteredHourLogEntries, (x) => moment(x.day), ['asc']).map((l: HourLogEntryModel) => ({
                      day: moment(l.day).format('M/D'),
                      hrs: l.hours,
                    }))}
                    margin={{ top: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="day" />
                    <YAxis yAxisId="left" orientation="left" stroke="#748AA1" />
                    <Tooltip />
                    <Bar yAxisId="left" dataKey="hrs" fill="#1C3FAA" />
                  </BarChart>
                )}
              </AutoSizer>

              <Row type="flex" justify="space-between" style={{ marginBottom: '25px' }}>
                <Col>
                  <Title level={4}>Attachments</Title>
                </Col>
                <Col>
                  <Button
                    onClick={() => {
                      history.push(`/hourlogentry/${projectId}/new/attachments?isAttachable=true`);
                    }}
                    size="small"
                  >
                    Show Attachments
                  </Button>
                </Col>
              </Row>

              {attachmentList.length === 0 ? (
                <Alert message="Upload at least 1 attachment first" type="warning" />
              ) : (
                <Checkbox.Group
                  options={attachmentList}
                  value={selectedAttachmentList}
                  onChange={onAttachmentSelection}
                  className="attachment-selection-list"
                />
              )}
            </React.Fragment>
          )}
        </StyledSpin>
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
          {attachmentList.length !== 0 && selectedAttachmentList.length !== 0 ? (
            <Popconfirm
              title={
                <div>
                  <p>Do you want to submit this timesheet?</p>
                  <TextArea
                    value={upcomingTimesheet && upcomingTimesheet.noteText}
                    onChange={(e: any) => (upcomingTimesheet.noteText = e.target.value)}
                  />
                </div>
              }
              okText="Yes"
              cancelText="No"
              placement="topRight"
              onConfirm={onCreateTimesheet}
            >
              <CustomButton type="primary">Submit</CustomButton>
            </Popconfirm>
          ) : (
            <CustomButton type="primary" disabled>
              Submit
            </CustomButton>
          )}
        </div>
      </React.Fragment>
    );
  })
);
