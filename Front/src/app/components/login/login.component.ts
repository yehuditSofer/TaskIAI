import { Component, EventEmitter, Output } from '@angular/core';
import { SocialAuthService, GoogleLoginProvider, SocialUser } from '@abacritt/angularx-social-login';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  @Output() emailChange = new EventEmitter<string>();
  name = '';
  constructor(private auth: SocialAuthService, private authService: AuthService) {
    this.auth.authState.subscribe((user: SocialUser | null) => {
      // שומרים את המשתמש הגלובלי
      this.authService.setUser({
        id: user!.id,
        name: user!.name,
        email: user!.email,
        photoUrl: user!.photoUrl,
        idToken: user!.idToken
      });
      localStorage.setItem('google_id_token', user!.idToken); // 🔑 כאן נשמר
      this.name = user?.name ?? '';
      this.emailChange.emit(user!.email);
    });
  }

  signIn() {
    this.auth.signIn(GoogleLoginProvider.PROVIDER_ID);
  }

  signOut() {
    this.auth.signOut(true).then(() => {
      console.log("Signed out");
      this.authService.setUser(null);
      localStorage.removeItem('google_id_token');
      this.name = '';
      this.emailChange.emit('');
    }).catch(err => {
      console.error("Sign out failed:", err);
    });
  }
}
