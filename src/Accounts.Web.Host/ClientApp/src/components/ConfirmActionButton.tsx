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
  disable?: boolean;
  onVisibleChange?:any;
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
  disable,
  onVisibleChange,
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
        getPopupContainer={(trigger: any) => trigger.parentNode}
        okButtonProps={{disabled: disable}}
        onVisibleChange={onVisibleChange}
        placement={placement}
        onConfirm={() => {
          if (onSubmit) {
            const returnVal = onSubmit({ setFormData });
            setIsReady(returnVal);
          } else {
            setIsReady(true);
          }
        }}
      >
        <Button style={style} type={type} icon={icon} loading={loading}>
          {title}
        </Button>
      </Popconfirm>
    </Authorize>
  );
};

export default ConfirmActionButton;
