import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectorRef, OnDestroy, HostListener } from '@angular/core';
import { BehaviorSubject, Subscription, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BookDto, BookRequestDto } from 'auto/autolmsclient-abstractions';
import { RequestService } from 'src/services/request.service';

enum ButtonState {
  Request = 'request',
  Pending = 'pending',
  Approved = 'approved'
}

@Component({
  selector: 'app-book-details-modal',
  templateUrl: './book-details-modal.component.html',
  styleUrls: ['./book-details-modal.component.less']
})
export class BookDetailsModalComponent implements OnInit, OnDestroy {
  @Input() book!: BookDto;
  @Input() displayConstants: any;
  @Output() close = new EventEmitter<void>();

  buttonTitle = 'Request';
  buttonState = new BehaviorSubject<ButtonState>(ButtonState.Request);
  private subscription = new Subscription();
  private destroy$ = new Subject<void>();
  showFullDescription = false;

  constructor(private cdr: ChangeDetectorRef, private requestService: RequestService) { }

  ngOnInit(): void {
    this.subscription = this.buttonState
      .pipe(takeUntil(this.destroy$))
      .subscribe((state: ButtonState) => {
        switch (state) {
          case ButtonState.Request:
            this.buttonTitle = 'Request';
            break;
          case ButtonState.Pending:
            this.buttonTitle = 'Pending...';
            break;
          case ButtonState.Approved:
            this.buttonTitle = 'Approved';
            break;
        }
        this.cdr.detectChanges();
      });

    document.addEventListener('keydown', this.handleEscapeKey.bind(this));
  }

  getTruncatedDescription(description: string): string {
    const words = description.split(' ');
    if (words.length > 100) {
      return words.slice(0, 100).join(' ') + '...';
    }
    return description;
  }

  toggleFullDescription() {
    this.showFullDescription = !this.showFullDescription;
  }

  handleButtonClick() {
    this.buttonState.next(ButtonState.Pending);

    const bookRequest: BookRequestDto = {
      bookId: this.book.id
    };

    this.requestService.addNewRequest(bookRequest).subscribe(
      response => {
        this.buttonState.next(ButtonState.Approved);
      },
      error => {
        console.error('Error sending book request:', error);
        this.buttonState.next(ButtonState.Request);
      }
    );
  }

  closeModal() {
    this.close.emit();
  }

  handleEscapeKey(event: KeyboardEvent) {
    if (event.key === 'Escape') {
      this.closeModal();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.subscription.unsubscribe();
    document.removeEventListener('keydown', this.handleEscapeKey.bind(this));
  }
}