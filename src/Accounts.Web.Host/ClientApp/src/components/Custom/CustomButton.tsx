import * as React from 'react';
import { Button } from 'antd';
import { ButtonProps } from 'antd/es/button';
import styled from '@emotion/styled';

const StyledButton = styled(Button)`
  box-shadow: 0px 3px 20px #2680eb66;
  height: 40px;
`;

type CustomButtonProps = ButtonProps;

const CustomButton: React.FC<CustomButtonProps> = (props: any) => {
  return <StyledButton {...props}>{props.children}</StyledButton>;
};

export default CustomButton;
