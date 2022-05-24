import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@environments/environment';
import * as CryptoJS from 'crypto-js';

@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
	tokenResponse: any;
	userResponse: any;

	constructor(
        private activatedRoute: ActivatedRoute,
		private http: HttpClient
	) { }

    ngOnInit() {
		this.activatedRoute.queryParams.subscribe(params => {
            if (params.code) {
                // this.getAccessToken(params.code, params.state); // uncomment, if you want to do it from UI
				// this.getTokenFromApi(params.code, params.state); // uncomment, if you want just to get token
				this.getCookieFromApi(params.code, params.state);
            }
        });
	}

	goToLoginPage() {
		const state = this.strRandom(40);
		const codeVerifier = this.strRandom(128);
		localStorage.setItem('state', state);
		localStorage.setItem('codeVerifier', codeVerifier);
		const codeVerifierHash = CryptoJS.SHA256(codeVerifier).toString(CryptoJS.enc.Base64);
		const codeChallenge = codeVerifierHash
			.replace(/=/g, '')
			.replace(/\+/g, '-')
			.replace(/\//g, '_');

		const params = [
			'client_id=' + environment.oauthClientId,
			'redirect_uri=' + encodeURIComponent(environment.oauthCallbackUrl),
			'response_type=code',
			'scope=' + encodeURIComponent('openid profile api1 IdentityServerApi'),
			'state=' + state,
			'code_challenge=' + codeChallenge,
			'code_challenge_method=S256',
			'response_mode=query'
		];
		const encoded = encodeURIComponent('/connect/authorize/callback?' + params.join('&'));

		window.location.href = environment.oauthLoginUrl + '?ReturnUrl=' + encoded;
	}

	getAccessToken(code: string, state: string) {
		if (state !== localStorage.getItem('state')) {
            alert('Invalid callBack state');
            return;
		}

		const codeVerifier = localStorage.getItem('codeVerifier');
		if (!codeVerifier) {
            alert('codeVerifier in localSotage is expected');
            return;
		}

		const payload = new HttpParams()
            .append('grant_type', 'authorization_code')
            .append('code', code)
            .append('code_verifier', codeVerifier)
            .append('redirect_uri', environment.oauthCallbackUrl)
            .append('client_id', environment.oauthClientId);

		this.http.post(environment.oauthTokenUrl, payload, {
			headers: {
				'Content-Type': 'application/x-www-form-urlencoded'
			}
		}).subscribe(response => {
			this.tokenResponse = response;
			this.getUserInfo();
		}, error => {
			console.warn('HTTP Error', error);
		});
    }

	getUserInfo() {
		if (!this.tokenResponse.access_token) {
			alert('accessToken is empty');
			return;
		}

		this.http.get(environment.oauthUserinfoUrl, {
			headers: {
				'Authorization': 'Bearer ' + this.tokenResponse.access_token
			}
		}).subscribe(response => {
			this.userResponse = response;
		}, error => {
			console.warn('HTTP Error', error);
		});
	}

	getTokenFromApi(code: string, state: string) {
		if (state !== localStorage.getItem('state')) {
            alert('Invalid callBack state');
            return;
		}

		const codeVerifier = localStorage.getItem('codeVerifier');
		if (!codeVerifier) {
            alert('codeVerifier in localSotage is expected');
            return;
		}

		this.http.get<any>(environment.apiToken, {
			headers: {
				'code': code,
				'code_verifier': codeVerifier
			}
		}).subscribe(response => {
			this.tokenResponse = response.tokenResponse;
			this.userResponse = response.userResponse;
		}, error => {
			console.warn('HTTP Error', error);
		});
    }

	getCookieFromApi(code: string, state: string) {
		if (state !== localStorage.getItem('state')) {
            alert('Invalid callBack state');
            return;
		}

		const codeVerifier = localStorage.getItem('codeVerifier');
		if (!codeVerifier) {
            alert('codeVerifier in localSotage is expected');
            return;
		}

		this.http.get<any>(environment.apiAuthorize, {
			withCredentials: true,
			headers: {
				'code': code,
				'code_verifier': codeVerifier
			}
		}).subscribe(response => {
			this.userResponse = response;
		}, error => {
			console.warn('HTTP Error', error);
		});
	}

	callApi() {
		this.http.get<any>(environment.apiUserInfo, {
			withCredentials: true,
		}).subscribe(response => {
			this.userResponse = response;
		}, error => {
			console.warn('HTTP Error', error);
		});
	}

	callAnonymous() {
		this.http.get<any>(environment.apiAnonymous, {
			withCredentials: true,
		}).subscribe(response => {
			console.warn(response);
		}, error => {
			console.warn('HTTP Error', error);
		});
	}

	goToLogoutPage() {
		window.location.href = environment.oauthLogoutUrl+'?ReturnUrl=' + encodeURIComponent(environment.oauthCallbackUrl);
	}

	private strRandom(length: number) {
		let result = '';
		const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
		const charactersLength = characters.length;
		for (let i = 0; i < length; i++) {
			result += characters.charAt(Math.floor(Math.random() * charactersLength));
		}
		return result;
	}
}
