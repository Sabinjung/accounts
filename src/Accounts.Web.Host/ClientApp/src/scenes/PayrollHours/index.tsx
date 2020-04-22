import React, { useState, useEffect } from 'react';
import { DatePicker, Table, Row, Col, Typography, Input, Button, Icon, Tag, Select } from 'antd';
import Highlighter from 'react-highlight-words';
import styled from '@emotion/styled';
import moment from 'moment';
import useAxios from '../../lib/axios/useAxios';

const { MonthPicker } = DatePicker;
const { Text } = Typography;
const { Option } = Select;

type DisplayHoursProps = {
    result: any;
    loading: boolean;
    setSelectedDate: any;
    selectedDate: any;
}

const StyledRow = styled(Row)`
    margin-bottom: 20px;
`
const StyledTag = styled(Tag)`
   padding: 5px !important;
`
const StyledDiv = styled.div`
    padding: 3px !important;
`
const StyledInput = styled(Input)`
    width: 175px !important;
    margin-bottom: 8px !important;
    display: block !important;
`
const StyledButton = styled(Button)`
    width: 80px !important;
`
const StyledText = styled(Text)`
    font-size: 16px;
`
const StyledSelect = styled(Select)`
    width: 58px;
`
const StyledProject = styled(Text)`
    color: #d83d42 !important;
`
const StyledTable = styled(Table)`
    .ant-table-row {
        td{
            &.created{
                background: #fdff93 !important;
                text-align: center;
            }
            &.approved{
                background: #9dccff !important;
                text-align: center;
            }
            &.invoiced{
                background: #7ac78c !important;
                text-align: center;
            }
            &.entered{
                background: #f98590 !important;
                text-align: center;
            }
            &.all-invoiced{
                background: #7ac78c !important;
            }
        }
    }
`

const daysInMonth = (month: any) => {
    let count = moment().month(month).daysInMonth();
    let days = [];
    for (let i = 1; i < count + 1; i++) {
        days.push(moment().month(month).date(i));
    }
    return days;
}


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
                <StyledButton onClick={() => handleReset(clearFilters)} size="small" >
                    Reset
                </StyledButton>
            </StyledDiv >
        ),
        filterIcon: (filtered: any) => <Icon type="search" style={{ color: filtered ? '#1890ff' : undefined }} />,
        onFilter: (value: any, record: any) =>
            dataIndex.reduce((s: any, di: any) => record[s].concat(record[di], ''))
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


const DisplayHours: React.SFC<DisplayHoursProps> = ({
    result,
    loading,
    setSelectedDate,
    selectedDate
}) => {

    const searchState = useState('');
    const [pageSize, setPageSize] = useState(10);
    let dates = daysInMonth(selectedDate.month())
    let totalCount = result && result.length;

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
                        {(item.isActive) ? (
                            <>
                                <Text>{item.consultantName}</Text> <br />
                                <Text type="secondary">{item.companyName}</Text>
                            </>
                        ) : (
                                <>
                                    <StyledProject>{item.consultantName}</StyledProject> <br />
                                    <StyledProject>{item.companyName}</StyledProject>
                                </>
                            )}
                    </div>
                )
            }
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
                        status = data.status
                        if (data.status === null) {
                            status = 'entered'
                        }
                    }
                });
                return {
                    props: {
                        className: status.toLowerCase(),
                    },
                    children: val
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
                        return totalHrs
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
                    if (data.status !== "Invoiced") {
                        isAllInvoiced = false;
                    }
                });
                return {
                    props: {
                        className: isAllInvoiced && 'all-invoiced',
                    },
                    children: val
                };
            },
        },
    ];

    return (
        <>
            <Row type="flex" justify="space-between">
                <Col>
                    <MonthPicker value={selectedDate} format={'MMMM YYYY'} allowClear={false} onChange={(date: any) => setSelectedDate(date)} />
                </Col>
                <Col>
                    <StyledText>Show </StyledText>
                    <StyledSelect value={pageSize} onChange={(value: any) => setPageSize(value)}>
                        <Option value={10}>10</Option>
                        <Option value={25}>25</Option>
                        <Option value={50}>50</Option>
                    </StyledSelect>
                </Col>
            </Row>
            <StyledRow type="flex" justify="end">
                <Col>
                    {
                        (totalCount < pageSize) ? <StyledText>{totalCount} out of {totalCount} Projects</StyledText>
                            : <StyledText>{pageSize} out of {totalCount} Projects</StyledText>
                    }
                </Col>
            </StyledRow>
            <StyledTable
                bordered
                loading={loading}
                size='small'
                dataSource={result}
                columns={columns}
                scroll={{ x: 1300 }}
                pagination={{ pageSize: pageSize }}
                footer={() => (
                    <Row gutter={48} type="flex" align='middle'>
                        <Col span={9}>
                            <StyledTag color="#d83d42" />
                                Inactive
                        </Col>
                        <Col>
                            <StyledTag color="#f98590" />
                                Entered
                        </Col>
                        <Col>
                            <StyledTag color="#fdff93" />
                                Pending
                        </Col>
                        <Col>
                            <StyledTag color="#9dccff" />
                                Approved
                        </Col>
                        <Col>
                            <StyledTag color="#7ac78c" />
                                Invoiced
                        </Col>
                    </Row>
                )}
            />
        </>
    );
}


const PayrollHours: React.SFC = () => {

    const [selectedDate, setSelectedDate] = useState(moment().subtract(1, 'months'))
    const month = selectedDate.month() + 1
    const year = selectedDate.year()

    const [{ data, loading }, makeRequest] = useAxios({
        url: 'api/services/app/HourLogEntry/GetPayrollHourLogsReport',
        params: { Month: month, Year: year },
    });
    const result = (data && data.result);

    useEffect(() => {
        makeRequest({ params: { Month: month, Year: year } });
    }, [selectedDate]);

    return (
        <DisplayHours
            result={result}
            loading={loading}
            selectedDate={selectedDate}
            setSelectedDate={setSelectedDate}
        />
    );
}

export default PayrollHours;