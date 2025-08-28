import { Component, EventEmitter, Output } from '@angular/core';
import { SocialAuthService, GoogleLoginProvider, SocialUser } from '@abacritt/angularx-social-login';
import { AuthState } from '../../auth.state';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  @Output() emailChange = new EventEmitter<string>();
  email = '';
  name = '';
  constructor(private auth: SocialAuthService, private state: AuthState, private authService: AuthService) {
    this.auth.authState.subscribe((user: SocialUser | null) => {
        // שומרים את המשתמש הגלובלי
        this.authService.setUser({
          id: user!.id,
          name: user!.name,
          email: user!.email,
          photoUrl: user!.photoUrl,
          idToken: user!.idToken
        });
        this.name = user?.name ?? '';
        this.state.setEmail(this.email);
        this.emailChange.emit(this.email);
      
    });
  }

  signIn() { this.auth.signIn(GoogleLoginProvider.PROVIDER_ID); }
  signOut() { this.auth.signOut(); }
}
