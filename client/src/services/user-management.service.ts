import { Inject, Injectable } from '@angular/core';
import { IClient } from 'auto/autolmsclient-abstractions';
import { RoleDto } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {

  constructor(@Inject('IClient') private client: IClient) { }

    assignRole(userId: string, role: string): Observable<any> {
        return this.client.assignRole(userId, role);
    }
}
