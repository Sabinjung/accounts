import * as React from 'react';
import { Popover, Row, Col, Input } from 'antd';
import styled from '@emotion/styled';
import moment from 'moment';

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
`;

const StyledInput = styled(Input)`
  border: none !important;
  &.is-holiday {
    background: #f6f6f6 !important;
  }
`;

type EditHourlogProps = {
  description: string;
  logedHours: any;
  setLogedHours: any;
};

const EditHourlog: React.FC<EditHourlogProps> = ({ description, logedHours, setLogedHours }) => {
  const handleHourEdit = (e: any) => {
    let newLogedHour = logedHours;
    let val: any = e.target.value === '' ? null : parseFloat(e.target.value);
    newLogedHour[e.target.name].hours = val;
    setLogedHours([...newLogedHour]);
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
      {logedHours.map((item: any, index: any) => (
        <Col key={index}>
          <StyledDiv>
            <StyledHeader className={weekEnd(item.day)}>{moment(item.day).format('MM/DD')}</StyledHeader>
            <StyledHour className={weekEnd(item.day)}>
              <StyledInput className={weekEnd(item.day)} name={index} defaultValue={item.hours} onChange={handleHourEdit} />
            </StyledHour>
          </StyledDiv>
        </Col>
      ))}
    </StyledRow>
  );

  return (
    <Popover content={content} placement="bottom" trigger="click">
      <a>{description}</a>
    </Popover>
  );
};

export default EditHourlog;
