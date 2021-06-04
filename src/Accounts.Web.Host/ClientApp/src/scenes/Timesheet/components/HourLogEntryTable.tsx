/** @jsx jsx */
import React from 'react';
import { Button, Icon, Input, Row, Col, Typography, Descriptions, Popover, Tooltip } from 'antd';
import { HourLogEntryRow, HourLogEntryInput } from './HourLogEntryInput';
import moment from 'moment';
import { ContextMenuTrigger } from 'react-contextmenu';
import Highlighter from 'react-highlight-words';
import { observer } from 'mobx-react';
import { jsx, css } from '@emotion/core';
import { withRouter } from 'react-router';
import CustomHoursTable from './../../../components/Custom/CustomHoursTable';
import CustomButton from '../../../components/Custom/CustomButton';
import styled from '@emotion/styled';

const { Text } = Typography;

const StyledProject = styled(Text)`
  color: #2a2a2a !important;
  font-weight: 600;
`;

var enumerateDaysBetweenDates = function (startDate: any, endDate: any) {
  var dates = [];

  var currDate = moment(startDate).subtract(1, 'day').startOf('day');
  var lastDate = moment(endDate).add(1, 'day').startOf('day');

  while (currDate.add(1, 'days').diff(lastDate) < 0) {
    dates.push(currDate.clone().toDate());
  }
  return dates;
};

function generateColumns(startDt: any, endDt: any, getColumnSearchProps: any) {
  const dates = enumerateDaysBetweenDates(startDt, endDt);
  return [
    {
      fixed: 'left',
      width: 30,
      render: ({ pastTimesheetDays }: any) => {
        if (pastTimesheetDays > 0 && pastTimesheetDays < 7)
          return (
            <Icon
              type="warning"
              title="Create timesheet"
              css={css`
                color: #b0b006;
              `}
            />
          );
        if (pastTimesheetDays > 7)
          return (
            <Icon
              type="warning"
              theme="filled"
              title="Past due by more than 7 days"
              css={css`
                color: red;
              `}
            />
          );
        return null;
      },
    },
    {
      title: 'Project',
      key: 'consultantName',
      fixed: 'left',
      width: 200,
      ...getColumnSearchProps(['consultantName', 'companyName']),
      render: (item: any) => {
        let mailTo = 'mailto:' + item.email;
        let content = (
          <Descriptions style={{ width: '200px' }}>
            <Descriptions.Item span={2}>
              <Text style={{ fontWeight: 'bold', fontSize: '16px' }}>{item.consultantName}</Text>
            </Descriptions.Item>
            <Descriptions.Item>
              <Tooltip
                title={
                  <Text copyable style={{ color: '#fff' }}>
                    {item.phoneNumber}
                  </Text>
                }
              >
                <Icon type="phone" theme="filled" style={{ color: '#1DA57A', marginRight: '10px', marginLeft: '20px' }} />
              </Tooltip>
              <Tooltip title={<a href={mailTo}>{item.email}</a>}>
                <Icon type="mail" theme="filled" style={{ color: '#1DA57A' }} />
              </Tooltip>
            </Descriptions.Item>
            <Descriptions.Item span={3}>{item.companyName}</Descriptions.Item>
            <Descriptions.Item label="Period" span={3}>
              {moment(item.projectStartDt).format('MM/DD/YYYY')} - {item.projectEndDt && moment(item.projectEndDt).format('MM/DD/YYYY')}
            </Descriptions.Item>
            <Descriptions.Item label="Invoice Cycle" span={3}>
              {item.invoiceCycleName}
            </Descriptions.Item>
            <Descriptions.Item label="Upcoming Timesheet" span={3}>
              {moment(item.upcomingTimesheetSummary.startDt).format('MM/DD/YYYY') >
              moment(item.upcomingTimesheetSummary.endDt).format('MM/DD/YYYY') ? (
                <span>The project has ended</span>
              ) : 
                (moment(item.upcomingTimesheetSummary.startDt).format('MM/DD/YYYY') + "-" + moment(item.upcomingTimesheetSummary.endDt).format('MM/DD/YYYY'))}
            </Descriptions.Item>
            <Descriptions.Item label="Upcoming Total Hours" span={3}>
              {item.upcomingTimesheetSummary.totalHrs} hrs
            </Descriptions.Item>
          </Descriptions>
        );
        return (
          <Popover placement="left" content={content} style={{ backgroundColor: '#000' }}>
            <div>
              <StyledProject>{item.consultantName}</StyledProject> <br />
              <Text type="secondary">{item.companyName}</Text>
            </div>
          </Popover>
        );
      },
    },
    ...dates.map((d: any) => ({
      title: moment(d).format('MM/DD'),
      dataIndex: 'hourLogEntries',
      width: 50,
      day: moment(d),
      editable: true,
      key: moment(d),
      className: moment(d).isoWeekday() === 6 || moment(d).isoWeekday() === 7 ? 'is-holiday' : '',
    })),

    {
      title: 'Hrs',
      dataIndex: 'totalHrs',
      className: 'total-hrs',
      width: 50,
      fixed: 'right',
    },
    {
      width: 30,
      fixed: 'right',
      render: (data: any) => {
        return (
          <ContextMenuTrigger
            id="SIMPLE"
            holdToDisplay={1}
            collect={() => ({
              projectId: data.key,
            })}
          >
            <Icon type="ellipsis" rotate={90} style={{ cursor: 'pointer' }} />
          </ContextMenuTrigger>
        );
      },
    },
  ];
}

