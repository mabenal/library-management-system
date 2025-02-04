import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BookDto, BookRequestDto } from 'auto/autolmsclient-abstractions';
import { RequestService } from 'src/services/request.service';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';

enum ButtonState {
  Request = 'Request',
  Pending = 'Pending',
  Approved = 'Approved'
}

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

  constructor(
    private route: ActivatedRoute,
    private booksService: BooksService,
    private requestService: RequestService
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
        this.checkPendingRequest();
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

  handleButtonClick() {
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
    // Logic to handle cancel request
  }

  private loadSimilarBooks() {
    this.similarBooks = this.books
      .filter(b => b.category === this.book.category && b.id !== this.book.id)
      .slice(0, 10);
  }

  private checkPendingRequest() {
    if (!this.book) {
      return;
    }

    this.requestService.getBookRequestsByClient().subscribe(
      (requests: BookRequestDto[]) => {
        const pendingRequest = requests.find(request => request.bookId === this.book.id && request.status === 'Pending');
        if (pendingRequest) {
          this.buttonState = ButtonState.Pending;
          this.buttonTitle = 'Pending...';
          this.showCancelButton = true;
        } else {
          this.showCancelButton = false;
        }
      },
      (error: any) => {
        console.error('Error fetching book requests:', error);
      }
    );
  }
}