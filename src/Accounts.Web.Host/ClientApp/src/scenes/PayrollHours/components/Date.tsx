import React, { useState, useEffect } from 'react';
import moment from 'moment';
import { Typography } from 'antd';
import styled from '@emotion/styled';
const { Text } = Typography;

type DateProps = {
  dateswitch: boolean;
  item: any;
};

const ApproveBox = styled.div`
  cursor: pointer;
`;

const Date: React.FC<DateProps> = ({ dateswitch, item }) => {
  const [state, setState] = useState(true);
  const handleClickToggle = () => {
    setState(!state);
  };

  useEffect(() => {
    setState(dateswitch);
  }, [dateswitch]);

  return (
    <ApproveBox onClick={handleClickToggle}>
      {state ? (
        <Text>{item.lastApprovedDate && moment(item.lastApprovedDate).fromNow()}</Text>
      ) : (
        <Text>{item.lastApprovedDate && moment(item.lastApprovedDate).format('MM/DD/YYYY')}</Text>
      )}
    </ApproveBox>
  );
};

export default Date;
