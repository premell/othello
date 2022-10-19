import { Component, OnInit } from '@angular/core';
import { Game } from 'src/app/helper/models';
import * as internal from 'stream';
//import { createDefaultState} from "../../helper/functions"


import { OthelloService } from '../../services/othello.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.sass']
})
export class HomeComponent implements OnInit {

  id: number = 0;
  //game!: Game;

  constructor(private othelloService: OthelloService) { }

  ngOnInit(): void {
    //this.othelloService.getGame(1).subscribe(id => console.log("here ", id))
    //this.othelloService.createGame(900, 63).subscribe(game => this.game = game)
    //createDefaultState()
  }
}
