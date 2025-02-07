import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RequestService } from 'src/services/request.service';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-cancellation-prompt',
  templateUrl: './cancellation-prompt.component.html',
  styleUrls: ['./cancellation-prompt.component.less']
})
export class CancellationPromptComponent {
  @Input() request!: BookRequestDto;
  @Output() close = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
  displayConstants = DisplayConstants;
  deleteMessage: string | null = null;

  constructor(private requestService: RequestService) {}

  confirmCancel(): void {
    if (this.request) {
      this.requestService.cancelRequest(this.request.bookId).subscribe(
        response => {
          this.request.status = 'Cancelled';
          this.deleteMessage = 'Request cancelled successfully.';
          this.cancelled.emit();
          setTimeout(() => this.closePopup(), 3000);
        },
        error => {
          console.error('Error cancelling request:', error);
          this.deleteMessage = 'Failed to cancel request.';
          setTimeout(() => this.closePopup(), 3000);
        }
      );
    }
  }

  closePopup() {
    this.close.emit();
  }
}