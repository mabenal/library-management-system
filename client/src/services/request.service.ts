import { Inject, Injectable } from '@angular/core';
import { BookRequestDto, IClient } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable()
export class RequestService {
    constructor(@Inject('IClient') private client: IClient) {}

    addNewRequest(bookRequest: BookRequestDto): Observable<BookRequestDto> {
        return this.client.addNewRequest(bookRequest);
    }

    getBookRequestsByClient(): Observable<any> { 
        return this.client.getBookRequestsByClient();
    }

    cancelRequest(bookId: string): Observable<any> {
        return this.client.cancelRequest(bookId);
    }

    cancelBookRequestByClient(clientId: string, bookId: string): Observable<any> {
        return this.client.cancelRequestByClient(clientId, bookId);
    }

    returnRequest(query: string): Observable<any> {
        return this.client.returnRequest(query);
    }

    overdueRequest(query: string): Observable<any> {
        return this.client.overdueRequest(query);
    }

    getAllBookRequests(): Observable<any> {
        return this.client.getAllBookRequests();
    }

    approveRequest(clientId: string, bookId: string): Observable<any> {
        return this.client.approveRequest(clientId, bookId);
    }
}