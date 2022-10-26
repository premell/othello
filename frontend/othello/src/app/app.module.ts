import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { BoardComponent } from './components/board/board.component';
import { BoardSidePanelComponent } from './components/board-side-panel/board-side-panel.component';
import { GameComponent } from './pages/game/game.component';
import { CreateGameModalComponent } from './modals/create-game-modal/create-game-modal.component';
import { CreateGameComponent } from './components/create-game/create-game.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatSliderModule } from '@angular/material/slider';
import { MatIconModule } from '@angular/material/icon';
//import { MatIcon } from '@angular/material/icon';
//import { AppRoutingModule } from './app-routing.module'; // CLI imports AppRoutingModule

import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { Routes, RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';

import { GetStatePipe } from './pipes/getstate.pipe';
import { TopbarLinksComponent } from './components/topbar-links/topbar-links/topbar-links.component';

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'Game/:id/:color', component: BoardComponent },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    NgbModule,
    BrowserAnimationsModule,
    MatSliderModule,
    MatIconModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(appRoutes),
  ],
  declarations: [
    AppComponent,
    HomeComponent,
    BoardComponent,
    BoardSidePanelComponent,
    GameComponent,
    CreateGameComponent,
    CreateGameModalComponent,
    PageNotFoundComponent,
    GetStatePipe,
    TopbarLinksComponent,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
