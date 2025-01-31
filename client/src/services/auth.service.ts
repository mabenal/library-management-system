import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AccountActionResponseDto, ChangePasswordRequestDto, LoginDto, LoginResponseDto, RegisterDto, RegisterResponseDto } from 'auto/autolmsclient-abstractions';
import { IClient } from 'auto/autolmsclient-abstractions';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(false);
  private userRole: string | null = null;

  constructor(@Inject('IClient') private client: IClient, private router: Router) {}

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

  register(registerDTO: RegisterDto):Promise<RegisterResponseDto>{
    return this.client.register(registerDTO).toPromise().then((response: RegisterResponseDto | undefined) => {
      if (response?.succeeded) {
        return response;
      } else {
        throw new Error('Registration failed');
      }
    });

  }

  changePassword(changePasswordRequestDto:ChangePasswordRequestDto):Promise<AccountActionResponseDto>{
    return this.client.changePassword(changePasswordRequestDto).toPromise().then((response:AccountActionResponseDto|undefined)=>{
      if(response?.isSuccessful){
        return response;
      }
      {
          throw new Error('password change failed');
      }
    })}

  isLoggedIn(): boolean {
    return this.loggedIn.value;
  }

  getUserRole(): string | null {
    return this.userRole;
  }

  logout() {
    this.loggedIn.next(false);
    this.userRole = null;
    this.router.navigate(['/login']);
  }
}