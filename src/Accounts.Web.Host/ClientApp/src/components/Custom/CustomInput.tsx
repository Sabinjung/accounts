import * as React from 'react';
import { Input } from 'antd';
import { InputProps } from 'antd/es/input';
import styled from '@emotion/styled';

const StyledInput = styled(Input)`
  box-shadow: 0px 3px 10px #0000000d;
`;

type CustomInputProps = InputProps;

const CustomInput: React.FC<CustomInputProps> = (props) => {
  return <StyledInput {...props} allowClear/>;
};

export default CustomInput;
