import React, { useState } from 'react';
import { Button, Popconfirm } from 'antd';

import Authorize from './Authorize';
import useAxios from '../lib/axios/useAxios';

export type ConfirmActionButtonProps = {
  children?: any;
  url: string;
  onSuccess?: any;
  params?: any;
  data?: any;
  onError?: (arg: any) => void;
  permissions?: any;
  onSubmit?: any;
  method?: any;
  style?: any;
  type?: any;
  title?: string;
  icon?: any;
  placement?: string;
};

const ConfirmActionButton: React.FC<ConfirmActionButtonProps> = ({
  children,
  url,
  onSuccess,
  params,
  data,
  onError,
  permissions,
  onSubmit,
  method = 'Post',
  style,
  type = 'primary',
  placement = 'topRight',
  title,
  icon,
}: any) => {
  const [isReady, setIsReady] = useState(false);
  const [formData, setFormData] = useState({});
  const [isVisible, setIsVisible] = useState(false);

  const [{ loading }] = useAxios(
    {
      url,
      method,
      params,
      data: data || formData,
    },
    {
      isReady,
      onSuccess: (response: any) => {
        setIsReady(false);
        setIsVisible(false);
        onSuccess(response);
      },
      onError: (err) => {
        onError && onError(err);
        setIsReady(false);
      },
    }
  );
  return (
    <Authorize permissions={permissions}>
      <Popconfirm
        title={children(setFormData, formData)}
        okText="Yes"
        cancelText="No"
        placement={placement}
        visible={isVisible}
        onCancel={() => {
          setIsVisible(false);
        }}
        onConfirm={() => {
          if (onSubmit) {
            const returnVal = onSubmit({ setFormData });
            setIsReady(returnVal);
          } else {
            setIsReady(true);
          }
        }}
      >
        <Button style={style} type={type} icon={icon} loading={loading} onClick={() => setIsVisible(true)}>
          {title}
        </Button>
      </Popconfirm>
    </Authorize>
  );
};

export default ConfirmActionButton;
