import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { GoogleMap, GoogleMapsModule } from '@angular/google-maps';
import { SocialLoginModule, SocialAuthServiceConfig, GoogleLoginProvider } from '@abacritt/angularx-social-login';
import { AppComponent } from './app.component';
import { AdsListComponent } from './components/ads-list/ads-list.component';
import { AdFormComponent } from './components/ad-form/ad-form.component';
import { LoginComponent } from './components/login/login.component';
import { environment } from '../environments/environment';
import { MapPickerComponent } from './components/map-picker/map-picker.component';

const routes: Routes = [
  { path: '', component: AdsListComponent },
  { path: 'new', component: AdFormComponent },
  { path: 'edit/:id', component: AdFormComponent }
];

@NgModule({
  declarations: [AppComponent, AdsListComponent, AdFormComponent, LoginComponent, MapPickerComponent],
  imports: [BrowserModule, HttpClientModule, FormsModule, ReactiveFormsModule, RouterModule.forRoot(routes), GoogleMapsModule, SocialLoginModule],
  providers: [
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(environment.googleClientId)
          }
        ],
        onError: (err: any) => console.error(err)
      } as SocialAuthServiceConfig
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
