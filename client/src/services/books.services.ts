import { Inject, Injectable } from '@angular/core';
import { IClient, ClientDto, BookDto } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable()
export class BooksService {constructor(@Inject('IClient') private client: IClient) {}

    books(): Observable<any> {
        return this.client.getAllBooks();
    }

    searchBooks(query: string): Observable<any> {
        return this.client.searchBooks(query);
    }

    addBook(body: BookDto): Observable<any> {
        console.log('Adding book:', body);
        return this.client.addBook(body);
    }

    getBook(id: string): Observable<any> {
        return this.client.getBook(id);
    }

    removeBook(id: string): Observable<any> {
        return this.client.removeBook(id);
    }

    updateBook(id: string, body: BookDto): Observable<any> {
        return this.client.updateBook(id, body);
    }
}