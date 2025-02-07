import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {
  private roleKey = 'userRoles';
  private clientIdKey = 'clientId';

  constructor(private cookieService: CookieService) {}

  setUserRoles(clientId: string, roles: string[]): void {
    this.cookieService.set(this.clientIdKey, clientId, {
      secure: true,
      sameSite: 'Strict',
    });
    this.cookieService.set(this.roleKey, JSON.stringify(roles), {
      secure: true,
      sameSite: 'Strict',
    });
  }

  getUserRoles(): string[] {
    const roles = this.cookieService.get(this.roleKey);
    return roles ? JSON.parse(roles) : [];
  }

  getClientId(): string | null {
    return this.cookieService.get(this.clientIdKey) || null;
  }

  clearUserRoles(): void {
    this.cookieService.delete(this.roleKey);
    this.cookieService.delete(this.clientIdKey);
  }
}