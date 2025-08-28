import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Location { lat: number; lng: number; }
export interface Ad {
  id?: string;
  title: string;
  description: string;
  createdBy: string;
  location?: Location | null;
  createdAt?: string;
  updatedAt?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AdsService {
  private base = '/api/ads';

  constructor(private http: HttpClient) { }

  private headers(email?: string) {
    const h: any = {};
    if (email) h['X-User-Email'] = email;
    return new HttpHeaders(h);
  }

  // --- GET list עם סינון ---
  list(q?: string, lat?: number, lng?: number, radiusKm?: number): Observable<Ad[]> {
    let params = new HttpParams();
    if (q) params = params.set('q', q);
    if (lat != null && lng != null && radiusKm != null) {
      params = params.set('lat', lat).set('lng', lng).set('radiusKm', radiusKm);
    }
    return this.http.get<Ad[]>(this.base, { params });
  }

  // --- GET by Id ---
  get(id: string): Observable<Ad> {
    return this.http.get<Ad>(`${this.base}/${id}`);
  }

  // --- POST (יצירה) ---
  create(ad: Ad, email: string): Observable<Ad> {
    return this.http.post<Ad>(this.base, ad, { headers: this.headers(email) });
  }

  // --- PUT (עדכון) ---
  update(id: string, ad: Ad, email: string): Observable<Ad> {
    return this.http.put<Ad>(`${this.base}/${id}`, ad, { headers: this.headers(email) });
  }

  // --- DELETE ---
  remove(id: string, email: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`, { headers: this.headers(email) });
  }
}
