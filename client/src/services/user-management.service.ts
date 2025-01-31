import { Inject, Injectable } from '@angular/core';
import { IClient } from 'auto/autolmsclient-abstractions';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {

  constructor(@Inject('IClient') private client: IClient) { }

}
