import * as React from 'react';
import { Modal } from 'antd';
import { ModalProps } from 'antd/es/modal';
import styled from '@emotion/styled';

const StyledModal = styled(Modal)`
  .ant-modal-close {
    box-shadow: 1px 1px 6px #00000029;
    border-radius: 50%;
    top: 21px;
    right: 20px;
    .ant-modal-close-x {
      width: 35px;
      height: 35px;
      line-height: 37px;
    }
  }
  .ant-modal-header {
    border: none;
    padding: 28px 43px;
    .ant-modal-title {
      font-size: 20px;
    }
  }
  .ant-modal-footer {
    border: none;
  }
`;
type CustomModalProps = ModalProps;

const CustomModal: React.FC<CustomModalProps> = (props: any) => {
  return <StyledModal {...props}> {props.children}</StyledModal>;
};

export default CustomModal;
