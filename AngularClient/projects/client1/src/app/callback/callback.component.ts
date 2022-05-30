import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
	selector: 'app-callback',
	templateUrl: './callback.component.html',
	styleUrls: ['./callback.component.scss']
})
export class CallbackComponent implements OnInit {
	env;

	constructor() {
		this.env = environment;
	}

	ngOnInit(): void {
	}
}