@observer
class HourLogEntryTable extends React.Component<any, any> {
  columns: any = [];
  searchInput: any;

  constructor(props: any) {
    super(props);
    debugger;
    this.state = {
      loading: false,
    };
  }

  getColumnSearchProps = (dataIndex: any) => ({
    filterDropdown: ({ setSelectedKeys, selectedKeys, confirm, clearFilters }: any) => (
      <div style={{ padding: 8 }}>
        <Input
          ref={(node: any) => {
            this.searchInput = node;
          }}
          placeholder={`Search`}
          value={selectedKeys[0]}
          onChange={(e: any) => setSelectedKeys(e.target.value ? [e.target.value] : [])}
          onPressEnter={() => this.props.handleSearch(selectedKeys, confirm)}
          style={{ width: 188, marginBottom: 8, display: 'block' }}
        />
        <Button
          type="primary"
          onClick={() => this.props.handleSearch(selectedKeys, confirm)}
          icon="search"
          size="small"
          style={{ width: 90, marginRight: 8 }}
        >
          Search
        </Button>
        <Button onClick={() => this.props.handleReset(clearFilters)} size="small" style={{ width: 90 }}>
          Reset
        </Button>
      </div>
    ),
    filterIcon: (filtered: any) => <Icon type="search" style={{ color: filtered ? '#1890ff' : undefined }} />,
    onFilter: (value: any, record: any) =>
      dataIndex
        .reduce((s: any, di: any) => record[s].concat(record[di], ''))
        .toString()
        .toLowerCase()
        .includes(value.toLowerCase()),
    onFilterDropdownVisibleChange: (visible: any) => {
      if (visible) {
        setTimeout(() => this.searchInput.select());
      }
    },
    render: (text: any) => (
      <Highlighter
        highlightStyle={{ backgroundColor: '#ffc069', padding: 0 }}
        searchWords={[this.state.searchText]}
        autoEscape
        textToHighlight={text.toString()}
      />
    ),
  });

  render() {
    const { entries, recordCount, pagination, loading, startDt, endDt, selectedRowKeys, onSelectChange, onChange, handleSave, history, baseUrl } = this.props;
    if (entries == null) return null;
    const components = {
      body: {
        row: HourLogEntryRow,
        cell: HourLogEntryInput,
      },
    };

    const columns = generateColumns(startDt, endDt, this.getColumnSearchProps).map((col: any) => {
      if (!col.editable) {
        return col;
      }
      return {
        ...col,
        onCell: (record: any) => ({
          projectId: record.key,
          editable: col.editable,
          dataIndex: col.dataIndex,
          title: col.title,
          day: col.day,
          key: col.day,
          projectStartDt: record.projectStartDt,
          projectEndDt: record.projectEndDt,
          upcomingTimesheetSummary: record.upcomingTimesheetSummary,
          pastTimesheetDays: record.pastTimesheetDays,
          hourEntry: record[col.dataIndex].find((x: any) => moment(x.day).format('MM/DD/YYYY') === col.day.format('MM/DD/YYYY')),
          handleSave: handleSave,
          createHourLogEntry: record.createHourLogEntry,
        }),
      };
    });

    const dataSource = entries.map((e: any, index: any) => {
      return {
        key: e.project.id,
        consultantName: e.project.consultantName,
        companyName: e.project.companyName,
        email: e.project.email,
        phoneNumber: e.project.phoneNumber,
        invoiceCycleName: e.project.invoiceCycleName,
        hourLogEntries: e.hourLogEntries,
        totalHrs: e.totalHrs,
        projectStartDt: e.project.startDt,
        projectEndDt: e.project.endDt,
        upcomingTimesheetSummary: e.project.upcomingTimesheetSummary,
        createHourLogEntry: e.createHourLogEntry,
        pastTimesheetDays: e.project.pastTimesheetDays,
      };
    });

    const rowSelection = {
      selectedRowKeys,
      onChange: onSelectChange,
    };
    return (
      <div>
        <CustomHoursTable
          rowSelection={rowSelection}
          components={components}
          loading={loading}
          rowClassName={() => 'editable-row'}
          dataSource={dataSource}
          columns={columns}
          size="small"
          scroll={{ x: 470 }}
          pagination={pagination}
          onChange={onChange}
          footer={() => (
            <Row type="flex" justify="space-between" align="bottom">
              <Col>Total Consultants: {recordCount}</Col>
              <Col>
                <CustomButton type="primary" onClick={() => history.push(`${baseUrl}/projectsummary`)}>
                  Project Summary
                </CustomButton>
              </Col>
            </Row>
          )}
        />
      </div>
    );
  }
}

export default withRouter(HourLogEntryTable);
