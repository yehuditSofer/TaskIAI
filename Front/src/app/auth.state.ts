import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthState {
  email$ = new BehaviorSubject<string>('');
  setEmail(email: string){ this.email$.next(email); }
  get email(){ return this.email$.value; }
}