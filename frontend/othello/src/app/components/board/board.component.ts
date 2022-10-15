import { Component, OnInit } from '@angular/core'
import { Move, Mark, Color, GameState, InvalidMove} from '../../helper/models'
import { createDefaultState, placeMark, getOpponentColor} from '../../helper/functions'
import { trigger, transition, state, animate, style } from '@angular/animations';

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

  gameStates!: GameState[]
  gameStateToRender: GameState = createDefaultState()
  playerColor: Color = Color.Black

  constructor() {}

  ngOnInit(): void {

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

    const move: Move = {
      Color: this.playerColor,
      MoveNumber: 2,
      TargetSquare: squareNumber,
      RemainingTime: 500,
    }

    const result = placeMark(this.gameStateToRender, move)

    if ('Message' in result) console.log("NOOOOOO ERROR ERR AH")
    else {
      this.gameStateToRender = result as GameState
      this.playerColor = getOpponentColor(this.playerColor)

      markAttachedToCursor = document.getElementsByClassName(
        'mark_moving_with_cursor'
      )[0] as HTMLElement
        markAttachedToCursor.style.backgroundColor = this.playerColor === Color.White ? "white" : "black"
    }

  }
}
