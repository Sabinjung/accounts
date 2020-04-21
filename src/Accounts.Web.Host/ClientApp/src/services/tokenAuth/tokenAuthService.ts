import { AuthenticationModel, ExternalAuthenticationModel } from './dto/authenticationModel';
import { AuthenticationResultModel, ExternalLoginProviderInfoModel } from './dto/authenticationResultModel';
import http from '../httpService';

class TokenAuthService {
  public async authenticate(authenticationInput: AuthenticationModel): Promise<AuthenticationResultModel> {
    let result = await http.post('api/TokenAuth/Authenticate', authenticationInput);
    return result.data.result;
  }

  public async refresh(): Promise<AuthenticationResultModel> {
    let result = await http.post('api/TokenAuth/refresh');
    return result.data.result;
  }

  public async externalAuthenticate(externalAuthenticationInput: ExternalAuthenticationModel): Promise<AuthenticationResultModel> {
    let result = await http.post('api/TokenAuth/ExternalAuthenticate', externalAuthenticationInput);
    return result.data.result;
  }

  public async getExternalAuthenticationProviders(): Promise<Array<ExternalLoginProviderInfoModel>> {
    let result = await http.get('api/TokenAuth/GetExternalAuthenticationProviders');
    return result.data.result;
  }
}

export default new TokenAuthService();
