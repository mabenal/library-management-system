import { Component, OnInit } from '@angular/core';
import { BooksService } from 'src/services/books.services';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { Observable, Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-book-search',
  templateUrl: './book-search.component.html',
  styleUrls: ['./book-search.component.less']
})
export class BookSearchComponent implements OnInit {
  searchResults$: Observable<BookDto[]> = new Observable<BookDto[]>();
  private searchTerms = new Subject<string>();
  searchTerm: string = '';

  constructor(private booksService: BooksService) { }

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
}