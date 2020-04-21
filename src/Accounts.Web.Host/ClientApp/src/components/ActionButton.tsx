import React, { useState } from 'react';
import { Button } from 'antd';
import Authorize from './Authorize';
import { RequestWrapper } from '../lib/axios/components/Request';

export default ({ children, url, onSuccess, params, data, onError, permissions, onSubmit, method = 'Post', style, type = "primary" }: any) => {
  const [isReady, setIsReady] = useState(false);
  const [formData, setFormData] = useState(null);
  const Method = RequestWrapper(method);
  return (
    <Authorize permissions={permissions}>
      <Method
        url={url}
        isReady={isReady}
        onSuccess={(response: any) => {
          setIsReady(false);
          onSuccess(response);
        }}
        onError={err => {
          onError && onError(err);
          setIsReady(false);
        }}
        params={params}
        data={data || formData}
      >
        {({ loading }: any) => (
          <Button
            style={style}
            type={type}
            loading={loading}
            onClick={() => {
              if (onSubmit) {
                onSubmit({ setFormData, setIsReady });
              } else {
                setIsReady(true);
              }
            }}
          >
            {children}
          </Button>
        )}
      </Method>
    </Authorize>
  );
};
