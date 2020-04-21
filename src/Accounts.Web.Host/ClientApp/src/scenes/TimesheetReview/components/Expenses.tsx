import React, { Fragment, useEffect } from 'react';
// import { Portal } from 'react-portal';

import { Button, Row, Col, Table } from 'antd';

import RouteableDrawer from '../../../components/RouteableDrawer';
import ExpenseCreateUpdate from './ExpenseCreateUpdate';
import { useHistory } from 'react-router';
import moment from 'moment';
import useAxios from '../../../lib/axios/useAxios';
import { Get } from '../../../lib/axios';
import Authorize from '../../../components/Authorize';
import ConfirmActionbutton from '../../../components/ConfirmActionButton';



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
            dataIndex: 'expenseTypeId',
        },
        {
            title: 'Date',
            dataIndex: 'reportDt',
            render: (val: string) => moment(val).format('MM/DD/YYYY'),
        },
        {
            title: 'Amount',
            dataIndex: 'amount',
        },
        {
            title: 'Comment',
            dataIndex: 'comment',
        },
        {
            title: 'Actions',
            render: (data: any, record: any) => (
                <Fragment>
                    <Button type="primary" icon="edit" style={{ marginRight: '8px' }} onClick={() => { history.push(`${path.path}/Expense/${record.id}/edit`) }} />
                    <ConfirmActionbutton
                        url="/api/services/app/Expense/Delete"
                        params={{ id: record.id }}
                        method="Delete"
                        type="danger"
                        icon="delete"
                        onSuccess={() => {
                            makeRequest({})
                        }}
                        permissions={['Timesheet.Delete']}
                    >
                        {() => {
                            return (
                                <div>
                                    Do you want to delete this Expense?
                                </div>
                            );
                        }}
                    </ConfirmActionbutton>
                </Fragment>
            )
        }

    ];

    return (
        <Fragment>
            <Row style={{ margin: '15px 15px 15px 3px' }}>
                <Col>
                    <Button type="primary" icon="plus" onClick={() => history.push(`${path.path}/Expense/new`)}>Add</Button>
                </Col>
            </Row>

            <Row style={{ marginTop: 20 }}>
                <Col
                    xs={{ span: 24, offset: 0 }}
                    sm={{ span: 24, offset: 0 }}
                    md={{ span: 24, offset: 0 }}
                    lg={{ span: 24, offset: 0 }}
                    xl={{ span: 24, offset: 0 }}
                    xxl={{ span: 24, offset: 0 }}
                ></Col>
                <Col>
                    <Table
                        loading={loading}
                        dataSource={dataSource}
                        columns={columns}
                        pagination={false}
                    />
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
