import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { ChangePasswordRequestDto, AccountActionResponseDto, ApplicationUser } from 'auto/autolmsclient-abstractions';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.less']
})
export class ChangePasswordComponent implements OnInit {
  currentPassword: string = '';
  newPassword: string = '';
  confirmNewPassword: string = '';
  errorMessage: string = '';
  username: string = '';
  user: ApplicationUser | undefined;

  constructor(private authService:AuthService, private router:Router, private store: Store) { }

  ngOnInit(): void {
    this.username = this.store.selectSnapshot(state => state.user.username);
  }

  async changePassword(){
    const changePasswordRequestPayload:ChangePasswordRequestDto= {
      username: this.username, 
      newPassword: this.newPassword,
      currentPassword: this.currentPassword
    }
    let changePasswordResponse: AccountActionResponseDto;
    try {
      changePasswordResponse = await this.authService.changePassword(changePasswordRequestPayload);
      alert("The password has been changed successfully");
      this.router.navigate(['/login']);
     } catch (error) {
      this.errorMessage = "user password change failed"
    }
  }
}
