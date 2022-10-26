import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { environment } from './../../environments/environment';

import { move, mark, Color, game, invalidMove} from '../helper/models'
import { Observable } from 'rxjs';
import { map } from "rxjs/operators";
import { getStateFromString, getStringFromState } from '../helper/functions';


// Dummy data
let dummyGame: game= {
  id: 1,
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

  createGame(timeLimit: number, timeIncrement: number): Observable<string>{
    return this.http.post<string>(environment.apiBaseUrl + "/game/createGame", JSON.stringify({
      timeLimit: timeLimit,
      timeIncrement: timeIncrement,
    }))
  }

  getGame(id: number): Observable<game>{
    // TODO somehow transform incoming string to array of numbers
    return this.http.get<game>(environment.apiBaseUrl + "/game/getGame?GameID="+ id).pipe(map(game => {
      // this means that no game was found
      if(game.id === 0) return game

      let formattedGame: game = game;
      formattedGame.moves = game.moves || []
      formattedGame.moves.forEach(move => {
        move.resultingState = getStateFromString(move.resultingState)
      });
      return game
    }))
  }

  makeMove(move: move, gameid: number): Observable<string>{
    console.log(getStringFromState(move.resultingState))
    console.log(gameid)
    return this.http.post<string>(environment.apiBaseUrl + "/game/makeMove", JSON.stringify({
      gameId: gameid,
      playerColor: move.playerColor,
      moveNumber: move.moveNumber,
      targetSquare: move.targetSquare,
      playerRemainingTime: move.remainingTime,
      resultingState: getStringFromState(move.resultingState)
    }))
  }
}
