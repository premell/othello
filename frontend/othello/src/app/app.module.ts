import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule } from "@angular/common/http"
import { AppComponent }  from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { BoardComponent } from './components/board/board.component';
import { BoardSidePanelComponent } from './components/board-side-panel/board-side-panel.component';

@NgModule({
  imports:      [ BrowserModule, HttpClientModule ],
  declarations: [ AppComponent, HomeComponent, BoardComponent, BoardSidePanelComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
