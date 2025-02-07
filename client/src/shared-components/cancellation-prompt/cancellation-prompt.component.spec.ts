import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelationPromptComponent } from './cancelation-prompt.component';

describe('CancelationPromptComponent', () => {
  let component: CancelationPromptComponent;
  let fixture: ComponentFixture<CancelationPromptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CancelationPromptComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CancelationPromptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
