import { Component, HostListener, OnInit } from '@angular/core';
import { AdsService, Ad } from '../../services/ads.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-ads-list',
  templateUrl: './ads-list.component.html',
  styleUrls: ['./ads-list.component.css']
})
export class AdsListComponent implements OnInit {
  q = '';
  ads: Ad[] = [];
  skip = 0;
  take = 20;
  loading = false;
  lat?: number;
  lng?: number;
  radiusKm?: number;

  constructor(private api: AdsService, private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    if (this.loading) return;
    this.loading = true;

    this.api.list(this.q, this.lat, this.lng, this.radiusKm, this.skip, this.take).subscribe(res => {
      this.ads = [...this.ads, ...res];   // מוסיפים תוצאות חדשות
      this.skip += this.take;             // מקדמים skip לטעינה הבאה
      this.loading = false;
    });
  }

  @HostListener('window:scroll', [])
  onScroll() {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
      this.load();
    }
  }

  useLocation() {
    navigator.geolocation.getCurrentPosition(p => {
      this.lat = p.coords.latitude;
      this.lng = p.coords.longitude;
      if (!this.radiusKm) this.radiusKm = 5;
      this.resetAndLoad();   // נבצע חיפוש מחדש מהשרת לפי מיקום
    });
  }

  create() {
    this.router.navigate(['/new']);
  }

  edit(ad: Ad) {
    this.router.navigate(['/edit', ad.id]);
  }

  remove(ad: Ad) {
    if (confirm('האם ברצונך למחוק מודעה זו?')) {
      this.api.remove(ad.id!).subscribe(() => {
        this.ads = this.ads.filter(a => a.id !== ad.id);
        alert('המודעה נמחקה בהצלחה');
      });
    }
  }

  allowEdit(CreatedBy: string) {
    return this.authService.isUser(CreatedBy);
  }

  isLogin() {
    return this.authService.isLoggedIn();
  }

  // פונקציה לחיפוש – מאפסת את הרשימה ונטענת מהשרת
  search() {
    this.resetAndLoad();
  }

  private resetAndLoad() {
    this.ads = [];
    this.skip = 0;
    this.load();
  }
}
