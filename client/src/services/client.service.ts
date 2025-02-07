import { Inject, Injectable } from '@angular/core';
import { IClient, ClientDto, BookDto } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable()
export class ClientService {
    constructor(@Inject('IClient') private client: IClient) {}

    getAllClients(): Observable<any> {  
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