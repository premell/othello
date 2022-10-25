import { Component, OnInit } from '@angular/core'
import { NgbModal, NgbModalConfig} from '@ng-bootstrap/ng-bootstrap'
import { CreateGameModalComponent } from 'src/app/modals/create-game-modal/create-game-modal.component'
import {Color} from '../../helper/models'

import {OthelloService} from "../../services/othello.service"

@Component({
  selector: 'app-create-game',
  templateUrl: './create-game.component.html',
  styleUrls: ['./create-game.component.scss'],
})
export class CreateGameComponent implements OnInit {
  createGameMenuIsShowing: boolean = false
  timeLimitValue: any = 10;
  timeIncrementValue: any = 0;
  readonly Color = Color;

  

  constructor(config: NgbModalConfig, private modalService: NgbModal, private othelloService: OthelloService) {
	}

  ngOnInit(): void {}

  open(content: any): void {
    this.modalService.open(content, {
        modalDialogClass: 'medium-modal',
    })
  }

  createGame(color: 'black'|'white'|'random'): void {
    let playerColor: Color;
    if (color === 'random') playerColor = Math.round(Math.random()) === 1 ? Color.White : Color.Black
    else if(color === 'black') playerColor = Color.Black
    else if(color === 'white') playerColor = Color.White

    let id: string = ""
    this.othelloService.createGame(this.timeLimitValue, this.timeIncrementValue).subscribe()



  }
}
