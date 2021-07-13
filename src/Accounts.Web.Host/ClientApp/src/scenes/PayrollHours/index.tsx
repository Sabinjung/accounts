import React, { useState, useEffect } from 'react';
import { DatePicker, Row, Col, Typography, Input, Button, Icon, Select, Card } from 'antd';
import Date from './components/Date';
import Highlighter from 'react-highlight-words';
import styled from '@emotion/styled';
import moment from 'moment';
import useAxios from '../../lib/axios/useAxios';
import CustomHoursTable from './../../components/Custom/CustomHoursTable';

const { MonthPicker } = DatePicker;
const { Text } = Typography;
const { Option } = Select;

type DisplayHoursProps = {
  result: any;
  loading: boolean;
  setSelectedDate: any;
  selectedDate: any;
};

type TagProps = {
  color: string;
};

const StyledApproved = styled.div`
  display: flex;
  align-items: center;
`;

const StyledRow = styled(Row)`
  margin-bottom: 20px;
`;
const StyledTag = styled.div<TagProps>`
  padding: 9px;
  border-radius: 5px;
  background: ${(props) => props.color};
`;
const StyledDiv = styled.div`
  padding: 3px !important;
`;
const StyledInput = styled(Input)`
  width: 175px !important;
  margin-bottom: 8px !important;
  display: block !important;
`;
const StyledButton = styled(Button)`
  width: 80px !important;
`;
const StyledText = styled(Text)`
  font-size: 16px;
`;
const StyledSelect = styled(Select)`
  width: 58px;
`;
const StyledProject = styled(Text)`
  color: #d83d42 !important;
`;
const StyledProj = styled(Text)`
  color: #2a2a2a !important;
  font-weight: 600;
`;
const StyledSwap = styled.div`
  position: absolute;
  right: 0;
  padding: 18px 6px;
  cursor: pointer;
  :hover {
    background: #e5e5e5;
  }
`;

const StyledIcon = styled(Icon)`
  color: rgba(0, 0, 0, 0.45);
`;

const StyledTable = styled(CustomHoursTable)`
  .iconrow {
    position: relative;
  }
  .ant-table-fixed-right {
    th {
      padding: 18px 8px !important;
    }
  }
  .ant-table-row {
    td {
      &.created {
        background: #fdff93 !important;
        text-align: center;
        color: #2a2a2a;
      }
      &.approved {
        background: #9dccff !important;
        text-align: center;
        color: #2a2a2a;
      }
      &.invoiced {
        background: #c6ade5 !important;
        text-align: center;
        color: #2a2a2a;
      }
      &.entered {
        background: #f98590 !important;
        text-align: center;
        color: #2a2a2a;
      }
      &.all-invoiced {
        text-align: center;
        background: #7034bd !important;
        color: white;
      }
    }
  }
`;

