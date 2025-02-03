import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AccountActionResponseDto, ApplicationUser, ChangePasswordRequestDto, LoginDto, LoginResponseDto, RegisterDto, RegisterResponseDto, UpdateUserRequestDto } from 'auto/autolmsclient-abstractions';
import { IClient } from 'auto/autolmsclient-abstractions';
import { CookieService } from 'ngx-cookie-service';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(false);
  private userRole: string | null = null;
  private tokenKey = 'authToken';


  constructor(@Inject('IClient') private client: IClient, private router: Router, private cookieService: CookieService) { }

  login(username: string, password: string): Promise<LoginResponseDto> {
    const loginDto: LoginDto = { userName: username, password: password };
    return this.client.login(loginDto).toPromise()
      .then((response: LoginResponseDto | undefined) => {
        if (response) {
          this.loggedIn.next(true);
          return response;
        } else {
          throw new Error('Login failed');
        }
      });
  }

  register(registerDTO: RegisterDto): Promise<RegisterResponseDto> {
    return this.client.register(registerDTO).toPromise().then((response: RegisterResponseDto | undefined) => {
      if (response?.succeeded) {
        return response;
      } else {
        throw new Error('Registration failed');
      }
    });

  }

  changePassword(changePasswordRequestDto: ChangePasswordRequestDto): Promise<AccountActionResponseDto> {
    return this.client.changePassword(changePasswordRequestDto).toPromise().then((response: AccountActionResponseDto | undefined) => {
      if (response?.isSuccessful) {
        return response;
      }
      {
        throw new Error('password change failed');
      }
    })
  }

  updateUserProfile(id: string, updateUserProfile: UpdateUserRequestDto): Promise<AccountActionResponseDto> {
    return this.client.updateProfile(id, updateUserProfile).toPromise().then((response: AccountActionResponseDto | undefined) => {
      if (response?.isSuccessful) {
        return response;
      }
      {
        throw new Error('user profile update failed');
      }
    })
  }


  getUserProfile(id: string): Promise<ApplicationUser> {
    return this.client.getProfile(id).toPromise().then((response: ApplicationUser | undefined) => {
      if (response !== undefined) {
        return response;
      }
      {
        throw new Error('get user profile failed');
      }
    })
  }

  deleteUser(id: string): void {
    this.client.deleteUser(id).subscribe((response: AccountActionResponseDto | undefined) => {
      if (response?.isSuccessful) {
        this.loggedIn.next(false);
        this.userRole = null;
        this.router.navigate(['/login']);
      }
      else {
        throw new Error('delete user failed');
      }
    })
  }

  isLoggedIn(): boolean {
    return this.cookieService.check(this.tokenKey);
  }


  getUserRole(): string | null {
    return this.userRole;
  }

  setToken(token: string): void {
    this.cookieService.set(this.tokenKey, token, {
      secure: true,
      sameSite: 'Strict',
    });
  }
  getToken(): string {
    return this.cookieService.get(this.tokenKey);
  }

  clearToken(): void {
    this.cookieService.delete(this.tokenKey);
  }

  logout() {
    this.loggedIn.next(false);
    this.userRole = null;
    this.router.navigate(['/login']);
  }
}