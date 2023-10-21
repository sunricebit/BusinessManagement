import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewStatisticComponent } from './view-statistic.component';

describe('ViewStatisticComponent', () => {
  let component: ViewStatisticComponent;
  let fixture: ComponentFixture<ViewStatisticComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewStatisticComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewStatisticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
