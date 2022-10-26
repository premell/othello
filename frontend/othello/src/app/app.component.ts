import { Component } from '@angular/core';
import { ColorSchemeService } from './services/color-scheme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'othello';
  
  constructor(private colorSchemeService: ColorSchemeService) {
    this.colorSchemeService.load();
  }
}


