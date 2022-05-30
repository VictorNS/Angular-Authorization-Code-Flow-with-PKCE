import { Component, Input, OnInit } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Component({
	selector: 'lib-identity-redirect',
	templateUrl: './identity-redirect.component.html',
	styleUrls: ['./identity-redirect.component.scss']
})
export class IdentityRedirectComponent implements OnInit {

	@Input()
	oauthClientId: string = '';
	@Input()
	oauthCallbackUrl: string = '';
	@Input()
	oauthLoginUrl: string = '';

	constructor() { }

	ngOnInit(): void {
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
			'scope=' + encodeURIComponent('openid profile api1 IdentityServerApi'),
			'state=' + state,
			'code_challenge=' + codeChallenge,
			'code_challenge_method=S256',
			'response_mode=query'
		];
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
