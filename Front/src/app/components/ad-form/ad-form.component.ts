import { Component, OnInit } from '@angular/core';
import { AdsService, Ad } from '../../services/ads.service';
import { Router } from '@angular/router';
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
  constructor(private adsService: AdsService, private authService: AuthService) { }

  ngOnInit() {
    this.user = this.authService.getUser();
  }

  onMapClick(event: google.maps.MapMouseEvent) {
    if (event.latLng) {
      this.marker = event.latLng.toJSON();
      this.ad.location = this.marker;
    }
  }

  save() {
    this.adsService.create(this.ad, this.user!.id).subscribe(res => {
      alert('מודעה נשמרה בהצלחה!');
      console.log(res);
    });
  }
}
