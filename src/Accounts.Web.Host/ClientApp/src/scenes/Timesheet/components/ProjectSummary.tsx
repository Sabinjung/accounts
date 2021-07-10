import React, { useState } from 'react';
import { Button, Icon, Input, Typography } from 'antd';
import Highlighter from 'react-highlight-words';
import moment, { Moment } from 'moment';
import { Get } from '../../../lib/axios';
import styled from '@emotion/styled';
import CustomTable from '../../../components/Custom/CustomTable';

const { Text } = Typography;

const StyledTable = styled(CustomTable)`
  .ant-table-small {
    border: none;
  }
  .ant-table-column-has-actions {
    .ant-table-header-column {
      margin-top: 0px;
    }
  }
  .ant-table-header-column {
    margin-top: 22px;
  }
`;

const StyledProject = styled(Text)`
  color: #2a2a2a !important;
  font-weight: 600;
`;

const getMonths = (startDt: Moment, endDt: Moment) => {
  let months = [];
  while (endDt > startDt || startDt.format('M') === endDt.format('M')) {
    months.push(startDt.clone().startOf('month'));
    startDt.add(1, 'month');
  }
  return months;
};

const getColumnSearchProps = (dataIndex: any, [searchText, setSearchText]: any) => {
  let searchInput: any;

  const handleSearch = (selectedKeys: any, confirm: any) => {
    confirm();
    setSearchText(selectedKeys[0]);
  };

  const handleReset = (clearFilters: any) => {
    clearFilters();
    setSearchText('');
  };
  return {
    filterDropdown: ({ setSelectedKeys, selectedKeys, confirm, clearFilters }: any) => (
      <div style={{ padding: 8 }}>
        <Input
          ref={(node: any) => {
            searchInput = node;
          }}
          placeholder={`Search`}
          value={selectedKeys[0]}
          onChange={(e: any) => setSelectedKeys(e.target.value ? [e.target.value] : [])}
          onPressEnter={() => handleSearch(selectedKeys, confirm)}
          style={{ width: 188, marginBottom: 8, display: 'block' }}
        />
        <Button type="primary" onClick={() => handleSearch(selectedKeys, confirm)} icon="search" size="small" style={{ width: 90, marginRight: 8 }}>
          Search
        </Button>
        <Button onClick={() => handleReset(clearFilters)} size="small" style={{ width: 90 }}>
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
        setTimeout(() => searchInput.select());
      }
    },
    render: (text: any) => (
      <Highlighter
        highlightStyle={{ backgroundColor: '#ffc069', padding: 0 }}
        searchWords={[searchText]}
        autoEscape
        textToHighlight={text.toString()}
      />
    ),
  };
};

const ProjectSummary: React.SFC = () => {
  let dateEnd = moment();
  let dateStart = dateEnd.clone().subtract(12, 'months');
  const months = getMonths(dateStart, dateEnd).reverse();
  const searchState = useState('');
  const columns = [
    {
      title: 'Project',
      key: 'consultantName',
      sorter: (a: any, b: any) => a.consultantName.localeCompare(b.consultantName),
      ...getColumnSearchProps(['consultantName', 'companyName'], searchState),
      render: (item: any) => {
        console.log(item);
        return (
          <div>
            <StyledProject>{item.consultantName}</StyledProject> <br />
            <Text type="secondary">{item.companyName}</Text>
          </div>
        );
      },
    },
    ...months.map((d: any) => ({
      title: d.format('MMM YYYY'),
      key: d,
      dataIndex: 'monthlySummaries',
      render: (item: any) => {
        let val;
        item &&
          item.map((data: any) => {
            if (d.month() === data.month - 1 && data.year === d.year()) {
              val = data.value && parseFloat(data.value).toFixed(2);
            }
          });
        return val;
      },
    })),
  ];

  return (
    <Get url={'/api/services/app/HourLogEntry/GetProjectMonthlyHourReport'}>
      {({ error, data, loading }: any) => {
        const result = data && data.result;
        return (
          <StyledTable loading={loading} dataSource={result} columns={columns} tableLayout="auto" size="small" pagination={{ size: 'default' }} />
        );
      }}
    </Get>
  );
};

export default ProjectSummary;
