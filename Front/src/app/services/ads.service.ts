import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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

  // --- GET list עם סינון ---
  list(q?: string, lat?: number, lng?: number, radiusKm?: number, skip: number = 0, take: number = 20): Observable<Ad[]> {
    let params = new HttpParams().set('skip', skip).set('take', take);
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
  create(ad: Ad): Observable<Ad> {
    return this.http.post<Ad>(this.base, ad);
  }

  // --- PUT (עדכון) ---
  update(id: string, ad: Ad): Observable<Ad> {
    return this.http.put<Ad>(`${this.base}/${id}`, ad);
  }

  // --- DELETE ---
  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
