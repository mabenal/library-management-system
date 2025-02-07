import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BookDto, BookRequestDto } from 'auto/autolmsclient-abstractions';
import { RequestService } from 'src/services/request.service';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';
import { BookRequestStateService, ButtonState } from 'src/services/book-request-state.service';

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  styleUrls: ['./book-details.component.less']
})
export class BookDetailsComponent implements OnInit {
  @Input() book!: BookDto;
  @Input() displayConstants: any = DisplayConstants;
  @Input() books: BookDto[] = [];
  buttonTitle = 'Request';
  buttonState: ButtonState = ButtonState.Request;
  showFullDescription = false;
  similarBooks: BookDto[] = [];
  showCancelButton = false;
  showPopup: boolean = false;
  public ButtonState = ButtonState;

  constructor(
    private route: ActivatedRoute,
    private booksService: BooksService,
    private requestService: RequestService,
    private bookRequestStateService: BookRequestStateService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params: any) => {
      const bookId = params['id'];
      this.loadBookDetails(bookId);
    });
  }

  loadBookDetails(bookId: string) {
    this.booksService.getBook(bookId).subscribe(
      (book: BookDto) => {
        this.book = book;
        this.loadSimilarBooks();
        this.checkRequestState();
      },
      (error: any) => {
        console.error('Error loading book details:', error);
      }
    );
  }

  getTruncatedDescription(description: string): string {
    const words = description.split(' ');
    if (words.length > 75) {
      return words.slice(0, 75).join(' ') + '...';
    }
    return description;
  }

  toggleFullDescription() {
    this.showFullDescription = !this.showFullDescription;
  }

  requestBook() {
    if (!this.book.id) {
      console.error('Book ID is undefined');
      return;
    }
  
    this.buttonState = ButtonState.Pending;
  
    const bookRequest: BookRequestDto = {
      bookId: this.book.id
    };
  
    this.requestService.addNewRequest(bookRequest).subscribe(
      (response: any) => {
        this.buttonState = ButtonState.Approved;
      },
      (error: any) => {
        console.error('Error sending book request:', error);
        this.buttonState = ButtonState.Request;
      }
    );
  }

  handleCancelButtonClick() {
    this.showPopup = true;
  }

  closePopup() {
    this.showPopup = false;
  }

  cancelRequest() {
    this.showPopup = true;
    this.buttonState = ButtonState.Request;
    this.buttonTitle = 'Request';
    this.showCancelButton = false;
  }

  private loadSimilarBooks() {
    this.similarBooks = this.books
      .filter(b => b.category === this.book.category && b.id !== this.book.id)
      .slice(0, 10);
  }

  private checkRequestState() {
    if (!this.book || !this.book.id) {
      return;
    }

    this.bookRequestStateService.checkRequestState(this.book.id).subscribe(
      ({ buttonState, buttonTitle, showCancelButton }) => {
        this.buttonState = buttonState;
        this.buttonTitle = buttonTitle;
        this.showCancelButton = showCancelButton;
      },
      (error: any) => {
        console.error('Error fetching book request state:', error);
      }
    );
  }
}