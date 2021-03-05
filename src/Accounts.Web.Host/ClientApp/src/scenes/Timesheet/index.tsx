/** @jsx jsx */
import './index.less';
import React from 'react';
import { inject, observer } from 'mobx-react';
import { Row, Col, Button, DatePicker, notification, Icon, Typography, Select, Card, Radio } from 'antd';
import moment, { Moment } from 'moment';
import HourLogEntryTable from './components/HourLogEntryTable';
import Stores from '../../stores/storeIdentifier';
import HourLogEntryStore from '../../stores/hourLogEntryStore';
import ProjectContextMenu from './components/ProjectContextMenu';
import { Portal } from 'react-portal';
import { RouteChildrenProps } from 'react-router';
import RouteableDrawer from '../../components/RouteableDrawer';
import NewTimesheet from './components/NewTimesheet';
import ConnectedTimesheetList from '../TimesheetReview/components/TimesheetList';
import TimesheetViewer from '../TimesheetReview/components/TimesheetViewer';
import AttachmentDrawer from './components/Attachments';
import ProjectSummary from './components/ProjectSummary';
import { jsx, css } from '@emotion/core';
import styled from '@emotion/styled';
import InvoiceDetail from '../../components/Domain/InvoiceDetail';
import CustomCancleButton from './../../components/Custom/CustomCancelButton';
import CustomButton from '../../components/Custom/CustomButton';

const { Text } = Typography;
const { Option } = Select;

// import * as Space from 'react-spaces';

const { RangePicker } = DatePicker;
export type ITimesheetProps = RouteChildrenProps & {
  hourLogEntryStore: HourLogEntryStore;
};

export interface ITimesheetState {
  startDt: Moment;
  endDt: Moment;
  selectedRowKeys: any;
  selectedProjectId: number | null;
  invoiceCycle: number;
  selectedQuery: number | null;
  range: number;
  isRangePickerOpen: boolean;
}

const fullDrawerBodyStyles = css`
  .ant-drawer-wrapper-body {
    display: flex;
    flex-direction: column;
    .ant-drawer-body {d
      flex: 1;
    }
  }
`;

const StyledRadioGroup = styled(Radio.Group)`
  .ant-radio-button-wrapper {
    margin: 0 4px;
    color: #2680eb !important;
    :hover {
      color: #fff !important;
      background: #2680eb !important;
    }
  }
  .ant-radio-button-wrapper-checked {
    color: #fff !important;
    background: #2680eb !important;
  }
`;

@inject(Stores.HourLogEntryStore)
@observer
class Timesheet extends React.Component<ITimesheetProps, ITimesheetState> {
  constructor(props: ITimesheetProps) {
    super(props);

    const startDt = this.setStartDate();
    this.state = {
      startDt,
      endDt: startDt.clone().endOf('month'),
      selectedRowKeys: [],
      selectedProjectId: null,
      invoiceCycle: 3,
      selectedQuery: null,
      range: 0,
      isRangePickerOpen: false,
    };
    this.handleSave = this.handleSave.bind(this);
    this.onDateSelectionChange = this.onDateSelectionChange.bind(this);
    this.getAll = this.getAll.bind(this);
  }

  async componentDidMount() {
    await this.getAll();
  }

  async getAll() {
    const { startDt, endDt } = this.state;
    await this.props.hourLogEntryStore.getAll({ startDt: startDt.format('MM/DD/YYYY'), endDt: endDt.format('MM/DD/YYYY') });
  }

  handleClick = async (e: any, data: any) => {
    const projectId = data.projectId;
    switch (data.item) {
      case 1:
        await this.props.hourLogEntryStore.saveHourLogEntries([projectId]);
        notification.open({
          message: 'Success',
          description: 'Hour Logs successfully saved.',
        });
        await this.getAll();
        break;
      case 2:
        this.setState({
          selectedProjectId: projectId,
        });
        this.props.history.push(`${this.props.match!.path}/${projectId}/attachments`);
        break;
      case 3:
        this.setState({
          selectedProjectId: projectId,
        });
        this.props.history.push(`${this.props.match!.path}/${projectId}/new`);
        break;
      case 4:
        this.setState({
          selectedProjectId: projectId,
        });
        this.props.history.push(`${this.props.match!.path}/${projectId}/timesheets`);
        break;
    }
  };

  onDateSelectionChange(dates: any) {
    this.setState(
      {
        startDt: dates[0],
        endDt: dates[1],
      },
      () => this.getAll()
    );
  }

  onSelectChange = (selectedRowKeys: any) => {
    this.setState({ selectedRowKeys });
  };

