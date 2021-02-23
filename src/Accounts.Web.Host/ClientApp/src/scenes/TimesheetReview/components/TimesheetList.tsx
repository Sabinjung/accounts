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
import PredefinedQueryPills from '../../../components/PredefinedQueryPills';
import { AutoSizer } from 'react-virtualized';
import styled from '@emotion/styled';

const { Text } = Typography;

const { RangePicker } = DatePicker;

export type ITimesheetListProps = {
  timesheetStore?: TimesheetStore;
  onSelectionChange?: (timesheetId: number) => void;
  projectId?: number;
  reload?: boolean;
};

const StyledText = styled(Text)`
  margin-right: 3px;
`;

const StyledForm = styled(Form)`
  .ant-form-item {
    margin-bottom: 0 !important;
  }

  .ant-form-item-label {
    line-height: unset;
  }
`;

const StyledRow = styled(Row)`
  margin-bottom: 5px;
`;

const StyledSearch = styled(Button)`
  color: #7034bd;
  font-size: 22px;
  :hover,
  :active,
  :focus {
    color: #7034bd;
  }
`;

const timesheetListStyles = (theme: any) => ({
  'overflow-y': 'auto',
  'overflow-x': 'hidden',
  '.ant-list-item': {
    padding: '16px 22px',
    background: '#fff',
    marginBottom: '10px',
    borderRadius: '20px',
    border: 'none',
    color: 'black',
    '&:hover': {
      cursor: 'pointer',
      background: '#DFEAFA',
    },
    '&.active': {
      background: '#DFEAFA',
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
  selectedFilter,
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

  const searchFilters = () => (
    <Popover
      trigger="click"
      visible={visible}
      onVisibleChange={() => setVisible(!visible)}
      content={
        <StyledForm layout="horizontal">
          <Form.Item label="Period">
            <RangePicker
              style={{ width: '300px' }}
              onChange={handleDateRange}
              ranges={{
                Today: [moment(), moment()],
                Yesterday: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
              }}
            />
          </Form.Item>
          <Form.Item label="Consultant">
            <EntityPicker
              url="/api/services/app/Consultant/Search"
              mapFun={(r) => ({ value: r.id, text: `${r.firstName} ${r.lastName}` })}
              value={consultantId}
              onChange={(value) => {
                setConsultantId(value);
              }}
            />
          </Form.Item>
          <Form.Item label="Company">
            <EntityPicker
              url="/api/services/app/Company/Search"
              mapFun={(r) => ({ value: r.id, text: r.displayName })}
              value={companyId}
              onChange={(value) => {
                setCompanyId(value);
              }}
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
        </StyledForm>
      }
      title="Filters"
    >
      <StyledSearch type="link" shape="circle" icon="search" />
    </Popover>
  );

  return (
    <ClassNames>
      {({ css }) => (
        <AutoSizer>
          {({ height, width }) => (
            <List
              css={timesheetListStyles}
              split={false}
              style={{ height, width, minHeight: 500 }}
              loading={isLoading}
              header={
                <PredefinedQueryPills
                  selectedFilter={selectedFilter}
                  size="small"
                  dataSource={predefinedQueries}
                  onClick={(name) => onFilterChanged(name)}
                  searchFilters={searchFilters}
                />
              }
              dataSource={dataSource}
              renderItem={(timesheet: any) => (
                <List.Item
                  onClick={() => {
                    onSelectionChange && onSelectionChange(timesheet.id);
                  }}
                  className={classNames({ active: selectedTimesheetId == timesheet.id })}
                >
                  <div style={{ width: '100%' }}>
                    <StyledRow type="flex" justify="space-between" align="middle">
                      <Col style={{ fontWeight: 600 }}>{timesheet.project.consultantName}</Col>
                      <Col>
                        <Text>
                          {moment(timesheet.startDt).format('MM/DD')} - {moment(timesheet.endDt).format('MM/DD')}
                        </Text>
                      </Col>
                    </StyledRow>
                    <Row type="flex" justify="space-between" align="middle">
                      <Col>
                        <Text>{timesheet.project.companyName}</Text>
                      </Col>
                      <Col>
                        {timesheet.statusId == 1 ? (
                          <StyledText code className={classNames({ danger: moment().diff(moment(timesheet.createdDt), 'days') > 5 })}>
                            {moment(timesheet.createdDt).fromNow()}
                          </StyledText>
                        ) : (
                          <StyledText code>{moment(timesheet.createdDt).fromNow()}</StyledText>
                        )}

                        <Text style={{ color: '#008dff' }}>{timesheet.totalHrs} hrs</Text>
                      </Col>
                    </Row>
                    <Row type="flex" justify="space-between" align="middle">
                      <Col>
                        {timesheet.qboInvoiceId && (
                          <Text>
                            eTrans ID:
                            {' ' + timesheet.qboInvoiceId}
                          </Text>
                        )}
                      </Col>
                      <Col>
                        {timesheet.eInvoiceId && (
                          <Text>
                            eInvoice ID:
                            <a href={`https://c70.qbo.intuit.com/app/invoice?txnId=${timesheet.qboInvoiceId}`} target="_blank">
                              {'  ' + timesheet.eInvoiceId}
                            </a>
                          </Text>
                        )}
                      </Col>
                    </Row>
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

const ConnectedTimesheetList: React.FunctionComponent<ITimesheetListProps> = (props) => {
  const { onSelectionChange, projectId } = props;
  const [selectedFilter, setSelectedFilter] = useState('Pending Apprv');
  return (
    <Get url="api/services/app/Timesheet/GetTimesheets" params={{ name: selectedFilter, isActive: true, projectId }}>
      {({ data, loading }: any) => {
        const result = (data && data.result) || { results: [], recordCounts: [] };
        const { results: dataSource, recordCounts: predefinedQueries } = result;

        return (
          <TimesheetList
            dataSource={dataSource}
            predefinedQueries={predefinedQueries}
            onSelectionChange={onSelectionChange}
            isLoading={loading}
            selectedFilter={selectedFilter}
            onFilterChanged={(filter: string) => setSelectedFilter(filter)}
          />
        );
      }}
    </Get>
  );
};

export default ConnectedTimesheetList;
