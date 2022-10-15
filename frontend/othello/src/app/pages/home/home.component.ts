import { Component, OnInit } from '@angular/core';
import { createDefaultState} from "../../helper/functions"

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.sass']
})
export class HomeComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {

    console.log("test")
    createDefaultState()
    //testState = 
  }



}
