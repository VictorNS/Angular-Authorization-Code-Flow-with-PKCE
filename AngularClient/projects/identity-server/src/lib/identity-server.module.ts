import { NgModule } from '@angular/core';
import { IdentityServerComponent } from './identity-server.component';
import { IdentityRedirectComponent } from './identity-redirect/identity-redirect.component';
import { IdentityCallbackComponent } from './identity-callback/identity-callback.component';

@NgModule({
  declarations: [
    IdentityServerComponent,
    IdentityRedirectComponent,
    IdentityCallbackComponent
  ],
  imports: [
  ],
  exports: [
    IdentityServerComponent,
    IdentityRedirectComponent,
    IdentityCallbackComponent
  ]
})
export class IdentityServerModule { }
