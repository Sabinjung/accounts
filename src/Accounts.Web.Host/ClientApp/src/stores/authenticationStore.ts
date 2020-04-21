import { action, observable, runInAction } from 'mobx';

import AppConsts from './../lib/appconst';
import LoginModel from '../models/Login/loginModel';
import tokenAuthService from '../services/tokenAuth/tokenAuthService';
import { ExternalAuthenticationModel } from '../services/tokenAuth/dto/authenticationModel';
import { ExternalLoginProviderInfoModel } from '../services/tokenAuth/dto/authenticationResultModel';

declare var abp: any;

class AuthenticationStore {
  @observable loginModel: LoginModel = new LoginModel();

  @observable externalAuthenticationProviders!: Array<ExternalLoginProviderInfoModel>;

  get isAuthenticated(): boolean {
    if (!abp.session.userId) return false;

    return true;
  }

  @action
  public async login(model: LoginModel) {
    let result = await tokenAuthService.authenticate({
      userNameOrEmailAddress: model.userNameOrEmailAddress,
      password: model.password,
      rememberClient: model.rememberMe,
    });

    var tokenExpireDate = model.rememberMe ? new Date(new Date().getTime() + 1000 * result.expireInSeconds) : undefined;
    abp.auth.setToken(result.accessToken, tokenExpireDate);
    abp.utils.setCookieValue(AppConsts.authorization.encrptedAuthTokenName, result.encryptedAccessToken, tokenExpireDate, abp.appPath);
  }

  @action
  public async externalLogin(input: ExternalAuthenticationModel): Promise<any> {
    try {
      let result = await tokenAuthService.externalAuthenticate(input);
      var tokenExpireDate = new Date(new Date().getTime() + 1000 * result.expireInSeconds);
      abp.auth.setToken(result.accessToken, tokenExpireDate);
      abp.utils.setCookieValue(AppConsts.authorization.encrptedAuthTokenName, result.encryptedAccessToken, tokenExpireDate, abp.appPath);
      return {
        isAuthenticationSuccessful: true,
      };
    } catch (err) {
      return {
        isAuthenticationSuccessful: false,
        error: err,
      };
    }
  }

  @action
  public async getExternalAuthenticationProviders() {
    let result = await tokenAuthService.getExternalAuthenticationProviders();
    runInAction(() => {
      this.externalAuthenticationProviders = result;
    });
  }

  @action
  logout() {
    localStorage.clear();
    sessionStorage.clear();
    abp.auth.clearToken();
  }

  @action
  async refresh() {
    abp.auth.clearToken();
    let result = await tokenAuthService.refresh();

    var tokenExpireDate = undefined;
    abp.auth.setToken(result.accessToken, tokenExpireDate);
    abp.utils.setCookieValue(AppConsts.authorization.encrptedAuthTokenName, result.encryptedAccessToken, tokenExpireDate, abp.appPath);
  }
}
export default AuthenticationStore;
