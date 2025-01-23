import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchResultsOverlayComponent } from './search-results-overlay.component';

describe('SearchResultsOverlayComponent', () => {
  let component: SearchResultsOverlayComponent;
  let fixture: ComponentFixture<SearchResultsOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SearchResultsOverlayComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchResultsOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
