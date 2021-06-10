import React, { useEffect } from 'react';
import RoutableTabs from '../../components/RoutableTabs';
import { Card, Row, Col } from 'antd';
import useAxios from '../../lib/axios/useAxios';
import moment from 'moment';

import ProjectInvoice from './components/ProjectInvoices';
import ProjectTimesheet from './components/ProjectTimesheets';
import ProjectUnassociatedHourLogs from './components/ProjectUnassociatedHourLogs';

const ProjectDetail = (props: any) => {
  const projectId = props.match.params.projectId;
  const [{ data }, makeRequest] = useAxios({
    url: 'api/services/app/Project/Get',
    params: { Id: projectId },
  });

  useEffect(() => {
    makeRequest({ params: { projectId } });
  }, [projectId]);

  const result = data && data.result;
  return (
    <Card>
      <Row type="flex" justify="space-between">
        <Col>
          <h1 className="companyName">{result && result.companyName}</h1>
          <h4 className="consultantName">{result && result.consultantName}</h4>
          <h4 className="dates">
            Start Date:
            {result && result.startDt !== null ? moment(result && result.startDt).format('MM/DD/YYYY') : '--'}
          </h4>
          <h4 className="dates">
            End Date:
            {result && result.endDt !== null ? moment(result && result.endDt).format('MM/DD/YYYY') : '--'}
          </h4>
        </Col>
        <Col>
          <Row type="flex" justify="space-between" gutter={10}>
            <Col>
              <Card className={'statCard hoursCard'}>
                <p>Hours Logged</p>
                <span>{result && result.totalHoursBilled} Hrs</span>
              </Card>
            </Col>
            <Col>
              <Card className={'statCard amountCard'}>
                <p>Amount Billed</p>
                <span>$ {result && result.totalAmountBilled}</span>
              </Card>
            </Col>
          </Row>
        </Col>
      </Row>

      <RoutableTabs
        {...props}
        routeConfig={[
          {
            label: 'Timesheets',
            getRoute: (url: string) => url,
            component: ProjectTimesheet,
          },
          {
            label: 'Invoices',
            getRoute: (url: string) => `${url}/invoices`,
            component: ProjectInvoice,
          },
          {
            label: 'Unassociated Hour Logs',
            getRoute: (url: string) => `${url}/unassociatedHourLogs`,
            component: ProjectUnassociatedHourLogs,
          },
        ]}
      />
    </Card>
  );
};

export default ProjectDetail;
