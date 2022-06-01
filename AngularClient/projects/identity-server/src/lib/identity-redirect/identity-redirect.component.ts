import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as CryptoJS from 'crypto-js';

@Component({
	selector: 'lib-identity-redirect',
	templateUrl: './identity-redirect.component.html',
	styleUrls: ['./identity-redirect.component.scss']
})
export class IdentityRedirectComponent implements OnInit {
	message: string = 'Validate current user information';

	@Input()
	oauthClientId: string = '';
	@Input()
	oauthCallbackUrl: string = '';
	@Input()
	oauthLoginUrl: string = '';
	@Input()
	apiScope: string = '';
	@Input()
	apiUserInfo: string = '';

	constructor(
		private readonly router: Router,
		private readonly http: HttpClient
	) { }

	ngOnInit(): void {
		this.http.get<any>(this.apiUserInfo, {
			withCredentials: true,
			headers: {
				'check_session': 'true'
			}
		}).subscribe(response => {
			this.router.navigate(['/']);
		}, (error: any) => {
			if (error.status === 401 || error.status === 403) {
				this.redirect();
			} else {
				this.message = 'Something went wrong';
			}
		});
	}

	redirect(): void {
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
			'client_id=' + this.oauthClientId,
			'redirect_uri=' + encodeURIComponent(this.oauthCallbackUrl),
			'response_type=code',
			'scope=' + encodeURIComponent('openid profile IdentityServerApi ' + this.apiScope),
			'state=' + state,
			'code_challenge=' + codeChallenge,
			'code_challenge_method=S256',
			'response_mode=query'
		];
		console.warn('callback', params.join('&'));
		const encoded = encodeURIComponent('/connect/authorize/callback?' + params.join('&'));

		window.location.href = this.oauthLoginUrl + '?ReturnUrl=' + encoded;
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
