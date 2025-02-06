import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';

@Component({
  selector: 'app-update-client',
  templateUrl: './update-client.component.html',
  styleUrls: ['./update-client.component.less']
})
export class UpdateClientComponent implements OnInit {
  client: ClientDto = {
    id: '',
    name: '',
    lastName: '',
    emailAddress: '',
    password: '',
    address: '',
    phoneNumber: ''
  };
  errorMessage: string = '';

  constructor(
    private clientService: BooksService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const clientId = this.route.snapshot.paramMap.get('id');
    if (clientId) {
      this.clientService.getClient(clientId).subscribe(
        client => {
          this.client = client;
        },
        error => {
          console.error('Error fetching client:', error);
          this.errorMessage = 'Failed to load client data';
        }
      );
    }
  }

  updateClient(): void {
    this.clientService.updateClient(this.client.id, this.client).subscribe(
      updatedClient => {
        alert('Client updated successfully');
        this.router.navigate(['/dashboard']); // Navigate to the clients list or another appropriate page
      },
      error => {
        console.error('Error updating client:', error);
        this.errorMessage = 'Failed to update client';
      }
    );
  }
}