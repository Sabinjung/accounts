import React from 'react';
import useAxios from '../useAxios';
import { AxiosResponse, AxiosError } from 'axios';

type RequestTypes = 'get' | 'post' | 'delete' | 'put' | 'head';

export interface IRequestProps {
  url: string;
  method?: RequestTypes;
  params?: any;
  data?: any;
  debounce?: number;
  isReady?: boolean;
  onSuccess?: (response: AxiosResponse) => void;
  onError?: (error: AxiosError) => void;
}

const Request: React.FC<IRequestProps> = ({ children, ...rest }) => {
  const config = {
    url: rest.url,
    params: rest.params,
    data: rest.data,
    method: rest.method,
  };

  const options = {
    isReady: rest.isReady,
    debounce: rest.debounce,
    onSuccess: rest.onSuccess,
    onError: rest.onError,
  };

  const [{ data, loading, error }, refetch] = useAxios(config, options);
  if (typeof children === 'function') {
    return children({ data, loading, error, refetch });
  }

  return null;
};

export const RequestWrapper = (method: RequestTypes) => {
  const WrappedRequest: React.FC<IRequestProps> = props => (
    <Request {...props} method={method}>
      {props.children}
    </Request>
  );

  return WrappedRequest;
};

export default Request;
