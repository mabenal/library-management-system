import { Component, OnInit } from '@angular/core';
import { BooksService } from 'src/services/books.services';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { Observable, Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-book-search',
  templateUrl: './book-search.component.html',
  styleUrls: ['./book-search.component.less']
})
export class BookSearchComponent implements OnInit {
  searchResults$: Observable<BookDto[]> = new Observable<BookDto[]>();
  private searchTerms = new Subject<string>();
  searchTerm: string = '';
  showOverlay: boolean = true;

  constructor(private booksService: BooksService, private router: Router) { }

  ngOnInit(): void {
    this.searchResults$ = this.searchTerms.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((term: string) => term.trim() ? this.booksService.searchBooks(term) : of([])),
    );
  }

  search(term: string): void {
    const sanitizedTerm = term.replace(/[^a-zA-Z0-9 ]/g, '');
    this.searchTerm = sanitizedTerm;
    this.searchTerms.next(sanitizedTerm);
  }

  onSubmit(event: Event, term: string): void {
    event.preventDefault();
    this.searchTerm = term;
    this.search(this.searchTerm);
    this.showOverlay = false;
    this.router.navigate(['/search-results'], { queryParams: { q: this.searchTerm } });
  }

  onSearchButtonClick(term: string): void {
    this.searchTerm = term;
    this.search(this.searchTerm);
    this.showOverlay = false;
    this.router.navigate(['/search-results'], { queryParams: { q: this.searchTerm } });
  }

  onViewAllClick(): void {
    this.showOverlay = false;
    this.router.navigate(['/search-results'], { queryParams: { q: this.searchTerm } });
  }
}