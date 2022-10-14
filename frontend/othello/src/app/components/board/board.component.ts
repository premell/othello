import { Component, OnInit } from '@angular/core'
import { Move, Mark, Color, GameState } from '../../helper/models'
import { createDefaultState, placeMark} from '../../helper/functions'

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

  // squareNumbers: number[] = [...Array(64).keys()]
  // marks: Mark[] = [
  //   {
  //     SquareNumber: 10,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 15,
  //     Color: Color.White,
  //   },
  //   {
  //     SquareNumber: 11,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 16,
  //     Color: Color.White,
  //   },
  // ]

  constructor() {}

  ngOnInit(): void {
    let testMove: Move = {
      Color: Color.Black,
      MoveNumber: 1,
      TargetSquare: 37,
      RemainingTime: 500,
    }
    placeMark(this.gameStateToRender, testMove)
    window.addEventListener('mousemove', (e) =>
      this.moveMarkToCursor(e.clientX, e.clientY)
    )
    window.addEventListener('resize', () => this.resizeCursorMark())

    const board = document.getElementsByClassName('board')[0]
    console.log(board)
    board.addEventListener('mouseenter', () => this.cursorEnterBoard())
    board.addEventListener('mouseleave', () => this.cursorLeaveBoard())
  }

  // squareContainsMark(squareNumber: number): boolean {
  //   return !!this.marks.find((mark) => mark.SquareNumber == squareNumber)
  // }

  // isWhite(squareNumber: number): boolean {
  //   return (
  //     this.marks?.find((mark) => mark?.SquareNumber == squareNumber)?.Color ===
  //     Color.White
  //   )
  // }

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
  // squareNumbers: number[] = [...Array(64).keys()]
  // marks: Mark[] = [
  //   {
  //     SquareNumber: 10,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 15,
  //     Color: Color.White,
  //   },
  //   {
  //     SquareNumber: 11,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 16,
  //     Color: Color.White,
  //   },
  // ]
  }

  cursorLeaveBoard(): void {
    const mark = this.getMarkAttachedToCursor()

    mark.style.visibility = 'hidden'
  // squareNumbers: number[] = [...Array(64).keys()]
  // marks: Mark[] = [
  //   {
  //     SquareNumber: 10,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 15,
  //     Color: Color.White,
  //   },
  //   {
  //     SquareNumber: 11,
  //     Color: Color.Black,
  //   },
  //   {
  //     SquareNumber: 16,
  //     Color: Color.White,
  //   },
  // ]
  }
}
