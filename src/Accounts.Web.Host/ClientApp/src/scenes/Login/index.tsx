import './index.less';

import * as React from 'react';

import { Button, Card, Checkbox, Col, Form, Icon, Input, Modal, Row } from 'antd';
import { inject, observer } from 'mobx-react';

import AccountStore from '../../stores/accountStore';
import AuthenticationStore from '../../stores/authenticationStore';
import { FormComponentProps } from 'antd/lib/form';
import { L } from '../../lib/abpUtility';
import { Redirect } from 'react-router-dom';
import SessionStore from '../../stores/sessionStore';
import Stores from '../../stores/storeIdentifier';
import TenantAvailabilityState from '../../services/account/dto/tenantAvailabilityState';
import rules from './index.validation';
import AppConsts from '../../lib/appconst';
import styled from '@emotion/styled';
import ITsutra from '../../images/itsutra.png';

const StyledH1 = styled.h1`
  color: #1c3faa;
  font-weight: bold;
  font-size: 34px;
  margin-bottom: 35px;
`;

const StyledCard = styled(Card)`
  border: none;
  padding: 15px 40px;
`;

const StyledInput = styled(Input)`
  input {
    height: 50px;
    box-shadow: 0px 3px 10px #0000000d;
    border-radius: 10px;
    padding-left: 40px !important;
  }
  .ant-input-prefix {
    left: 20px;
    top: 42%;
  }
  .ant-input-suffix {
    top: 43%;
  }
`;

const StyledButton = styled(Button)`
  width: 100%;
  height: 50px;
  border-radius: 10px;
`;

const StyledGoogle = styled(Button)`
  width: 100%;
  height: 50px;
  border-radius: 10px;
  border: 1px solid #2a2a2a;
  color: #2a2a2a;
  font-weight: 500;
`;

const StyledLable = styled.p`
  color: #5e5e5e;
`;

const StyledForm = styled(Form)`
  .ant-input-affix-wrapper {
    padding: 0px 0px 9px 0px;
  }
`;

const StyledP = styled.p`
  text-align: center;
  margin: 2em 0 0 0;
`;

const IconFont = Icon.createFromIconfontCN({
  scriptUrl: '//at.alicdn.com/t/font_2286956_rugdmojd5iq.js',
});

const FormItem = Form.Item;
declare var abp: any;

export const Google = {
  REDIRECT_URI: `${AppConsts.appBaseUrl}/user/oauth_callback`,
  SCOPE: 'openid profile email',
};

export interface ILoginProps extends FormComponentProps {
  authenticationStore?: AuthenticationStore;
  sessionStore?: SessionStore;
  accountStore?: AccountStore;
  history: any;
  location: any;
}

@inject(Stores.AuthenticationStore, Stores.SessionStore, Stores.AccountStore)
@observer
class Login extends React.Component<ILoginProps> {
  changeTenant = async () => {
    let tenancyName = this.props.form.getFieldValue('tenancyName');
    const { loginModel } = this.props.authenticationStore!;

    if (!tenancyName) {
      abp.multiTenancy.setTenantIdCookie(undefined);
      window.location.href = '/';
      return;
    } else {
      await this.props.accountStore!.isTenantAvailable(tenancyName);
      const { tenant } = this.props.accountStore!;
      switch (tenant.state) {
        case TenantAvailabilityState.Available:
          abp.multiTenancy.setTenantIdCookie(tenant.tenantId);
          loginModel.tenancyName = tenancyName;
          loginModel.toggleShowModal();
          window.location.href = '/';
          return;
        case TenantAvailabilityState.InActive:
          Modal.error({ title: L('Error'), content: L('TenantIsNotActive') });
          break;
        case TenantAvailabilityState.NotFound:
          Modal.error({ title: L('Error'), content: L('ThereIsNoTenantDefinedWithName{0}', tenancyName) });
          break;
      }
    }
  };

  handleSubmit = async (e: any) => {
    e.preventDefault();
    const { loginModel } = this.props.authenticationStore!;
    await this.props.form.validateFields(async (err: any, values: any) => {
      if (!err) {
        await this.props.authenticationStore!.login(values);
        sessionStorage.setItem('rememberMe', loginModel.rememberMe ? '1' : '0');
        const { state } = this.props.location;
        window.location = state ? state.from.pathname : '/';
      }
    });
  };

