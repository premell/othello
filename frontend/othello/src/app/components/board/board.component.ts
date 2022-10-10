import { Component, OnInit } from '@angular/core'

enum Color {
  Black,
  White,
}
interface Mark {
  SquareNumber: number
  Color: Color
}

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.sass'],
})
export class BoardComponent implements OnInit {
  squareNumbers: number[] = [...Array(64).keys()]
  marks: Mark[] = [
    {
      SquareNumber: 10,
      Color: Color.Black,
    },
    {
      SquareNumber: 15,
      Color: Color.White,
    },
  ]

  constructor() {}

  ngOnInit(): void {}

  squareContainsMark(squareNumber: number): boolean {
    return !!this.marks.find(mark => mark.SquareNumber == squareNumber);
  }

  isWhite(squareNumber: number): boolean {
    return this.marks?.find(mark => mark?.SquareNumber == squareNumber)?.Color === Color.White;
  }
}
