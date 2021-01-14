import React, { useState } from 'react';
import { Popover, Row, Col, Input, DatePicker } from 'antd';
import styled from '@emotion/styled';
import moment from 'moment';
import { isGranted } from '../../lib/abpUtility';

const StyledDiv = styled.div`
  height: 82px;
  width: 55px;
  border: 1px solid #e8e8e8;
  text-align: center;
`;

const StyledHeader = styled.div`
  border-bottom: 1px solid #e8e8e8;
  font-weight: 700;
  padding: 5px;
  background: #f0f2f5;
  &.is-holiday {
    background: #ddd !important;
  }
`;

const StyledHour = styled.div`
  padding: 9px;
  &.is-holiday {
    background: #f6f6f6 !important;
  }
`;

const StyledRow = styled(Row)`
  flex-flow: row nowrap;
  max-width: 600px;
  overflow-x: scroll;
  overflow-y: hidden;
`;

const StyledInput = styled(Input)`
  border: none !important;
  &.is-holiday {
    background: #f6f6f6 !important;
  }
`;

const StyledDatepicker = styled(DatePicker)`
  width: 106px;
  .ant-calendar-picker-input {
    color: #1da57a;
  }
`;

type EditHourlogProps = {
  description: string;
  logedHours: any;
  setLogedHours: any;
  originalHours: any;
};

const EditHourlog: React.FC<EditHourlogProps> = ({ description, logedHours, setLogedHours, originalHours }) => {
  let startDt = originalHours[0].day;
  let endDt = originalHours[originalHours.length - 1].day;
  const [endDate, setEndDate] = useState(moment(endDt));
  let filteredLogedHours = logedHours;

  const disabledDate: any = (current: any) => {
    return current < moment(startDt) || current >= moment(endDt).endOf('days');
  };

  const handleDate = (date: any) => {
    filteredLogedHours = originalHours.filter((data: any) => moment(data.day).isBetween(startDt, date, 'day', '[]'));
    setEndDate(date);
    setLogedHours([...filteredLogedHours]);
  };

  const handleHourEdit = (e: any) => {
    let val: any = isNaN(parseFloat(e.target.value)) ? null : parseFloat(e.target.value);
    filteredLogedHours[e.target.name].hours = val;
    setLogedHours([...filteredLogedHours]);
  };

  const weekEnd = (day: any) => {
    if (moment(day).isoWeekday() === 6 || moment(day).isoWeekday() === 7) {
      return 'is-holiday';
    } else {
      return '';
    }
  };

  const content = (
    <StyledRow type="flex">
      {filteredLogedHours.map((item: any, index: any) => (
        <Col key={index}>
          <StyledDiv>
            <StyledHeader className={weekEnd(item.day)}>{moment(item.day).format('MM/DD')}</StyledHeader>
            <StyledHour className={weekEnd(item.day)}>
              <StyledInput className={weekEnd(item.day)} name={index} value={item.hours} onChange={handleHourEdit} />
            </StyledHour>
          </StyledDiv>
        </Col>
      ))}
    </StyledRow>
  );

  return (
    <>
      <Popover content={content} placement="bottom" trigger="click">
        {isGranted('Invoicing.EditEndDate') ? <a>{'Billing Period ' + moment(startDt).format('MM/DD/YYYY') + '-'}</a> : <a>{description}</a>}
      </Popover>
      {isGranted('Invoicing.EditEndDate') && (
        <StyledDatepicker size="small" allowClear={false} value={endDate} disabledDate={disabledDate} onChange={handleDate} format="MM/DD/YYYY" />
      )}
    </>
  );
};

export default EditHourlog;
