import './index.css';

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as moment from 'moment';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'mobx-react';
import { ThemeProvider } from 'emotion-theming';
import { Global, css } from '@emotion/core';

import Utils from './utils/utils';
import abpUserConfigurationService from './services/abpUserConfigurationService';
import { RootStore } from './stores/storeInitializer';
import registerServiceWorker from './registerServiceWorker';
import httpService from './services/httpService';
import AxiosProvider from './lib/axios/AxiosProvider';
import theme from './theme';
const globalStyles = css`
  .ant-table-row-cell-break-word {
    word-wrap: inherit !important;
    word-break: inherit !important;
  }
  ::-webkit-scrollbar {
    width: 5px;
    height: 5px;
  }

  /* Track */
  ::-webkit-scrollbar-track {
    background: #f1f1f1;
  }

  /* Handle */
  ::-webkit-scrollbar-thumb {
    background: #888;
  }

  /* Handle on hover */
  ::-webkit-scrollbar-thumb:hover {
    background: #555;
  }
`;

declare var abp: any;

Utils.setLocalization();

abpUserConfigurationService.getAll().then(data => {
  Utils.extend(true, abp, data.data.result);
  abp.clock.provider = Utils.getCurrentClockProvider(data.data.result.clock.provider);

  moment.locale(abp.localization.currentLanguage.name);

  if (abp.clock.provider.supportsMultipleTimezone) {
    moment.tz.setDefault(abp.timing.timeZoneInfo.iana.timeZoneId);
  }
  const rootStore = new RootStore();

  ReactDOM.render(
    <Provider {...rootStore}>
      <AxiosProvider instance={httpService}>
        <BrowserRouter>
          <ThemeProvider theme={theme}>
            <Global styles={css(globalStyles)} />
            <App />
          </ThemeProvider>
        </BrowserRouter>
      </AxiosProvider>
    </Provider>,
    document.getElementById('root') as HTMLElement
  );

  registerServiceWorker();
});
