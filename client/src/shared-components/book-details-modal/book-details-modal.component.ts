import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { BookDto } from 'auto/autolmsclient-abstractions';

enum ButtonState {
  Complete = 'request',
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
  buttonState = new BehaviorSubject<ButtonState>(ButtonState.Complete);
  private subscription!: Subscription;
  showFullDescription = false;

  constructor(private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.subscription = this.buttonState.subscribe((state: ButtonState) => {
      switch (state) {
        case ButtonState.Complete:
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
  }

  getTruncatedDescription(description: string): string {
    const words = description.split(' ');
    if (words.length > 100) {
      return words.slice(0, 100).join(' ') + '...';
    }
    return description;
  }

  toggleDescription() {
    this.showFullDescription = !this.showFullDescription;
  }

  handleButtonClick() {
    this.buttonState.next(ButtonState.Pending);
  }

  closeModal() {
    this.close.emit();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}