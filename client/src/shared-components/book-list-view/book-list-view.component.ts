import { Component, OnInit, EventEmitter, Input, Output, ChangeDetectorRef, OnDestroy} from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BookDetailsModalComponent } from '../book-details-modal/book-details-modal.component';

@Component({
  selector: 'app-book-list-view',
  templateUrl: './book-list-view.component.html',
  styleUrls: ['./book-list-view.component.less']
})
export class BookListViewComponent implements OnInit {
  @Input() books: BookDto[] = []; // List of books
  @Input() displayConstants: any;
  @Output() showBookDetails = new EventEmitter<BookDto>();

  constructor() { }

  ngOnInit(): void {
  }
  
  onShowBookDetails(book: BookDto) {
    this.showBookDetails.emit(book);
  }

}
