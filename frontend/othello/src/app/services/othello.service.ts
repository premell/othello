import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { environment } from './../../environments/environment';

import { Move, Mark, Color, Game, InvalidMove} from '../helper/models'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OthelloService {

  constructor(private http: HttpClient) { }

  getGame(id: number): Observable<Game[]>{
    return this.http.get<Game[]>(environment.apiBaseUrl + "/game/getGame?GameID="+ id)
  }

  createGame(timeLimit: number, timeIncrement: number): Observable<Game>{
    console.log("creating game...")

    return this.http.post<Game>(environment.apiBaseUrl + "/game/createGame", JSON.stringify({
      TimeLimit: timeLimit,
      TimeIncrement: timeIncrement,
    }))
  }
}
