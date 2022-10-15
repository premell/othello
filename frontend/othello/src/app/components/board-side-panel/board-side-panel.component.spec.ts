import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardSidePanelComponent } from './board-side-panel.component';

describe('BoardSidePanelComponent', () => {
  let component: BoardSidePanelComponent;
  let fixture: ComponentFixture<BoardSidePanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BoardSidePanelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BoardSidePanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
