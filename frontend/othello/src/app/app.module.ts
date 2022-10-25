import { NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { HttpClientModule } from '@angular/common/http'
import { AppComponent } from './app.component'
import { HomeComponent } from './pages/home/home.component'
import { BoardComponent } from './components/board/board.component'
import { BoardSidePanelComponent } from './components/board-side-panel/board-side-panel.component'
import { GameComponent } from './pages/game/game.component'
import { CreateGameModalComponent } from './modals/create-game-modal/create-game-modal.component'
import { CreateGameComponent } from './components/create-game/create-game.component'

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'

import { MatSliderModule } from '@angular/material/slider';

import {FormsModule, ReactiveFormsModule} from '@angular/forms';

@NgModule({
  imports: [BrowserModule, HttpClientModule, NgbModule, BrowserAnimationsModule, MatSliderModule, FormsModule, ReactiveFormsModule],
  declarations: [
    AppComponent,
    HomeComponent,
    BoardComponent,
    BoardSidePanelComponent,
    GameComponent,
    CreateGameComponent,
    CreateGameModalComponent,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
