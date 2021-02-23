import * as React from 'react';
import { Button } from 'antd';
import { ButtonProps } from 'antd/es/button';
import styled from '@emotion/styled';

const StyledButton = styled(Button)`
  color: #2a2a2a;
  background: #cccccc;
  border: none;
  height: 40px;
  font-weight: 500;
  :hover,
  :active,
  :focus {
    background: #cccccc;
    font-weight: 500;
    border: none;
    color: #2a2a2a;
  }
`;

type CustomCancleButtonProps = ButtonProps;

const CustomCancleButton: React.FC<CustomCancleButtonProps> = (props: any) => {
  return <StyledButton {...props}>{props.children}</StyledButton>;
};

export default CustomCancleButton;