  onRangeToggle = (e: any) => {
    this.setState({ range: e.target.value, isRangePickerOpen: false });
    switch (e.target.value) {
      case 1:
        this.onDateSelectionChange([moment(), moment()]);
        break;
      case 2:
        this.onDateSelectionChange([moment().subtract(1, 'days'), moment().subtract(1, 'days')]);
        break;
      case 3:
        this.onDateSelectionChange([moment().subtract(6, 'days'), moment()]);
        break;
      case 4:
        this.onDateSelectionChange([moment().subtract(29, 'days'), moment()]);
        break;
      case 5:
        this.onDateSelectionChange([moment().startOf('month'), moment().endOf('month')]);
        break;
      case 6:
        this.onDateSelectionChange([moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]);
        break;
    }
  };

  changeRange(direction: number) {
    const { startDt, endDt, invoiceCycle } = this.state;
    let dates = [startDt, endDt];

    function shiftDateRange(val: any, type: string, direction: number, unitOfTime: any) {
      const methodName = direction == 1 ? 'add' : 'subtract';
      const newStartDt = direction == 1 ? endDt.clone().add(1, 'days') : startDt.clone()[methodName](val, type);
      const newEnddt = direction == 1 ? endDt.clone()[methodName](val, type).endOf(unitOfTime) : startDt.clone().subtract(1, 'days');
      return [newStartDt, newEnddt];
    }
    switch (invoiceCycle) {
      case 1:
        dates = shiftDateRange(1, 'weeks', direction, 'week');
        break;
      case 2:
        dates = shiftDateRange(2, 'weeks', direction, 'week');
        break;
      case 3:
        dates = shiftDateRange(1, 'months', direction, 'month');
        break;
    }
    this.onDateSelectionChange(dates);
  }

  handleSave = (row: any) => {
    console.log(this.props);
    console.log(row);
  };

  saveProjects = async () => {
    await this.props.hourLogEntryStore.saveHourLogEntries(this.state.selectedRowKeys);
    notification.open({
      message: 'Success',
      description: 'Hour Logs successfully saved.',
    });
    this.setState(
      {
        selectedRowKeys: [],
      },
      () => this.getAll()
    );
  };

  setStartDate = () => {
    if (moment().date() <= 7) {
      return moment().startOf('month').subtract(1, 'month');
    } else {
      return moment().startOf('month');
    }
  };

  handleSelect = (value: number) => {
    this.setState({ selectedQuery: value });
  };

