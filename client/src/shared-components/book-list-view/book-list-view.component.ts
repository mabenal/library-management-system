import { Component, OnInit, EventEmitter, Input, Output, ChangeDetectorRef, OnDestroy} from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BookDetailsModalComponent } from '../book-details-modal/book-details-modal.component';
import { PaginationComponent } from './pagination/pagination.component';

@Component({
  selector: 'app-book-list-view',
  templateUrl: './book-list-view.component.html',
  styleUrls: ['./book-list-view.component.less']
})
export class BookListViewComponent implements OnInit {
  @Input() books: BookDto[] = []; // List of books
  @Input() displayConstants: any;
  @Output() showBookDetails = new EventEmitter<BookDto>();
  
  currentPage: number = 1;
  itemsPerPage: number = 15;

  constructor() { }

  ngOnInit(): void {
    console.log('Books:', this.books);
  }
  
  onShowBookDetails(book: BookDto) {
    this.showBookDetails.emit(book);
  }

  get paginatedBooks(): BookDto[] {
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