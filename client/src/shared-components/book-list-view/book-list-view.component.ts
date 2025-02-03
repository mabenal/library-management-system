import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BookDto } from 'auto/autolmsclient-abstractions';

@Component({
  selector: 'app-book-list-view',
  templateUrl: './book-list-view.component.html',
  styleUrls: ['./book-list-view.component.less']
})
export class BookListViewComponent implements OnInit {
  @Input() books: BookDto[] = [];
  @Input() displayConstants: any;
  @Input() enablePagination: boolean = true;
  @Output() showBookDetails = new EventEmitter<BookDto>();

  currentPage: number = 1;
  itemsPerPage: number = 15;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  onShowBookDetails(book: BookDto) {
    this.router.navigate(['/book', book.id]);
  }

  get paginatedBooks(): BookDto[] {
    if (!this.enablePagination) {
      return this.books;
    }
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.books.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get totalPages(): number {
    return Math.ceil(this.books.length / this.itemsPerPage);
  }
}