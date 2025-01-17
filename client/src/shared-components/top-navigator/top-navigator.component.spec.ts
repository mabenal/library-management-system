import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopNavigatorComponent } from './top-navigator.component';

describe('TopNavigatorComponent', () => {
  let component: TopNavigatorComponent;
  let fixture: ComponentFixture<TopNavigatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TopNavigatorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopNavigatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
