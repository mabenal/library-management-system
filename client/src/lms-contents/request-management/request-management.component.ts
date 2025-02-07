import { Component, OnInit } from '@angular/core';
import { RequestService } from 'src/services/request.service';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { DisplayConstants } from 'src/constants/constants';
import { Router } from '@angular/router';

type SortableFields = 'clientId' | 'bookId' | 'title' | 'status' | 'dateRequested' | 'acceptedReturnDate';

@Component({
  selector: 'app-request-management',
  templateUrl: './request-management.component.html',
  styleUrls: ['./request-management.component.less']
})
export class RequestManagementComponent implements OnInit {

  requests: BookRequestDto[] = [];
  filteredRequests: BookRequestDto[] = [];
  selectedRequest: BookRequestDto | null = null;
  sortDirection: { [key in SortableFields]?: boolean } = {};
  currentPage: number = 1;
  itemsPerPage: number = 15;

  constructor(private requestService: RequestService, private router: Router) { }

  ngOnInit(): void {
    this.fetchRequests();
  }

  async fetchRequests() {
    try {
      this.requests = await this.requestService.getAllBookRequests().toPromise();
      this.filteredRequests = this.requests;
    } catch (error) {
      console.error('Error fetching requests', error);
    }
  }

  selectRequest(request: BookRequestDto) {
    this.selectedRequest = request;
  }

  deleteRequest(clientId: string | undefined, bookId: string | undefined) {
    if (clientId && bookId) {
      this.requestService.cancelBookRequestByClient(clientId, bookId).subscribe(() => {
        this.requests = this.requests.filter(request => request.bookId !== bookId && request.clientId !== clientId);
        this.filteredRequests = this.requests;
      });
    } else {
      console.error('Client ID or Book ID is undefined');
    }
  }
  
  approveRequest(clientId: string | undefined, bookId: string | undefined) {
    if (clientId && bookId) {
      this.requestService.approveRequest(clientId, bookId).subscribe(
        response => {
          this.fetchRequests();
        },
        error => {
          console.error('Error approving request', error);
        }
      );
    } else {
      console.error('Client ID or Book ID is undefined');
    }
  }

  sortRequests(field: SortableFields): void {
    this.sortDirection[field] = !this.sortDirection[field];
    const direction = this.sortDirection[field] ? 1 : -1;
    this.filteredRequests.sort((a, b) => {
      const aField = a[field];
      const bField = b[field];
  
      if (aField === undefined || bField === undefined) {
        return 0;
      }
  
      if (typeof aField === 'string' && typeof bField === 'string') {
        return aField.localeCompare(bField) * direction;
      } else if (aField instanceof Date && bField instanceof Date) {
        return (aField.getTime() - bField.getTime()) * direction;
      } else {
        return (aField > bField ? 1 : -1) * direction;
      }
    });
  }

  filterRequests(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value.toLowerCase();
    this.filteredRequests = this.requests.filter(request =>
      request.title?.toLowerCase().includes(searchTerm) ||
      request.status?.toLowerCase().includes(searchTerm)
    );
  }

  get paginatedRequests(): BookRequestDto[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredRequests.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get totalPages(): number {
    return Math.ceil(this.filteredRequests.length / this.itemsPerPage);
  }
}