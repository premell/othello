import { Component, OnInit } from '@angular/core'
import { move, mark, Color, game, invalidMove, attemptMove} from '../../helper/models'
import { placeMark, getStartingState, getOpponentColor} from '../../helper/functions'

import { trigger, transition, state, animate, style } from '@angular/animations';

import {OthelloService} from "../../services/othello.service"
import { ActivatedRoute, Router } from '@angular/router';

import {Observable, timer, switchMap} from 'rxjs';

let markAttachedToCursor: HTMLElement
let referenceMarkOnBoard: HTMLElement

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss'],
})
export class BoardComponent implements OnInit {
  // you have to do this so you can use it in the template
  readonly Color = Color
  game!: game
  gameStateToRender!: number[]
  playerColor: Color = Color.Black

  test$: Observable<game>;

  test2!: game;



  constructor(private othelloService: OthelloService,
    private activeRoute: ActivatedRoute,
    private route: Router
     ) {
      this.test$ = timer(0, 1000).pipe(
        switchMap(() => this.othelloService.getGame(1))
        )


      timer(0, 1000).pipe(
        switchMap(() => this.othelloService.getGame(1))
        ).subscribe(k => this.test2 = k)
     }



  ngOnInit(): void {
    const id: string = this.activeRoute.snapshot.params['id']
    const color: number = parseInt(this.activeRoute.snapshot.params['color'])


    console.log("color ", color)
    // if the url parameters doesnt make sense, return 404 
    if(!(/^[0-9]+$/).test(id) || (color !== Color.Black && color !== Color.White)) this.route.navigate(['404'])

    this.playerColor = color === Color.White ? Color.White : Color.Black
    //this.game = this.othelloService.getGame(parseInt(id))


    if(this.game.moves.length === 0) this.gameStateToRender = getStartingState()

    window.addEventListener('mousemove', (e) =>
      this.moveMarkToCursor(e.clientX, e.clientY)
    )
    window.addEventListener('resize', () => this.resizeCursorMark())

    const board = document.getElementsByClassName('board')[0]
    board.addEventListener('mouseenter', () => this.cursorEnterBoard())
    board.addEventListener('mouseleave', () => this.cursorLeaveBoard())
  }

  isPlayerTurn(): boolean {
    return true
  }

  // a reference mark to get the element width
  getReferenceMark(): HTMLElement {
    if (referenceMarkOnBoard === undefined || referenceMarkOnBoard === null)
      referenceMarkOnBoard = document.getElementsByClassName(
        'mark'
      )[1] as HTMLElement

    return referenceMarkOnBoard
  }
  getMarkAttachedToCursor(): HTMLElement {
    if (markAttachedToCursor === undefined || markAttachedToCursor === null)
      markAttachedToCursor = document.getElementsByClassName(
        'mark_moving_with_cursor'
      )[0] as HTMLElement

    return markAttachedToCursor
  }

  moveMarkToCursor(mouseX: number, mouseY: number): void {
    const mark = this.getMarkAttachedToCursor()
    const markWidth = mark.clientWidth
    const markHeight = mark.clientHeight

    const widthOffset = mouseY - markWidth / 2
    const heightOffset = mouseX - markHeight / 2

    mark.style.top = widthOffset + 'px'
    mark.style.left = heightOffset + 'px'

    if (mark.clientWidth === 0) this.resizeCursorMark()
  }

  resizeCursorMark(): void {
    const mark = this.getMarkAttachedToCursor()
    const referenceMark = this.getReferenceMark()

    mark.style.width = referenceMark.clientWidth + 'px'
    mark.style.height = referenceMark.clientHeight + 'px'
  }

  cursorEnterBoard(): void {
    const mark = this.getMarkAttachedToCursor()
    if (this.isPlayerTurn()) mark.style.visibility = 'visible'
  }

  cursorLeaveBoard(): void {
    const mark = this.getMarkAttachedToCursor()

    mark.style.visibility = 'hidden'
  }

  handleSquareClicked = (squareNumber: number) => {
    console.log(squareNumber)

    // TODO
    if(!this.isPlayerTurn()) return

    const move: attemptMove  = {
      playerColor: this.playerColor,
      moveNumber: 2,
      targetSquare: squareNumber,
      remainingTime: 500,
    }

    const result = placeMark(this.game, move)

    //...BoardComponent.
    if ('message' in result) console.log("NOOOOOO ERROR ERR AH")
    else {
      this.game.moves.push(result)
      this.gameStateToRender = result.resultingState
      this.playerColor = getOpponentColor(this.playerColor)

      markAttachedToCursor = document.getElementsByClassName(
        'mark_moving_with_cursor'
      )[0] as HTMLElement
        markAttachedToCursor.style.backgroundColor = this.playerColor === Color.White ? "white" : "black"
    }

  }

  printTest =() => {
    console.log("test")
    console.log(this.test$)
    console.log(this.test2)
  }
}


