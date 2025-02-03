import { Component, OnInit } from '@angular/core';
import { IClient, ClientDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services'; 
import { DisplayConstants } from 'src/constants/constants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-grouping',
  templateUrl: './user-grouping.component.html',
  styleUrls: ['./user-grouping.component.less']
})
export class UserGroupingComponent implements OnInit {

  clients: ClientDto[] = []; 
  selectedClient: ClientDto | null = null;

  constructor(private bookService: BooksService, private router: Router) { }

  ngOnInit(): void {
    this.addClient();
  }

   async addClient() {
    try{
    this.clients = await this.bookService.allClients().toPromise();

    }
    catch (error) {
      console.error('Error fetching clients', error);
  }
}
selectClient(client: ClientDto) {
  this.selectedClient = client;
}

deleteClient(id: string) {
  try {
  this.bookService.deleteClient(id).subscribe(() => {
    this.clients = this.clients.filter(client => client.id !== id);
  });
  } catch (error) {
    console.error('Error deleting client', error);
  }
}
editClient(id: string): void {
  this.router.navigate(['/update-client', id]);
}
}
