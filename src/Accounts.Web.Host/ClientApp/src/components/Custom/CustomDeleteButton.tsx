import * as React from 'react';
import { Button } from 'antd';
import { ButtonProps } from 'antd/es/button';
import styled from '@emotion/styled';

const StyledButton = styled(Button)`
  background: #ff00001a;
  color: #ff0000;
  border: none;
  :hover,
  :active,
  :focus {
    background: #ff00001a;
    color: #ff0000;
    border: none;
  }
`;

type CustomDeleteButtonProps = ButtonProps;

const CustomDeleteButton: React.FC<CustomDeleteButtonProps> = (props: any) => {
  return (
    <StyledButton {...props} icon="delete" type="danger">
      {props.children}
    </StyledButton>
  );
};

export default CustomDeleteButton;
