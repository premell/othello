import { Component, OnInit } from '@angular/core'
import { NgbModal,NgbModalRef, NgbModalConfig} from '@ng-bootstrap/ng-bootstrap'
import { firstValueFrom, take } from 'rxjs'
import { CreateGameModalComponent } from 'src/app/modals/create-game-modal/create-game-modal.component'
import { runInContext } from 'vm'
import {Color} from '../../helper/models'

import { Router } from '@angular/router';
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

  modalReference!: NgbModalRef;

  

  constructor(config: NgbModalConfig, private modalService: NgbModal, private othelloService: OthelloService, private route: Router) {
	}

  ngOnInit(): void {}

  open(content: any): void {
      this.modalReference = this.modalService.open(content, {
        modalDialogClass: 'medium-modal',
    })
  }

  async createGame(color: 'black'|'white'|'random'): Promise<void> {
    let playerColor: Color;
    if (color === 'random') playerColor = Math.round(Math.random()) === 1 ? Color.White : Color.Black
    else if(color === 'black') playerColor = Color.Black
    else playerColor = Color.White

    //let id: string = ""
    const id = await firstValueFrom(this.othelloService.createGame(this.timeLimitValue, this.timeIncrementValue))

    console.log("ID ", id)

    this.route.navigate(['Game', id, playerColor] )

    if(this.modalReference) this.modalReference.close()
    console.log("after?")



    //const id: string = await this.service.getData().pipe(take(1)).toPromise();
//this.data = this.modifyMyData(data);


  }
}
