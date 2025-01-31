import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto, RegisterResponseDto } from 'auto/autolmsclient-abstractions';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls: ['./create-account.component.less']
})
export class CreateAccountComponent implements OnInit {
  name:string ="";
  lastName:string="";
  emailAddress:string="";
  address:string="";
  phoneNumber:string="";
  password:string="";
  confirmPassword="";
  errorMessage:string="";

  constructor(private authService: AuthService, private router:Router) { }

  ngOnInit(): void {}

  async createAccount(){
    const registerDto : RegisterDto ={
      name:this.name,
      lastName:this.lastName,
      email:this.emailAddress,
      address:this.address,
      phoneNumber:this.phoneNumber,
      password:this.password,
      confirmPassword:this.confirmPassword
    }

    let registerResponse: RegisterResponseDto;
    try {
      registerResponse = await this.authService.register(registerDto);
      this.router.navigate(['/login']);
    } catch (error) {
      this.errorMessage = "user registration failed"
    }
  }
}
