import { DOCUMENT } from "@angular/common";
import { Inject, Injectable } from "@angular/core";
import { THEMES } from "@constants";
import { theme } from "@models";

@Injectable({
  providedIn: "root"
})
export class ColorSchemeService {
  private colorScheme: string = 'dark';

  constructor(@Inject(DOCUMENT) private document: Document) {}

    _detectPrefersColorScheme() {
        // Detect if prefers-color-scheme is supported
        if (window.matchMedia('(prefers-color-scheme)').media !== 'not all') {
            // Set colorScheme to Dark if prefers-color-scheme is dark. Otherwise, set it to Light.
            this.colorScheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        } else {
            // If the browser does not support prefers-color-scheme, set the default to dark.
            this.colorScheme = 'dark';
        }
    }

    _setColorScheme(scheme: string) {
        this.colorScheme = scheme;
        // Save prefers-color-scheme to localStorage
        localStorage.setItem('prefers-color', scheme);
    }

    _getColorScheme() {
        const localStorageColorScheme = localStorage.getItem('prefers-color');
        // Check if any prefers-color-scheme is stored in localStorage
        if (localStorageColorScheme) {
            // Save prefers-color-scheme from localStorage
            this.colorScheme = localStorageColorScheme;
        } else {
            // If no prefers-color-scheme is stored in localStorage, try to detect OS default prefers-color-scheme
            this._detectPrefersColorScheme();
        }
    }

    load() {
        this._getColorScheme();
        this.setTheme(this.colorScheme)
    }

  setTheme(name = "dark") {
    const theme: theme = THEMES[name as keyof typeof THEMES];
    this.colorScheme = name
    Object.keys(theme).forEach((key) => {
      this.document.documentElement.style.setProperty(`--${key}`, theme[key as keyof typeof theme]);
    });
  }

  currentActive(){
    return this.colorScheme;
  }

  toggleTheme(){
    this.setTheme(this.colorScheme === 'dark' ? 'light' : 'dark')
  }
}
