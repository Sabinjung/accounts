import * as React from 'react';
import { Button } from 'antd';
import { ButtonProps } from 'antd/es/button';
import styled from '@emotion/styled';

const StyledButton = styled(Button)`
  background: #2680eb1a;
  color: #2680eb;
  border: none;
  margin-right: 8px;
  :hover,
  :active,
  :focus {
    background: #2680eb1a;
    color: #2680eb;
    border: none;
  }
`;

type CustomEditButtonProps = ButtonProps;

const CustomEditButton: React.FC<CustomEditButtonProps> = (props: any) => {
  return (
    <StyledButton {...props} icon="edit" type="primary">
      {props.children}
    </StyledButton>
  );
};

export default CustomEditButton;
