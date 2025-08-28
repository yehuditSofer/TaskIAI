import { Component } from '@angular/core';

@Component({
  selector: 'app-map-picker',
  templateUrl: './map-picker.component.html',
  styleUrls: ['./map-picker.component.css']
})
export class MapPickerComponent {
  center = { lat: 32.0853, lng: 34.7818 }; // תל אביב כברירת מחדל
  selected: google.maps.LatLngLiteral | null = null;

  onMapClick(event: google.maps.MapMouseEvent) {
    if (event.latLng) {
      this.selected = event.latLng.toJSON();
    }
  }
}
