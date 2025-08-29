import { Injectable } from '@angular/core';

export interface User {
  id: string;
  name: string;
  email: string;
  photoUrl?: string;
  idToken?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: User | null = null;

  setUser(user: User | null) {
    this.currentUser = user;
  }

  getUser(): User | null {
    return this.currentUser;
  }

  clearUser() {
    this.currentUser = null;
  }

  isLoggedIn(): boolean {
    return this.currentUser !== null;
  }

  isUser(idUser: string): boolean {
    return idUser == this.currentUser?.id;
  }
}
