import React from 'react';
import moment from 'moment';
import CustomCancleButton from '../../../components/Custom/CustomCancelButton';
import CustomHoursTable from '../../../components/Custom/CustomHoursTable';
import { Row, Col } from 'antd';
import styled from '@emotion/styled';

export type IHourLogYearlyReportProps = {
  loading: boolean;
  dataSource: any;
  onClose?: any;
};

const StyledRow = styled(Row)`
.consultantName {
  font-size: 1rem;
  font-weight: 500;
  margin-bottom: 30px;
}
`;

const HourLogYearlyReport: React.FC<IHourLogYearlyReportProps> = ({ loading, dataSource, onClose }) => {
  const tableData = dataSource && dataSource.unassociatedHourReportDtos;
  const handleRowExpand = (record: any) => {
    const dayList = record.days;
    const columns: any = dayList.map((record: any) => ({
      title: moment(record.day).format('MM/DD'),
      width: 60,
      dataIndex: moment(record.day).format('MM/DD'),
      className: moment(record.day).isoWeekday() === 6 || moment(record.day).isoWeekday() === 7 ? 'is-holiday' : '',
    }));

    let result = {};
    for (let i = 0; i < dayList.length; i++) {
      result[moment(dayList[i].day).format('MM/DD')] = dayList[i].hour;
    }

    return <CustomHoursTable className="inner-table" size="small" columns={columns} dataSource={[result]} pagination={false} scroll={{ x: 700 }} />;
  };

  const column = [
    {
      title: 'Month',
      dataIndex: 'monthName',
      key: 'monthName',
      render: (label: any) => {
        const month =
          label &&
          moment()
            .month(label - 1)
            .format('MMMM');
        return month ? month : null;
      },
    },
    {
      title: 'Year',
      dataIndex: 'year',
      key: 'year',
    },
    {
      title: 'Unassociated Hours',
      dataIndex: 'totalHours',
      key: 'totalHours',
    },
  ];

  return (
    <React.Fragment>
      <StyledRow>
        <Col offset={1}>
        <h4 className="consultantName">{dataSource && dataSource.consultantName}</h4>
        </Col>
      </StyledRow>
      <CustomHoursTable
        loading={loading}
        dataSource={tableData}
        onExpand={(expanded, record) => handleRowExpand(record)}
        expandedRowRender={handleRowExpand}
        columns={column}
        pagination={{ pageSize: 10, total: tableData === undefined ? 0 : tableData.length, defaultCurrent: 1 }}
      />

      <div
        style={{
          position: 'absolute',
          left: 0,
          bottom: 0,
          width: '100%',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
        }}
      >
        <CustomCancleButton
          style={{ marginRight: 8 }}
          onClick={() => {
            onClose();
          }}
        >
          Cancel
        </CustomCancleButton>
      </div>
    </React.Fragment>
  );
};

export default HourLogYearlyReport;
