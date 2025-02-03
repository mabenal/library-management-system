import { Component, Inject, OnInit } from '@angular/core';
import { RequestService } from 'src/services/request.service';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.less']
})
export class HistoryComponent implements OnInit {
  requests: BookRequestDto[] = [];
  showPopup: boolean = false;
  selectedRequest: BookRequestDto | null = null;
  displayConstants = DisplayConstants;

  constructor(@Inject('IClient') private requestService: RequestService) { }

  ngOnInit(): void {
    this.getRequests();
  }

  async getRequests() {
    try {
      const response = await this.requestService.getbookRequestByClient().toPromise();
      if (response) {
        this.requests = response;
      }
    } catch (error) {
      console.error('Error getting book requests:', error);
    }
  }

  cancelRequest(request: BookRequestDto): void {
    this.selectedRequest = request;
    this.showPopup = true;
  }

  closePopup() {
    this.showPopup = false;
    this.selectedRequest = null;
  }

  confirmCancel(): void {
    if (this.selectedRequest) {
      this.requestService.cancelRequest(this.selectedRequest.bookId).subscribe(
        response => {
          this.selectedRequest!.status = 'Cancelled';
          this.showPopup = false;
          this.selectedRequest = null;
        },
        error => {
          console.error('Error cancelling request:', error);
        }
      );
    }
  }
}