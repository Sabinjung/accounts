export interface AuthenticationResultModel {
  accessToken: string;
  encryptedAccessToken: string;
  expireInSeconds: number;
  userId: number;
}

export interface ExternalLoginProviderInfoModel {
  name: string;
  clientId: string;
}
