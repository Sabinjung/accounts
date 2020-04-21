import { useReducer, useEffect, useCallback, useContext } from 'react';
import axios, { AxiosRequestConfig, AxiosInstance, AxiosResponse, AxiosError } from 'axios';
import { debounce, merge } from 'lodash';
import { AxiosContext } from './AxiosProvider';

/*
    1. isReady
    2. onSuccess
    3. onFailure
    4. refetchQueries
*/

const ActionTypes = {
  REQUEST_START: 'REQUEST_START',
  REQUEST_END: 'REQUEST_END',
};

function createInitialState() {
  return {
    loading: false,
  };
}

interface IRequestState {
  loading: boolean;
  data?: any;
  error?: any;
}

interface IUseAxiosOptions {
  isReady?: boolean;
  debounce?: number;
  onSuccess?: (response: AxiosResponse) => void;
  onError?: (error: AxiosError) => void;
}

function reducer(state: IRequestState, action: any) {
  switch (action.type) {
    case ActionTypes.REQUEST_START:
      return {
        ...state,
        loading: true,
        error: null,
      };
    case ActionTypes.REQUEST_END:
      return {
        ...state,
        loading: false,
        ...(action.error ? {} : { data: action.payload.data }),
        [action.error ? 'error' : 'response']: action.payload,
      };
    default:
      return state;
  }
}

async function request(axiosInstance: AxiosInstance, config: AxiosRequestConfig, dispatch: React.Dispatch<any>, options?: IUseAxiosOptions) {
  if (options && !options.isReady) return;
  try {
    dispatch({ type: ActionTypes.REQUEST_START });
    const response = await axiosInstance(config);
    dispatch({ type: ActionTypes.REQUEST_END, payload: response });
    if (options && options.onSuccess) {
      options.onSuccess(response);
    }
    return response;
  } catch (err) {
    dispatch({ type: ActionTypes.REQUEST_END, payload: err, error: true });
    if (options && options.onError) {
      options.onError(err);
    }
    throw err;
  }
}

export default function useAxios(
  config: AxiosRequestConfig | string,
  options?: IUseAxiosOptions
): [IRequestState, (config: AxiosRequestConfig) => void] {
  let finalConfig: AxiosRequestConfig;
  if (typeof config === 'string') {
    finalConfig = {
      url: config,
    };
  } else {
    finalConfig = config;
  }

  const mergedOptions = merge({ isReady: true, debounce: 200 }, options);

  const stringifiedConfig = JSON.stringify(finalConfig);

  const debounceRequest = debounce(request, options && options.debounce);
  const [state, dispatch] = useReducer(reducer, createInitialState());
  const axiosInstance = useContext(AxiosContext);

  useEffect(() => {
    const cancellationToken = axios.CancelToken.source();
    debounceRequest(axiosInstance, { ...finalConfig, cancelToken: cancellationToken.token }, dispatch, mergedOptions);
    return () => cancellationToken.cancel();
  }, [stringifiedConfig, mergedOptions.isReady]);

  const refetch = useCallback(
    (configOverride: AxiosRequestConfig) => {
      const cancellationToken = axios.CancelToken.source();
      const updatedOptions = merge(mergedOptions, { isReady: true });
      return debounceRequest(axiosInstance, { ...merge(finalConfig, configOverride), cancelToken: cancellationToken.token }, dispatch, updatedOptions);
    },
    [stringifiedConfig, mergedOptions.isReady]
  );

  return [state, refetch];
}
