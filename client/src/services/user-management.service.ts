import { Inject, Injectable } from '@angular/core';
import { IClient } from 'auto/autolmsclient-abstractions';
import { RoleDto } from 'auto/autolmsclient-abstractions';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {

  constructor(@Inject('IClient') private client: IClient) { }

  assignRole(userId: string, role: string): void {
    const roleDto: RoleDto = { userId, role };
    this.client.assignRole(roleDto).subscribe(response => {
    }, error => {
      console.error('Error assigning role', error);
    });
  }
}
