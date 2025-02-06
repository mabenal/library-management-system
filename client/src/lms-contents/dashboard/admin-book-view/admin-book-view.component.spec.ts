import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminBookViewComponent } from './admin-book-view.component';

describe('AdminBookViewComponent', () => {
  let component: AdminBookViewComponent;
  let fixture: ComponentFixture<AdminBookViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminBookViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminBookViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
