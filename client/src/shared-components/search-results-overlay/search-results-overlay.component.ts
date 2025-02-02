import { Component, Input, Output, EventEmitter } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { Router } from '@angular/router';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-search-results-overlay',
  templateUrl: './search-results-overlay.component.html',
  styleUrls: ['./search-results-overlay.component.less']
})
export class SearchResultsOverlayComponent {
  @Input() results: BookDto[] = [];
  @Input() searchTerm: string = '';
  @Output() viewAll = new EventEmitter<void>();

  displayConstants = DisplayConstants;

  constructor(private router: Router) {}

  viewAllClick(): void {
    this.viewAll.emit();
  }

  highlight(text: string): string {
    if (!this.searchTerm) {
      return text;
    }
    const regex = new RegExp(`(${this.searchTerm})`, 'gi');
    return text.replace(regex, '<span class="highlight">$1</span>');
  }
}