import { Component, Input } from '@angular/core'
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'

@Component({
  selector: 'ngbd-modal-content',
  templateUrl: './create-game-modal.component.html',
  styleUrls: ['./create-game-modal.component.scss'],
})
export class CreateGameModalComponent {
  @Input() name: string = "";
  timeIncrementValue: any = 0;

  constructor(public activeModal: NgbActiveModal) {}
}

