import React from 'react';

//@ts-ignore
import { PageHeader, Row, Col, Icon, Typography, Tooltip, Button, Descriptions } from 'antd';
//@ts-ignore
import moment from 'moment';

import TimesheetModel from '../../../models/Timesheet/timesheetModel';
//@ts-ignore
const { Text } = Typography;

export type ITimesheetHeaderProps = {
  timesheet: TimesheetModel;
};

export default class TimesheetHeader extends React.Component<ITimesheetHeaderProps> {
  render() {
    // const {
    //   timesheet: {
    //     project: { consultantName, companyName, startDt: projectStartDt, endDt: projectEndDt },
    //     startDt,
    //     endDt,
    //   },
    // } = this.props;
    return (
      // <Row type="flex" justify="space-between" gutter={10}>
      //   <Col>
      //     <h3>
      //       <Text>{consultantName}</Text>
      //     </h3>
      //   </Col>
      //   <Col>
      //     <Tooltip title="anujmalla@gmail.com">
      //       <Icon type="mail" className="mr-10" />
      //     </Tooltip>
      //     <Tooltip title="(510) 363-2406">
      //       <Icon type="phone" />
      //     </Tooltip>
      //   </Col>
      //   <Col>
      //     <Text code>
      //       {moment(startDt).format('MM/DD')} - {moment(endDt).format('MM/DD')}
      //     </Text>
      //   </Col>
      //   <Col>
      //     <h3>
      //       <Text type="secondary">{companyName}</Text>
      //     </h3>
      //   </Col>
      //   <Col>
      //     <Text code>
      //       {moment(projectStartDt).format('MM/DD')} {projectEndDt && <span>- {moment(projectEndDt).format('MM/DD')}</span>}
      //     </Text>
      //   </Col>
      // </Row>
      <PageHeader
        onBack={() => window.history.back()}
        title="Title"
        subTitle="This is a subtitle"
        extra={[
          <Button key="3">Operation</Button>,
          <Button key="2">Operation</Button>,
          <Button key="1" type="primary">
            Primary
          </Button>,
        ]}
      >
        <Descriptions size="small" column={3}>
          <Descriptions.Item label="Created">
            <Text>Lili Qu</Text>
          </Descriptions.Item>
          <Descriptions.Item label="Association">
            <a>421421</a>
          </Descriptions.Item>
          <Descriptions.Item label="Creation Time">
            <Text>2017-01-10</Text>
          </Descriptions.Item>
          <Descriptions.Item label="Effective Time">
            <Text>2017-10-10</Text>
          </Descriptions.Item>
          <Descriptions.Item label="Remarks">
            <Text>Gonghu Road, Xihu District, Hangzhou, Zhejiang, China</Text>
          </Descriptions.Item>
        </Descriptions>
      </PageHeader>
    );
  }
}
