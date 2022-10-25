import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { environment } from './../../environments/environment';

import { Move, Mark, Color, Game, InvalidMove} from '../helper/models'
import { Observable } from 'rxjs';



// Dummy data
let dummyGame: Game= {
  Id: "1",
  TimeLimit: 500,
  TimeIncrement: 5,
  GameStatus: "currently_playing",
  WhiteTimeRemaining: 500,
  BlackTimeRemaining: 500,
  Moves: []
}

@Injectable({
  providedIn: 'root'
})
export class OthelloService {

  constructor(private http: HttpClient) { }

  getGame(id: number): Game{
    return dummyGame
    //return this.http.get<Game>(environment.apiBaseUrl + "/game/getGame?GameID="+ id)
  }

  createGame(timeLimit: number, timeIncrement: number): Observable<string>{
    return this.http.post<string>(environment.apiBaseUrl + "/game/createGame", JSON.stringify({
      TimeLimit: timeLimit,
      TimeIncrement: timeIncrement,
    }))
  }
}
