import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
	selector: 'lib-identity-callback',
	templateUrl: './identity-callback.component.html',
	styleUrls: ['./identity-callback.component.scss']
})
export class IdentityCallbackComponent implements OnInit {
	message: string = '';

	@Input()
	apiAuthorize: string = '';
	
	constructor(
        private readonly activatedRoute: ActivatedRoute,
		private readonly router: Router,
		private readonly http: HttpClient,
	) { }

	ngOnInit(): void {
		this.activatedRoute.queryParams.subscribe(params => {
            if (params.code) {
				this.message = 'in progres...';
				this.getCookieFromApi(params.code, params.state);
			} else {
				this.message = 'Something went wrong';
			}
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

		this.http.get<any>(this.apiAuthorize, {
			withCredentials: true,
			headers: {
				'code': code,
				'code_verifier': codeVerifier
			}
		}).subscribe(response => {
			console.warn('getCookieFromApi', response);
			this.router.navigate(['/']);
		}, error => {
			console.warn('getCookieFromApi', error);
		});
	}
}
