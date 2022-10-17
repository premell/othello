import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { environment } from './../../environments/environment';

import { Move, Mark, Color, GameState, InvalidMove} from '../helper/models'

@Injectable({
  providedIn: 'root'
})
export class OthelloService {

  constructor(private http: HttpClient) { }

  getGame(): Observable<GameState[]>{
    return this.http.get(environment.apiBaseUrl + "/game/getGame")
  }
}
