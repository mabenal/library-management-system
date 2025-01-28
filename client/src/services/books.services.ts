import { Inject, Injectable } from '@angular/core';
import { IClient } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable()
export class BooksService {
    constructor(@Inject('IClient') private client: IClient) {}

    books(): Observable<any> {
        return this.client.getAllBooks();
    }

    searchBooks(query: string): Observable<any> {
        return this.client.searchBooks(query);
    }
}