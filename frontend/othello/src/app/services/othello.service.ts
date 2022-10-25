import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { environment } from './../../environments/environment';

import { move, mark, Color, game, invalidMove} from '../helper/models'
import { Observable } from 'rxjs';



// Dummy data
let dummyGame: game= {
  id: "1",
  timeLimit: 500,
  timeIncrement: 5,
  gameStatus: "currently_playing",
  whiteTimeRemaining: 500,
  blackTimeRemaining: 500,
  moves: []
}

@Injectable({
  providedIn: 'root'
})
export class OthelloService {

  constructor(private http: HttpClient) { }

  getGame(id: number): Observable<game>{
    //return dummyGame
    return this.http.get<game>(environment.apiBaseUrl + "/game/getGame?GameID="+ id)
  }

  createGame(timeLimit: number, timeIncrement: number): Observable<string>{
    return this.http.post<string>(environment.apiBaseUrl + "/game/createGame", JSON.stringify({
      TimeLimit: timeLimit,
      TimeIncrement: timeIncrement,
    }))
  }
}