const daysInMonth = (month: any) => {
  let count = moment().month(month).daysInMonth();
  let days = [];
  for (let i = 1; i < count + 1; i++) {
    days.push(moment().month(month).date(i));
  }
  return days;
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
      <StyledDiv>
        <StyledInput
          ref={(node: any) => {
            searchInput = node;
          }}
          placeholder={`Search`}
          value={selectedKeys[0]}
          onChange={(e: any) => setSelectedKeys(e.target.value ? [e.target.value] : [])}
          onPressEnter={() => handleSearch(selectedKeys, confirm)}
        />
        <StyledButton type="primary" onClick={() => handleSearch(selectedKeys, confirm)} icon="search" size="small" style={{ marginRight: '10px' }}>
          Search
        </StyledButton>
        <StyledButton onClick={() => handleReset(clearFilters)} size="small">
          Reset
        </StyledButton>
      </StyledDiv>
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

const DisplayHours: React.SFC<DisplayHoursProps> = ({ result, loading, setSelectedDate, selectedDate }) => {
  const searchState = useState('');
  const [pageSize, setPageSize] = useState(10);
  const [date, setDate] = useState(true);
  let dates = daysInMonth(selectedDate.month());
  let totalCount = result && result.length;

  const handleAllDate = () => {
    setDate(!date);
  };
  const columns = [
    {
      title: 'Project',
      key: 'consultantName',
      width: 180,
      fixed: 'left' as 'left',
      sorter: (a: any, b: any) => a.consultantName.localeCompare(b.consultantName),
      ...getColumnSearchProps(['consultantName', 'companyName'], searchState),
      render: (item: any) => {
        return (
          <div>
            {item.isActive ? (
              <>
                <StyledProj>{item.consultantName}</StyledProj> <br />
                <Text type="secondary">{item.companyName}</Text>
              </>
            ) : (
              <>
                <StyledProject>{item.consultantName}</StyledProject> <br />
                <StyledProject>{item.companyName}</StyledProject>
              </>
            )}
          </div>
        );
      },
    },
    {
      title: () => (
        <StyledApproved>
          <div>Last Approved</div>
          <StyledSwap onClick={handleAllDate}>
            <StyledIcon type="swap" />
          </StyledSwap>
        </StyledApproved>
      ),
      className: 'iconrow',
      key: 'lastApproved',
      width: 150,
      fixed: 'left' as 'left',
      render: (item: any) => {
        return (
          <div>
            <Date dateswitch={date} item={item} />
          </div>
        );
      },
    },

    ...dates.map((d: any) => ({
      title: moment(d).format('MM/DD'),
      width: 50,
      key: moment(d).format('MM/DD/YYYY'),
      render: (item: any) => {
        let val;
        let status = '';
        item.dailyHourLogs.map((data: any) => {
          if (d.month() === moment(data.day).month() && d.date() === moment(data.day).date()) {
            val = data.hours;
            status = data.status;
            if (data.status === null) {
              status = 'entered';
            }
          }
        });
        return {
          props: {
            className: status.toLowerCase(),
          },
          children: val,
        };
      },
    })),
    {
      title: '1H',
      key: '1st',
      fixed: 'right' as 'right',
      width: 50,
      render: (item: any) => {
        let val = 0;
        val = item.dailyHourLogs.reduce((totalHrs: any, data: any) => {
          if (moment(data.day).date() <= 15) {
            totalHrs += data.hours;
            return totalHrs;
          } else {
            return totalHrs;
          }
        }, 0);
        return val;
      },
    },
    {
      title: '2H',
      key: '2nd',
      fixed: 'right' as 'right',
      width: 50,
      render: (item: any) => {
        let val = 0;
        val = item.dailyHourLogs.reduce((totalHrs: any, data: any) => {
          if (moment(data.day).date() > 15) {
            totalHrs += data.hours;
            return totalHrs;
          } else {
            return totalHrs;
          }
        }, 0);
        return val;
      },
    },
    {
      title: 'T',
      key: 'total',
      fixed: 'right' as 'right',
      width: 50,
      render: (item: any) => {
        let val = 0;
        let isAllInvoiced = true;
        val = item.dailyHourLogs.reduce((totalHrs: any, data: any) => {
          totalHrs += data.hours;
          return totalHrs;
        }, 0);
        if (item.dailyHourLogs.length === 0) {
          isAllInvoiced = false;
        }
        item.dailyHourLogs.map((data: any) => {
          if (data.status !== 'Invoiced') {
            isAllInvoiced = false;
          }
        });
        return {
          props: {
            className: isAllInvoiced && 'all-invoiced',
          },
          children: val,
        };
      },
    },
  ];

  return (
    <Card>
      <h1>PAYROLL HOURS</h1>
      <Row type="flex" justify="space-between">
        <Col>
          <MonthPicker value={selectedDate} format={'MMMM YYYY'} allowClear={false} onChange={(date: any) => setSelectedDate(date)} getCalendarContainer={(trigger: any) => trigger.parentNode} />
        </Col>
        <Col>
          <StyledText>Show </StyledText>
          <StyledSelect value={pageSize} onChange={(value: any) => setPageSize(value)} getPopupContainer={(trigger: any) => trigger.parentNode}>
            <Option value={10}>10</Option>
            <Option value={25}>25</Option>
            <Option value={50}>50</Option>
          </StyledSelect>
        </Col>
      </Row>
      <StyledRow type="flex" justify="end">
        <Col>
          {totalCount < pageSize ? (
            <StyledText>
              {totalCount} out of {totalCount} Projects
            </StyledText>
          ) : (
            <StyledText>
              {pageSize} out of {totalCount} Projects
            </StyledText>
          )}
        </Col>
      </StyledRow>
      <StyledTable
        loading={loading}
        size="small"
        dataSource={result}
        columns={columns}
        scroll={{ x: 470 }}
        pagination={{ pageSize: pageSize, size: 'default' }}
        footer={() => (
          <Row gutter={48} type="flex" align="middle">
            <Col span={9}>
              <Row type="flex" gutter={8} align="middle">
                <Col>
                  <StyledTag color="#d83d42" />
                </Col>
                <Col>Inactive</Col>
              </Row>
            </Col>
            <Col>
              <Row type="flex" gutter={8} align="middle">
                <Col>
                  <StyledTag color="#f98590" />
                </Col>
                <Col>Entered</Col>
              </Row>
            </Col>
            <Col>
              <Row type="flex" gutter={8} align="middle">
                <Col>
                  <StyledTag color="#fdff93" />
                </Col>
                <Col>Pending</Col>
              </Row>
            </Col>
            <Col>
              <Row type="flex" gutter={8} align="middle">
                <Col>
                  <StyledTag color="#9dccff" />
                </Col>
                <Col>Approved</Col>
              </Row>
            </Col>
            <Col>
              <Row type="flex" gutter={8} align="middle">
                <Col>
                  <StyledTag color="#C6ADE5" />
                </Col>
                <Col>Invoiced</Col>
              </Row>
            </Col>
          </Row>
        )}
      />
    </Card>
  );
};

const PayrollHours: React.FC = () => {
  const [selectedDate, setSelectedDate] = useState(moment().subtract(1, 'months'));
  const month = selectedDate.month() + 1;
  const year = selectedDate.year();

  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/HourLogEntry/GetPayrollHourLogsReport',
    params: { Month: month, Year: year },
  });
  const result = data && data.result;

  useEffect(() => {
    makeRequest({ params: { Month: month, Year: year } });
  }, [selectedDate]);

  return <DisplayHours result={result} loading={loading} selectedDate={selectedDate} setSelectedDate={setSelectedDate} />;
};

export default PayrollHours;
