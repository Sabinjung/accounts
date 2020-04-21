import React, { useState, useEffect } from 'react';
import { Button, Typography, Spin, Descriptions, Checkbox, Row, Col, Popconfirm, Input, notification, Alert } from 'antd';
import moment from 'moment';
import _ from 'lodash';
import { inject, observer } from 'mobx-react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip } from 'recharts';
import { AutoSizer } from 'react-virtualized';
import { useHistory } from 'react-router-dom';
import Stores from '../../../stores/storeIdentifier';
import HourLogEntryModel from '../../../models/Timesheet/hourLogEntryModel';
import AttachmentModel from '../../../models/Timesheet/attachmentModel';

const { Title, Text } = Typography;
const { TextArea } = Input;

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

    return (
      <React.Fragment>
        <Spin spinning={isLoading || upcomingTimesheet == null}>
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
                <Descriptions.Item label="Start Date">
                  <Text>{moment(upcomingTimesheet.project.startDt).format('MM/DD/YYYY')}</Text>
                </Descriptions.Item>
                <Descriptions.Item label="End Date" span={2}>
                  <Text>{(upcomingTimesheet.project.endDt && moment(upcomingTimesheet.project.endDt).format('MM/DD/YYYY')) || ''}</Text>
                </Descriptions.Item>
              </Descriptions>

              <Title level={4}>
                Total Hours (<Text>{upcomingTimesheet.totalHrs}</Text> hrs)
              </Title>

              <Descriptions>
                <Descriptions.Item label="Start Date">
                  <Text>{moment(upcomingTimesheet.startDt).format('MM/DD/YYYY')}</Text>
                </Descriptions.Item>
                <Descriptions.Item label="End Date">
                  <Text>{moment(upcomingTimesheet.endDt).format('MM/DD/YYYY')}</Text>
                </Descriptions.Item>
              </Descriptions>
              <AutoSizer disableHeight>
                {({ width }) => (
                  <BarChart
                    width={width}
                    height={250}
                    data={_.orderBy(upcomingTimesheet.filteredHourLogEntries, x => moment(x.day), ['asc']).map((l: HourLogEntryModel) => ({
                      day: moment(l.day).format('M/D'),
                      hrs: l.hours,
                    }))}
                    margin={{ top: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="day" />
                    <YAxis yAxisId="left" orientation="left" stroke="#8884d8" />
                    <Tooltip />
                    <Bar yAxisId="left" dataKey="hrs" fill="#82ca9d" />
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
              
              {attachmentList.length === 0 ? 
                <Alert message="Upload at least 1 attaachment first" type="warning" /> : 
                <Checkbox.Group
                  options={attachmentList}
                  value={selectedAttachmentList}
                  onChange={onAttachmentSelection}
                  className="attachment-selection-list"
                />
              }
            </React.Fragment>
          )}
        </Spin>
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
          <Button style={{ marginRight: 8 }} onClick={() => { onClose() }}>Cancel</Button>
          {attachmentList.length !== 0 && selectedAttachmentList.length !== 0 ? 
            <Popconfirm
              title={
                <div>
                  Do you want to submit this timesheet?
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
              <Button type="primary">Submit</Button>
            </Popconfirm> : 
            <Button type="primary" disabled>Submit</Button>
          }
        </div>
      </React.Fragment>
    );
  })
);
