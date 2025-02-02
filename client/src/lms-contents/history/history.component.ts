import { Component, OnInit } from '@angular/core';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { RequestService } from 'src/services/request.service';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.less']
})
export class HistoryComponent implements OnInit {

  requests: BookRequestDto[] = [];
  showPopup: boolean = false;
  selectedRequest: any;

  constructor(private requestService: RequestService) { }

  ngOnInit(): void {
    this.getRequests();
  }

  async getRequests() {
    try {
      const response = await this.requestService.getbookRequestByClient().toPromise();
      if (response) {
        this.requests = response;
        console.log('Book requests:', response);
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
      this.selectedRequest.status = 'Cancelled';
    }
    this.showPopup = false;
  }

}
