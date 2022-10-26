import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopbarLinksComponent } from './topbar-links.component';

describe('TopbarLinksComponent', () => {
  let component: TopbarLinksComponent;
  let fixture: ComponentFixture<TopbarLinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TopbarLinksComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TopbarLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
