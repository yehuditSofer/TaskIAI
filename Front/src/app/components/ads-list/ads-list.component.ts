import { Component } from '@angular/core';
import { AdsService, Ad } from '../../services/ads.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-ads-list',
  templateUrl: './ads-list.component.html',
  styleUrls: ['./ads-list.component.css']
})
export class AdsListComponent {
  q = '';
  ads: Ad[] = [];
  lat?: number; lng?: number; radiusKm?: number;
  constructor(private api: AdsService, private router: Router, private authService: AuthService) { this.load(); }
  load() {
    this.api.list(this.q, this.lat, this.lng, this.radiusKm).subscribe(res => this.ads = res);
  }
  useLocation() {
    navigator.geolocation.getCurrentPosition(p => {
      this.lat = p.coords.latitude; this.lng = p.coords.longitude;
      if (!this.radiusKm) this.radiusKm = 5;
      this.load();
    });
  }
  create() {
    this.router.navigate(['/new']);
  }
  edit(ad: Ad) {
    this.router.navigate(['/edit', ad.id]);
  }
  allowEdit(CreatedBy: string) {
    return this.authService.isUser(CreatedBy);
  }
}
