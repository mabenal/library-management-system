import { Component, OnInit } from '@angular/core';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.less']
})
export class HistoryComponent implements OnInit {

  requests: BookRequestDto[] = [];
  showPopup: boolean = false;
  selectedRequest: any;

  constructor(private bookService: BooksService) { }

  ngOnInit(): void {
    this.getRequests();
  }

  async getRequests() {
    try {
      this.requests = await this.bookService.getAllbookRequests().toPromise();
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
