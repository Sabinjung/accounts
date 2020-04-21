import React from 'react';

import axios, { AxiosInstance } from 'axios';

export const AxiosContext = React.createContext<AxiosInstance>(axios);

type IAxiosProviderProps = {
  instance: AxiosInstance;
};

const AxiosProvider: React.FC<IAxiosProviderProps> = ({ instance, children }) => {
  return <AxiosContext.Provider value={instance}>{children}</AxiosContext.Provider>;
};

export default AxiosProvider;
