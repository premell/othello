import { Component } from '@angular/core';
import { ColorSchemeService } from 'src/app/services/color-scheme.service';

@Component({
  selector: 'app-topbar-links',
  templateUrl: './topbar-links.component.html',
  styleUrls: ['./topbar-links.component.scss']
})
export class TopbarLinksComponent {

  public themes = [
    {
        name: 'dark',
        icon: 'brightness_3'
    },
    {
        name: 'light',
        icon: 'wb_sunny'
    }
];

constructor(public colorSchemeService: ColorSchemeService) {
}

/* setTheme(themeName: string) {
    this.colorSchemeService.setTheme(themeName);
} */

}
