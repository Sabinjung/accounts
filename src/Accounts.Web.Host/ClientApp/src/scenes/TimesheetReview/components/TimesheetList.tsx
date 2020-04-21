/** @jsx jsx */
import React, { useState } from 'react';
import { Button, Row, Col, List, Typography, DatePicker, Form, Popover, Checkbox } from 'antd';
import moment from 'moment';
import _ from 'lodash';
import classNames from 'classnames';
import { jsx, ClassNames } from '@emotion/core';
import { Get } from '../../../lib/axios';
import TimesheetStore from '../../../stores/timesheetStore';
import EntityPicker from '../../../components/EntityPicker';
import PredefinedQueryPills from "../../../components/PredefinedQueryPills";
import styles from './timesheet.module.scss';
import { AutoSizer } from 'react-virtualized';

console.log(styles.title);
const { Text } = Typography;

const { RangePicker } = DatePicker;

export type ITimesheetListProps = {
  timesheetStore?: TimesheetStore;
  onSelectionChange?: (timesheetId: number) => void;
  projectId?: number;
  reload?: boolean;
};

const timesheetListStyles = (theme: any) => ({
  'overflow-y': 'auto',
  'overflow-x': 'hidden',
  '.ant-list-item': {
    padding: '5px 10px',
    color: 'black',
    '&:hover': {
      cursor: 'pointer',
      background: '#e8e8e8',
    },
    '&.active': {
      background: '#d9d9d9',
    },
    '.ant-list-item-meta-title,.ant-list-item-meta-description': {
      color: theme.colors.secondary,
    },
  },
});

export const TimesheetList = ({
  isLoading,
  predefinedQueries,
  onFilterChanged,
  dataSource,
  onSelectionChange,
  selectedTimesheetId,
  filterTimesheet,
}: any) => {
  const [startTime, setStartTime] = useState();
  const [consultantId, setConsultantId] = useState();
  const [companyId, setCompanyId] = useState();
  const [endTime, setEndTime] = useState();
  const [visible, setVisible] = useState(false);

  const handleDateRange = (dates: any, dateString: any) => {
    setStartTime(dateString[0]);
    setEndTime(dateString[1]);
  };

  return (
    <ClassNames>
      {({ css }) => (
        <AutoSizer>
          {({ height, width }) => (
            <List
              css={timesheetListStyles}
              size="small"
              style={{ height, width, minHeight: 500 }}
              loading={isLoading}
              header={
                <Row gutter={10} type="flex" justify="space-between">
                  <Col>
                    <PredefinedQueryPills className="highlight-tab" size="small" dataSource={predefinedQueries} onClick={(name) => onFilterChanged(name)} />
                  </Col>
                  <Col>
                    <Popover
                      trigger="click"
                      visible={visible}
                      onVisibleChange={() => setVisible(!visible)}
                      content={
                        <Form layout="horizontal" className="timesheet-filters">
                          <Form.Item label="Period">
                            <RangePicker
                              onChange={handleDateRange}
                              ranges={{
                                Today: [moment(), moment()],
                                Yesterday: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                                'This Month': [moment().startOf('month'), moment().endOf('month')],
                                'Last Month': [
                                  moment()
                                    .subtract(1, 'month')
                                    .startOf('month'),
                                  moment()
                                    .subtract(1, 'month')
                                    .endOf('month'),
                                ],
                              }}
                            />
                          </Form.Item>
                          <Form.Item label="Consultant">
                            <EntityPicker
                              url="/api/services/app/Consultant/Search"
                              mapFun={r => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
                              value={consultantId}
                              onChange={(value) => { setConsultantId(value) }}
                            />
                          </Form.Item>
                          <Form.Item label="Company">
                            <EntityPicker
                              url="/api/services/app/Company/Search"
                              mapFun={r => ({ value: r.id, text: r.displayName })}
                              value={companyId}
                              onChange={(value) => { setCompanyId(value) }}
                            />
                          </Form.Item>
                          <Form.Item>
                            <Checkbox>Include Timesheet Not Created</Checkbox>
                          </Form.Item>
                          <Form.Item>
                            <Button
                              type="primary"
                              onClick={() => {
                                filterTimesheet(startTime, endTime, consultantId, companyId);
                                setVisible(false);
                              }}
                            >
                              Apply
                            </Button>
                          </Form.Item>
                        </Form>
                      }
                      title="Filters"
                    >
                      <Button type="link" shape="circle" icon="search" />
                    </Popover>
                  </Col>
                </Row>
              }
              dataSource={dataSource}
              renderItem={(timesheet: any) => (
                <List.Item
                  onClick={() => {
                    onSelectionChange && onSelectionChange(timesheet.id);
                  }}
                  className={classNames({ active: selectedTimesheetId == timesheet.id })}
                >
                  <List.Item.Meta
                    title={timesheet.project.consultantName}
                    description={
                      <div style={{ display: 'flex', flexDirection: 'column' }}>
                        <Text>{timesheet.project.companyName}</Text>
                        {timesheet.qboInvoiceId && (
                          <Text>
                            Intuit ID:
                            <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${timesheet.qbInvoiceId}`} target="_blank">
                              {'  ' + timesheet.qboInvoiceId}
                            </a>
                          </Text>
                        )}
                      </div>
                    }
                  />
                  <div>
                    <div style={{ textAlign: 'right' }}>
                      {moment(timesheet.startDt).format('MM/DD')} - {moment(timesheet.endDt).format('MM/DD')}
                    </div>
                    <div>
                      {timesheet.statusId == 1 ? (
                        <Text code className={classNames({ danger: moment().diff(moment(timesheet.createdDt), 'days') > 5 })}>
                          {moment.utc(timesheet.createdDt).fromNow()}
                        </Text>
                      ) : (
                          <Text code>{moment.utc(timesheet.createdDt).fromNow()}</Text>
                        )}

                      <Text style={{ color: '#008dff' }}>{timesheet.totalHrs} hrs</Text>
                    </div>
                  </div>
                </List.Item>
              )}
            />
          )}
        </AutoSizer>
      )}
    </ClassNames>
  );
};

const ConnectedTimesheetList: React.FunctionComponent<ITimesheetListProps> = props => {
  const { onSelectionChange, projectId } = props;
  const [selectedFilter, setSelectedFilter] = useState('Pending Apprv');

  return (
    <Get url="api/services/app/Timesheet/GetTimesheets" params={{ name: selectedFilter, isActive: true, projectId }}>
      {({ error, data, isLoading }: any) => {
        const result = (data && data.result) || { results: [], recordCounts: [] };
        const { results: dataSource, recordCounts: predefinedQueries } = result;

        return (
          <TimesheetList
            dataSource={dataSource}
            predefinedQueries={predefinedQueries}
            onSelectionChange={onSelectionChange}
            isLoading={isLoading}
            onFilterChanged={(filter: any) => setSelectedFilter(filter)}
          />
        );
      }}
    </Get>
  );
};

export default ConnectedTimesheetList;
