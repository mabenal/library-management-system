import { Component, Input } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-results-overlay',
  templateUrl: './search-results-overlay.component.html',
  styleUrls: ['./search-results-overlay.component.less']
})
export class SearchResultsOverlayComponent {
  @Input() results: BookDto[] = [];
  @Input() searchTerm: string = '';

  constructor(private router: Router) {}

  viewAll(): void {
    this.router.navigate(['/search-results']);
  }

  highlight(text: string): string {
    if (!this.searchTerm) {
      return text;
    }
    const regex = new RegExp(`(${this.searchTerm})`, 'gi');
    return text.replace(regex, '<span class="highlight">$1</span>');
  }
}