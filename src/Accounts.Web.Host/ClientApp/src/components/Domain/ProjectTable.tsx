/** @jsx jsx */
import { Table, Icon, Dropdown, Menu } from 'antd';
import _ from 'lodash';
import { jsx, css } from '@emotion/core';
import moment from 'moment';
import { NavLink } from 'react-router-dom';
import { useHistory } from 'react-router';

const createProjectMenu = (projectId: number) => (
  <Menu>
    <Menu.Item>
      <NavLink to={`/projects/${projectId}/edit`}>Edit</NavLink>
    </Menu.Item>
  </Menu>
);

const columns = [
  {
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
    title: 'Consultant',
    dataIndex: 'consultantName',
    width: 140,
  },
  {
    title: 'Company',
    dataIndex: 'companyName',
    width: 140,
  },
  {
    title: 'End Client',
    dataIndex: 'endClient',
    width: 140,
  },
  {
    title: 'Start Date',
    dataIndex: 'startDt',
    render: (val: string) => moment(val).format('MM/DD/YYYY'),
    width: 120,
  },
  {
    title: 'End Date',
    dataIndex: 'endDt',
    render: (val: string) => val && moment(val).format('MM/DD/YYYY'),
    width: 120,
  },

  {
    title: 'Term',
    dataIndex: 'termName',
    width: 120,
  },
  {
    title: 'Invoice Cycle',
    dataIndex: 'invoiceCycleName',
    width: 120,
  },
  {
    title: 'Rate',
    dataIndex: 'rate',
    render: (val: string) => val && `$${val}`,
    width: 70,
  },
  {
    title: 'Total Hrs',
    dataIndex: 'totalHoursBilled',
    width: 70,
  },
  {
    title: 'Invoiced Amount',
    dataIndex: 'totalAmountBilled',
    render: (val: string) => val && `$ ${val.toLocaleString()}`,
    width: 120,
  },
  {
    width: 30,
    title: '',
    dataIndex: '',
    render: (data: any, record: any) => {
      return (
        <Dropdown overlay={createProjectMenu(record.id)}>
          <Icon type="ellipsis" rotate={90} style={{ cursor: 'pointer' }} />
        </Dropdown>
      );
    },
  },
];

export const ProjectTable = ({
  isLoading,
  predefinedQueries,
  onFilterChanged,
  dataSource,
  onSelectionChange,
  selectedProjectId,
  pagination,
  onChange,
  onRow,
}: any) => {
  const history = useHistory();

  return (
    <Table
      tableLayout="fixed"
      loading={isLoading}
      dataSource={dataSource}
      columns={columns}
      pagination={pagination}
      onChange={onChange}
      onRow={(record: any, rowIndex: any) => ({ onDoubleClick: () => history.push(`/projects/${record.id}/detail`) })}
    />
  );
};
