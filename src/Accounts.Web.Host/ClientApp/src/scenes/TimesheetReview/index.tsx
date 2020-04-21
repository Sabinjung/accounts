/** @jsx jsx */
import './index.less';
import React, { useState, useEffect, useRef } from 'react';
import { Row, Col } from 'antd';
import { useRouteMatch } from 'react-router-dom';
import { TimesheetList } from './components/TimesheetList';
import TimesheetViewer from './components/TimesheetViewer';
// import { withAxios } from 'react-axios';
import _ from 'lodash';
import useAxios from '../../lib/axios/useAxios';
import { jsx, css } from '@emotion/core';
import { AxiosInstance, AxiosResponse } from 'axios';
import { Route, useHistory } from 'react-router';
import { Portal } from 'react-portal';
import RouteableDrawer from '../../components/RouteableDrawer';
import InvoiceDetail, { InvoiceDetailForm } from '../../components/Domain/InvoiceDetail';
import { Get } from '../../lib/axios';

interface ITimesheetReviewProps {
  axios: AxiosInstance;
  isLoading: boolean;
  response: AxiosResponse;
  makeRequest: (props: any) => void;
}

const TimesheetReview: React.FC<ITimesheetReviewProps> = () => {
  const [selectedFilter, setSelectedFilter] = useState<string>('Pending Apprv');
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/Timesheet/GetTimesheets',
    params: { isActive: true, name: selectedFilter },
  });
  const result = (data && data.result) || { results: [], recordCounts: [] };
  const { results: dataSource, recordCounts: predefinedQueries } = result;

  let history = useHistory();
  const match = useRouteMatch<{ timesheetId: string }>('/timesheets/:timesheetId');
  let selectedTimesheetId = '';
  if (match) {
    const { params } = match;
    if (params && params.timesheetId) {
      selectedTimesheetId = params.timesheetId;
    } else {
      const timesheetListItem: any = _.first(dataSource);
      if (timesheetListItem) {
        history.push(`/timesheets/${timesheetListItem.id}`);
      }
    }
  }

  const refetch = () => {
    makeRequest({ params: { name: selectedFilter, isActive: true } });
  };

  const isFirstRun = useRef(true);
  useEffect(() => {
    if (isFirstRun.current) {
      isFirstRun.current = false;
      return;
    }
    refetch();
  }, [selectedFilter]);

  const viewerRef = React.createRef<any>();

  const filterTimesheet = (startTime: any, endTime: any, consultantId: number, companyId: number) => {
    makeRequest({ params: { startTime: startTime, endTime: endTime, consultantId: consultantId, companyId: companyId } });
  }
  // debugger;

  return (
    <React.Fragment>
      <Row type="flex" style={{ height: '100%' }} gutter={8} justify="space-between">
        <Col span={6}>
          <TimesheetList
            onSelectionChange={(timesheetId: number) => history.push(`/timesheets/${timesheetId}`)}
            dataSource={dataSource}
            predefinedQueries={predefinedQueries}
            isLoading={loading}
            selectedTimesheetId={selectedTimesheetId}
            filterTimesheet={filterTimesheet}
            onFilterChanged={(filter: string) => {
              setSelectedFilter(filter);
            }}
          />
        </Col>
        <Col span={18} style={{ flex: 1 }}>
          <Route path="/timesheets/:timesheetId">
            {({ match }: any) => {
              if (!match)
                return (
                  <div
                    css={css`
                      height: 100%;
                      display: flex;
                      justify-content: center;
                      align-items: center;
                      background-color: white;
                    `}
                  >
                    Select Timesheet from the list
                  </div>
                );
              const { params } = match;
              if (!params || !params.timesheetId) return null;
              return (
                <TimesheetViewer
                  ref={viewerRef}
                  path={`/timesheets/${params.timesheetId}`}
                  timesheetId={params.timesheetId}
                  onTimesheetApproved={() => {
                    const selectedTimesheetIndex = dataSource.findIndex((t: any) => t.id === params.timesheetId);
                    const nextTimesheetItem = dataSource[selectedTimesheetIndex + 1];

                    if (nextTimesheetItem) {
                      history.push(`/timesheets/${nextTimesheetItem.id}`);
                    }
                    refetch();
                  }}
                  onTimesheetDeleted={() => {
                    const selectedTimesheetIndex = dataSource.findIndex((t: any) => t.id === params.timesheetId);
                    const nextTimesheetItem = dataSource[selectedTimesheetIndex + 1];

                    if (nextTimesheetItem) {
                      history.push(`/timesheets/${nextTimesheetItem.id}`);
                    }
                    refetch();
                  }}
                />
              );
            }}
          </Route>
        </Col>
      </Row>
      <Portal>
        {match && (
          <RouteableDrawer path={[`${match!.path}/invoice/:invoiceId`, `${match!.path}/generate`]} width={'50vw'} title="Invoice Detail">
            {({
              match: {
                params: { invoiceId, timesheetId },
              },
              onClose,
            }: any) => {
              if (invoiceId) {
                return (
                  <InvoiceDetail
                    invoiceId={invoiceId}
                    onClose={onClose}
                    onInvoiceSubmitted={() => {
                      refetch();
                      viewerRef.current && viewerRef.current.refetch();
                    }}
                  />
                );
              } else {
                return (
                  <Get url={'/api/services/app/Invoice/GenerateInvoice'} params={{ timesheetId }}>
                    {({ error, data, isLoading }: any) => {
                      const result = data && data.result;
                      return (
                        <InvoiceDetailForm
                          invoice={result}
                          onClose={onClose}
                          onInvoiceSubmitted={() => {
                            refetch();
                            viewerRef.current && viewerRef.current.refetch();
                          }}
                        />
                      );
                    }}
                  </Get>
                );
              }
            }}
          </RouteableDrawer>
        )}
      </Portal>
    </React.Fragment>
  );
};

export default TimesheetReview;
