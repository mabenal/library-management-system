import { Inject, Injectable } from '@angular/core';
import { IClient, ClientDto } from 'auto/autolmsclient-abstractions';
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

    allClients(): Observable<any> {  
        return this.client.getAllClients();
    }

    deleteClient(id: string): Observable<any> {
        return this.client.deleteClient(id);
    }
    updateClient(id: string, body: ClientDto): Observable<any> {
        return this.client.updateClient(id, body);
    }
    getClient(id: string): Observable<any> {
        return this.client.getClient(id);
    }
}