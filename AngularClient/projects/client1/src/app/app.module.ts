import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { CallbackComponent } from './callback/callback.component';
import { IdentityRedirectComponent } from 'projects/identity-server/src/lib/identity-redirect/identity-redirect.component';
import { IdentityCallbackComponent } from 'projects/identity-server/src/lib/identity-callback/identity-callback.component';

@NgModule({
	declarations: [
		AppComponent,
		LoginComponent,
		CallbackComponent,
		IdentityRedirectComponent,
		IdentityCallbackComponent
	],
	imports: [
		BrowserModule,
		HttpClientModule,
		AppRoutingModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
