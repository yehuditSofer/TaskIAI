import { Component, OnInit } from '@angular/core';
import { AdsService, Ad } from '../../services/ads.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService, User } from '../../services/auth.service';

@Component({
  selector: 'app-ad-form',
  templateUrl: './ad-form.component.html',
  styleUrls: ['./ad-form.component.css']
})
export class AdFormComponent implements OnInit {
  ad: Ad = { title: '', description: '', createdBy: '' };
  center = { lat: 32.0853, lng: 34.7818 }; // תל אביב כברירת מחדל
  marker: google.maps.LatLngLiteral | null = null;
  user: User | null = null;
  isEdit = false;

  constructor(
    private adsService: AdsService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    if (!this.authService.isLoggedIn())
      this.router.navigate(['/']);
    this.user = this.authService.getUser();

    // בודקים אם יש id בפרמטרים => מצב עריכה
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEdit = true;
        this.adsService.get(id).subscribe(ad => {
          this.ad = ad;
          if (ad.location) {
            this.marker = ad.location;
            this.center = ad.location;
          }
        });
      }
    });
  }

  onMapClick(event: google.maps.MapMouseEvent) {
    if (event.latLng) {
      this.marker = event.latLng.toJSON();
      this.ad.location = this.marker;
    }
  }

  save() {
    this.ad.createdBy = this.user!.id;

    if (this.isEdit && this.ad.id) {
      // עדכון
      this.adsService.update(this.ad.id, this.ad).subscribe(res => {
        alert('מודעה עודכנה בהצלחה!');
        this.router.navigate(['/']);
      });
    } else {
      // יצירה
      this.adsService.create(this.ad).subscribe(res => {
        alert('מודעה נשמרה בהצלחה!');
        this.router.navigate(['/']);
      });
    }
  }
}
