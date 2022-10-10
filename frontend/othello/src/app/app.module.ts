import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent }  from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { BoardComponent } from './components/board/board.component';

@NgModule({
  imports:      [ BrowserModule ],
  declarations: [ AppComponent, HomeComponent, BoardComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
