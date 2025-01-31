import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChangePasswordRequestDto, AccountActionResponseDto } from 'auto/autolmsclient-abstractions';
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

  constructor(private authService:AuthService, private router:Router) { }

  ngOnInit(): void {}

  async onSubmit(){
    const changePasswordRequestPayload :ChangePasswordRequestDto= {
      username: 'username', //TODO: get username from local storage
      newPassword: this.newPassword,
      currentPassword: this.currentPassword
    }
    let changePasswordResponse: AccountActionResponseDto;
    try {
      changePasswordResponse = await this.authService.changePassword(changePasswordRequestPayload);

      this.router.navigate(['/login']);
     } catch (error) {
      this.errorMessage = "user password change failed"
    }
  }
}
