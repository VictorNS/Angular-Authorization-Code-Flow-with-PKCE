export const environment = {
	production: true,
	oauthClientId: 'ang2',
	oauthCallbackUrl: 'http://localhost:4202/oauth/callback',

	oauthLoginUrl: 'https://localhost:5001/Account/Login',
	oauthLogoutUrl: 'https://localhost:5001/Account/Logout',
	oauthTokenUrl: 'https://localhost:5001/connect/token',
	oauthUserinfoUrl: 'https://localhost:5001/connect/userinfo',

	apiScope: 'api2',
	apiToken: 'https://localhost:6002/api/account/token',
	apiAuthorize: 'https://localhost:6002/api/account/authorize',
	apiUserInfo: 'https://localhost:6002/api/account/userinfo',
	apiAnonymous: 'https://localhost:6002/api/account/anonymous',
};
