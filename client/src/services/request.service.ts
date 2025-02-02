import { Inject, Injectable } from '@angular/core';
import { BookRequestDto, IClient } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable()
export class RequestService {
    constructor(@Inject('IClient') private client: IClient) {}

    addNewRequest(bookRequest: BookRequestDto): Observable<BookRequestDto> {
        return this.client.addNewRequest(bookRequest);
    }

    getbookRequestByClient(): Observable<any> {
        return this.client.getBookRequestsByClient();
    }

    cancelRequest(query: string): Observable<any> {
        return this.client.cancelRequest(query);
    }

    returnRequest(query: string): Observable<any> {
        return this.client.returnRequest(query);
    }

    overdueRequest(query: string): Observable<any> {
        return this.client.overdueRequest(query);
    }

    getAllbookRequests(): Observable<any> {
        return this.client.getAllBookRequests();
    }
}