  render() {
    const { startDt, endDt, selectedRowKeys, selectedQuery } = this.state;
    const { projectHourLogEntries } = this.props.hourLogEntryStore;
    let filteredProjectHourLogEntries;
    const hasSelected = selectedRowKeys.length > 0;
    if (selectedQuery == 0) {
      filteredProjectHourLogEntries =
        projectHourLogEntries &&
        projectHourLogEntries.filter((data: any) => {
          return data.project.pastTimesheetDays == 0;
        });
    } else if (selectedQuery == 1) {
      filteredProjectHourLogEntries =
        projectHourLogEntries &&
        projectHourLogEntries.filter((data: any) => {
          return data.project.pastTimesheetDays > 0 && data.project.pastTimesheetDays < 7;
        });
    } else if (selectedQuery == 2) {
      filteredProjectHourLogEntries =
        projectHourLogEntries &&
        projectHourLogEntries.filter((data: any) => {
          return data.project.pastTimesheetDays > 7;
        });
    } else {
      filteredProjectHourLogEntries = projectHourLogEntries;
    }
    return (
      <Card style={{ height: '100vh' }}>
        <div style={{ marginBottom: 16 }}>
          <h1>HOUR LOGS</h1>
          <Row type="flex" justify="space-between" align="middle">
            <Col>
              {hasSelected ? (
                <CustomButton type="primary" style={{ marginRight: 10 }} onClick={this.saveProjects}>
                  Save Entries
                </CustomButton>
              ) : (
                <CustomCancleButton>Save Entries</CustomCancleButton>
              )}
              <span style={{ marginLeft: 8 }}>{hasSelected ? `Selected ${selectedRowKeys.length} items` : ''}</span>
            </Col>
            <Col>
              <Row type="flex" align="middle">
                <Col>
                  <Select
                    size="large"
                    allowClear={true}
                    style={{ width: '200px', marginRight: '20px' }}
                    placeholder="Search Query"
                    onChange={this.handleSelect}
                  >
                    <Option value="0">Timesheet Created</Option>
                    <Option value="1">{'Past Due < 7D'}</Option>
                    <Option value="2">{'Past Due > 7D'}</Option>
                  </Select>
                </Col>
                <Col>
                  <Button size="large" type="default" onClick={() => this.changeRange(-1)}>
                    <Icon type="left" />
                  </Button>
                </Col>
                <Col>
                  <RangePicker
                    size="large"
                    allowClear={false}
                    open={this.state.isRangePickerOpen}
                    onOpenChange={() => this.setState({ isRangePickerOpen: !this.state.isRangePickerOpen })}
                    renderExtraFooter={() => (
                      <StyledRadioGroup value={this.state.range} onChange={this.onRangeToggle} size="small">
                        <Radio.Button value={1}>Today</Radio.Button>
                        <Radio.Button value={2}>Yesterday</Radio.Button>
                        <Radio.Button value={3}>Last 7 Days</Radio.Button>
                        <Radio.Button value={4}>Last 30 Days</Radio.Button>
                        <Radio.Button value={5}>This Month</Radio.Button>
                        <Radio.Button value={6}>Last Month</Radio.Button>
                      </StyledRadioGroup>
                    )}
                    value={[startDt, endDt]}
                    onChange={(dates: any) => {
                      this.onDateSelectionChange(dates);
                      this.setState({ range: 0 });
                    }}
                  />
                </Col>
                <Col>
                  <Button size="large" type="default" onClick={() => this.changeRange(1)}>
                    <Icon type="right" />
                  </Button>
                </Col>
              </Row>
            </Col>
          </Row>
        </div>
        <HourLogEntryTable
          entries={filteredProjectHourLogEntries}
          startDt={startDt}
          endDt={endDt}
          onSelectChange={this.onSelectChange}
          handleSave={this.handleSave}
          selectedRowKeys={selectedRowKeys}
          baseUrl={this.props.match!.url}
        />
        <ProjectContextMenu handleClick={this.handleClick} />
        <Portal>
          <RouteableDrawer
            path={`${this.props.match!.path}/:projectId/new`}
            width={'25vw'}
            title={({
              history,
              match: {
                params: { projectId },
              },
            }: any) => (
              <Text>
                {'New Timesheet '}
                <Button
                  size="small"
                  onClick={() => {
                    history.push(`${this.props.match!.path}/${projectId}/new/timesheets`);
                  }}
                >
                  Show Previous Timesheets
                </Button>
              </Text>
            )}
          >
            {({
              match: {
                params: { projectId },
              },
              onClose,
            }: any) => <NewTimesheet projectId={projectId} onSave={this.getAll} onClose={onClose} />}
          </RouteableDrawer>
          <RouteableDrawer
            path={[`${this.props.match!.url}/:projectId/timesheets`, `${this.props.match!.url}/:projectId/new/timesheets`]}
            width={'25vw'}
            title="Timesheet List"
          >
            {(prop: any) => {
              const {
                match: {
                  params: { projectId },
                },
              } = prop;
              return <ConnectedTimesheetList projectId={projectId} />;
            }}
          </RouteableDrawer>
          <RouteableDrawer
            path={[
              `${this.props.match!.path}/:projectId/timesheets/:timesheetId`,
              `${this.props.match!.path}/:projectId/new/timesheets/:timesheetId`,
            ]}
            width={'50vw'}
            title="Timesheet Detail"
            exact
            css={css(fullDrawerBodyStyles)}
          >
            {({
              match: {
                params: { timesheetId, projectId },
              },
            }: any) => {
              return <TimesheetViewer timesheetId={timesheetId} path={`${this.props.match!.path}/${projectId}/timesheets/${timesheetId}`} />;
            }}
          </RouteableDrawer>
          <RouteableDrawer
            path={[`${this.props.match!.path}/:projectId/new/attachments`, `${this.props.match!.path}/:projectId/attachments`]}
            width={'50vw'}
            title="Attachments"
            exact
            css={css(fullDrawerBodyStyles)}
          >
            {({
              location: { search },
              match: {
                params: { projectId },
              },
            }: any) => {
              var queryParams = new URLSearchParams(search);
              return (
                <AttachmentDrawer
                  projectId={projectId}
                  enableTimesheetAttachment={queryParams.get('isAttachable') ? true : false}
                  shouldLoad={queryParams.get('isAttachable') ? false : true}
                />
              );
            }}
          </RouteableDrawer>
          <RouteableDrawer
            path={`${this.props.match!.path}/:projectId/timesheets/:timesheetId/invoice/:invoiceId`}
            width={'25vw'}
            title="Invoice Details"
          >
            {({
              match: {
                params: { invoiceId },
              },
              onClose,
            }: any) => <InvoiceDetail invoiceId={invoiceId} onClose={onClose} />}
          </RouteableDrawer>
          <RouteableDrawer path={`${this.props.match!.path}/projectsummary`} width={'75vw'} title="Project Summary">
            {() => <ProjectSummary />}
          </RouteableDrawer>
        </Portal>
      </Card>
    );
  }
}

export default Timesheet;
