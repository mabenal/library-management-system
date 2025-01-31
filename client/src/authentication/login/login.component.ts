import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  selectedTab: string = 'client';
  errorMessage: string = '';

  constructor(@Inject(AuthService) private authService: AuthService, private router: Router) {}

  selectTab(tab: string) {
    this.selectedTab = tab;
  }

  async onSubmit() {
    try {
      const user = await this.authService.login(this.username, this.password);
      this.router.navigate(['/books']);
    } catch (error) {
      this.errorMessage = 'Invalid username or password.';
    }
  }
}