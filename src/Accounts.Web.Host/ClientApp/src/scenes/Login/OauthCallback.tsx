import React, { useEffect, useState } from 'react';
import { inject, observer } from 'mobx-react';
import Stores from '../../stores/storeIdentifier';

import { Result, Button } from 'antd';
import { useHistory } from 'react-router';
import AppConsts from '../../lib/appconst';

export const Google = {
  REDIRECT_URI: `${AppConsts.appBaseUrl}/user/oauth_callback`,
  SCOPE: 'openid profile email',
};

const LoginCallback = inject(Stores.AuthenticationStore)(
  observer(({ location, authenticationStore }: any) => {
    const history = useHistory();
    const [authenticationResult, setAuthenticationResult] = useState({
      isAuthenticationSuccessful: false,
      error: '',
    });
    const qs = new URLSearchParams(location.search);
    useEffect(() => {
      async function externalAuthenticate() {
        const result = await authenticationStore.externalLogin({
          authProvider: 'Google',
          providerAccessCode: qs.get('code'),
          providerKey: 'Google',
        });
        setAuthenticationResult(result);
      }
      externalAuthenticate();
    }, [location]);

    if (authenticationResult.isAuthenticationSuccessful) {
      window.location.href = qs.get('state') || '/';
    }

    if (authenticationResult.error) {
      return (
        <Result
          status="warning"
          title="403"
          subTitle="Sorry, you are not authorized to access this application. Contact system administrator or try login with different user."
          extra={
            <Button type="primary" onClick={() => history.push('/user/login')}>
              Login
            </Button>
          }
        />
      );
    }
    return authenticationResult.isAuthenticationSuccessful ? (
      <Result title="Your are successfully authenticated." />
    ) : authenticationResult.error ? (
      <Result status="error" title="Error authenticating " />
    ) : (
      <Result title="Authenticating" />
    );
  })
);

export default LoginCallback;
