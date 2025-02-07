import { Component, OnInit } from '@angular/core';
import {ClientDto } from 'auto/autolmsclient-abstractions';
import { Router } from '@angular/router';
import { UserManagementService } from 'src/services/user-management.service';
import { ClientService } from 'src/services/client.service';

type SortableFields = 'id' | 'name' | 'lastName' | 'address' | 'phoneNumber';

@Component({
  selector: 'app-user-grouping',
  templateUrl: './user-grouping.component.html',
  styleUrls: ['./user-grouping.component.less']
})
export class UserGroupingComponent implements OnInit {

  clients: ClientDto[] = []; 
  filteredClients: ClientDto[] = [];
  selectedClient: ClientDto | null = null;
  sortDirection: { [key in SortableFields]?: boolean } = {};
  currentPage: number = 1;
  itemsPerPage: number = 15;

  constructor(private clientService: ClientService, private router: Router, private userManagementService: UserManagementService) { }

  ngOnInit(): void {
    this.addClient();
  }

  async addClient() {
    try {
      this.clients = await this.clientService.getAllClients().toPromise();
      this.filteredClients = this.clients;
    } catch (error) {
      console.error('Error fetching clients', error);
    }
  }

  selectClient(client: ClientDto) {
    this.selectedClient = client;
  }

  deleteClient(id: string) {
    try {
      this.clientService.deleteClient(id).subscribe(() => {
        this.clients = this.clients.filter(client => client.id !== id);
        this.filteredClients = this.clients;
      });
    } catch (error) {
      console.error('Error deleting client', error);
    }
  }

  editClient(id: string): void {
    this.router.navigate(['/update-client', id]);
  }

  assignRole(clientId: string, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const role = selectElement.value;
    this.userManagementService.assignRole(clientId, role);
  }

  sortClients(field: SortableFields): void {
    this.sortDirection[field] = !this.sortDirection[field];
    const direction = this.sortDirection[field] ? 1 : -1;
    this.filteredClients.sort((a, b) => {
      if (field === 'id' || field === 'phoneNumber') {
        return (a[field] > b[field] ? 1 : -1) * direction;
      } else {
        return (a[field].localeCompare(b[field])) * direction;
      }
    });
  }

  filterClients(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value.toLowerCase();
    this.filteredClients = this.clients.filter(client =>
      client.name.toLowerCase().includes(searchTerm) ||
      client.lastName.toLowerCase().includes(searchTerm) ||
      client.phoneNumber.toLowerCase().includes(searchTerm) ||
      client.address.toLowerCase().includes(searchTerm)
    );
  }

  get paginatedClients(): ClientDto[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredClients.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get totalPages(): number {
    return Math.ceil(this.filteredClients.length / this.itemsPerPage);
  }
}