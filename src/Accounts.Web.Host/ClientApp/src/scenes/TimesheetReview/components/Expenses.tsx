import React, { Fragment, useEffect } from 'react';
// import { Portal } from 'react-portal';

import { Row, Col } from 'antd';

import RouteableDrawer from '../../../components/RouteableDrawer';
import ExpenseCreateUpdate from './ExpenseCreateUpdate';
import { useHistory } from 'react-router';
import moment from 'moment';
import useAxios from '../../../lib/axios/useAxios';
import { Get } from '../../../lib/axios';
import Authorize from '../../../components/Authorize';
import ConfirmActionbutton from '../../../components/ConfirmActionButton';
import CustomButton from '../../../components/Custom/CustomButton';
import CustomEditButton from './../../../components/Custom/CustomEditButton';
import CustomTable from '../../../components/Custom/CustomTable';

export default (path: any, timesheetId: number) => {
  const history = useHistory();
  const [{ data, loading }, makeRequest] = useAxios({
    url: '/api/services/app/Expense/Search',
    params: { isActive: true, timesheetId: path.timesheetId },
  });
  const result = (data && data.result) || { results: [] };
  const { results: dataSource } = result;

  useEffect(() => {
    makeRequest({});
  }, []);

  const columns = [
    {
      title: 'Expense Type',
      dataIndex: 'expenseTypeName',
      key:"expenseType"
    },
    {
      title: 'Date',
      dataIndex: 'reportDt',
      render: (val: string) => moment(val).format('MM/DD/YYYY'),
      key:"date"
    },
    {
      title: 'Amount',
      dataIndex: 'amount',
      key:"amount"
    },
    {
      title: 'Comment',
      dataIndex: 'comment',
      key:"comment"
    },
    {
      title: 'Actions',
      key:"actions",
      render: (data: any, record: any) => (
        <Fragment>
          <CustomEditButton
            onClick={() => {
              history.push(`${path.path}/Expense/${record.id}/edit`);
            }}
          />
          <ConfirmActionbutton
            url="/api/services/app/Expense/Delete"
            params={{ id: record.id }}
            style={{background:"#ff00001a", color:"#ff0000", border:"none"}}
            method="Delete"
            type="danger"
            icon="delete"
            onSuccess={() => {
              makeRequest({});
            }}
            permissions={['Timesheet.Delete']}
          >
            {() => {
              return <div>Do you want to delete this Expense?</div>;
            }}
          </ConfirmActionbutton>
        </Fragment>
      ),
    },
  ];

  return (
    <Fragment>
      <Row style={{ margin: '15px 15px 15px 3px' }} type="flex" align="middle" justify="space-between">
        <Col>
          <h2 style={{ fontWeight: 600 }}>Expenses</h2>
        </Col>
        <Col>
          <CustomButton type="primary" icon="plus" onClick={() => history.push(`${path.path}/Expense/new`)}>
            Add Expenses
          </CustomButton>
        </Col>
      </Row>

      <Row style={{ marginTop: 20 }}>
        <Col>
          <CustomTable loading={loading} dataSource={dataSource} columns={columns} pagination={false} />
        </Col>
      </Row>
      <Authorize permissions={['Expense.Create']}>
        <RouteableDrawer path={[`${path.path}/Expense/new`]} width={'25vw'} title="Expense">
          {({ onClose }: any) => {
            return (
              <ExpenseCreateUpdate
                onClose={onClose}
                timesheetId={path.timesheetId}
                onExpenseAdded={() => {
                  onClose();
                  makeRequest({});
                }}
              />
            );
          }}
        </RouteableDrawer>
      </Authorize>
      <Authorize permissions={['Expense.Update']}>
        <RouteableDrawer path={[`${path.path}/Expense/:id/edit`]} width={'25vw'} title="Expense">
          {({
            match: {
              params: { id },
            },
            onClose,
          }: any) => {
            return (
              <Get url="/api/services/app/Expense/Get" params={{ id: id }}>
                {({ data }: any) => {
                  return (
                    <ExpenseCreateUpdate
                      expense={data && data.result}
                      onClose={onClose}
                      timesheetId={path.timesheetId}
                      onExpenseAdded={() => {
                        onClose();
                        makeRequest({});
                      }}
                    />
                  );
                }}
              </Get>
            );
          }}
        </RouteableDrawer>
      </Authorize>
    </Fragment>
  );
};
