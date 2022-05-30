import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss']
})
export class AppComponent {
	title = 'Client2';
	response: any;

	constructor(
		private http: HttpClient
	) { }

	callAuthorizedApi() {
		this.http.get<any>(environment.apiUserInfo, {
			withCredentials: true,
		}).subscribe(response => {
			this.response = response;
		}, error => {
			this.response = error;
		});
	}

	callAnonymousApi() {
		this.http.get<any>(environment.apiAnonymous, {
			withCredentials: true,
		}).subscribe(response => {
			this.response = response;
		}, error => {
			this.response = error;
		});
	}
}
