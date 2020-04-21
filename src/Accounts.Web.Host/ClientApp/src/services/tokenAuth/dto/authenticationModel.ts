export interface AuthenticationModel {
  userNameOrEmailAddress: string;
  password: string;
  rememberClient: boolean;
}

export interface ExternalAuthenticationModel {
  providerAccessCode: string;
  authProvider: string;
}