  handleExternalLogin = async (name: string, clientId: string) => {
    let { from } = this.props.location.state || { from: { pathname: '/' } };
    const qParams = [
      `redirect_uri=${Google.REDIRECT_URI}`,
      `scope=${Google.SCOPE}`,
      `prompt=consent`,
      `state=${from.pathname}`,
      `response_type=code`,
      `client_id=${clientId}`,
    ].join('&');
    try {
      const url = `https://accounts.google.com/o/oauth2/v2/auth?${qParams}`;
      window.location.assign(url);
    } catch (e) {
      console.error(e);
    }
  };

  componentWillMount() {
    this.props.authenticationStore!.getExternalAuthenticationProviders();
  }

  public render() {
    let { from } = this.props.location.state || { from: { pathname: '/' } };
    if (this.props.authenticationStore!.isAuthenticated) return <Redirect to={from} />;

    const { loginModel } = this.props.authenticationStore!;
    const { getFieldDecorator } = this.props.form;
    return (
      <Col className="name">
        <StyledForm className="" onSubmit={this.handleSubmit}>
          <Row>
            {/* <Row style={{ marginTop: 100 }}>
              <Col span={8} offset={8}>
                <Card>
                  <Row>
                    {!!this.props.sessionStore!.currentLogin.tenant ? (
                      <Col span={24} offset={0} style={{ textAlign: 'center' }}>
                        <Button type="link" onClick={loginModel.toggleShowModal}>
                          {L('CurrentTenant')} : {this.props.sessionStore!.currentLogin.tenant.tenancyName}
                        </Button>
                      </Col>
                    ) : (
                      <Col span={24} offset={0} style={{ textAlign: 'center' }}>
                        <Button type="link" onClick={loginModel.toggleShowModal}>
                          {L('NotSelected')}
                        </Button>
                      </Col>
                    )}
                  </Row>
                </Card>
              </Col>
            </Row>

            <Row>
              <Modal
                visible={loginModel.showModal}
                onCancel={loginModel.toggleShowModal}
                onOk={this.changeTenant}
                title={L('ChangeTenant')}
                okText={L('OK')}
                cancelText={L('Cancel')}
              >
                <Row>
                  <Col span={8} offset={8}>
                    <h3>{L('TenancyName')}</h3>
                  </Col>
                  <Col>
                    <FormItem>
                      {getFieldDecorator(
                        'tenancyName',
                        {}
                      )(<Input placeholder={L('TenancyName')} prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />} size="large" />)}
                    </FormItem>
                    {!getFieldValue('tenancyName') ? <div>{L('LeaveEmptyToSwitchToHost')}</div> : ''}
                  </Col>
                </Row>
              </Modal>
            </Row> */}
            <Row style={{ marginTop: 10 }}>
              <Col span={8} offset={8}>
                <StyledCard>
                  <img src={ITsutra} alt="" style={{ marginBottom: '25px', width: '99px' }} />
                  <StyledH1>{L('Login to Accounts')}</StyledH1>
                  <StyledLable>User Name or Email</StyledLable>
                  <FormItem>
                    {getFieldDecorator('userNameOrEmailAddress', { rules: rules.userNameOrEmailAddress })(
                      <StyledInput
                        placeholder={L('Enter Your User Name Or Email')}
                        prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />}
                        size="large"
                        allowClear
                      />
                    )}
                  </FormItem>
                  <StyledLable>Password</StyledLable>
                  <FormItem>
                    {getFieldDecorator('password', { rules: rules.password })(
                      <StyledInput
                        placeholder={L('Enter Your Password')}
                        prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />}
                        type="password"
                        size="large"
                        allowClear
                      />
                    )}
                  </FormItem>
                  <Row style={{ margin: '0px 0px 25px 5px ' }} type="flex" justify="space-between">
                    <Col>
                      <Checkbox checked={loginModel.rememberMe} onChange={loginModel.toggleRememberMe} style={{ paddingRight: 8 }} />
                      Remember Me
                    </Col>
                    <Col>
                      <a>{L('Forgot Password?')}</a>
                    </Col>
                  </Row>
                  <StyledButton htmlType={'submit'} type="primary">
                    Log In
                  </StyledButton>
                  <p style={{ textAlign: 'center', marginTop: '1em' }}>OR</p>
                  {this.props.authenticationStore!.externalAuthenticationProviders &&
                    this.props.authenticationStore!.externalAuthenticationProviders.map((info) => (
                      <StyledGoogle onClick={() => this.handleExternalLogin(info.name, info.clientId)} block>
                        <IconFont type="icongoogle" />
                        Sign in with Google
                      </StyledGoogle>
                    ))}
                  <StyledP>
                    Accounts - iTSutra, Inc © 2019
                  </StyledP>
                </StyledCard>
              </Col>
            </Row>
          </Row>
        </StyledForm>
      </Col>
    );
  }
}

export default Form.create()(Login);
