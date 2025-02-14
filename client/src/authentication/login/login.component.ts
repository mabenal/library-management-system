import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Store } from '@ngxs/store';
import { SetUser } from '../store/actions/user.actions';
import { ApplicationUser } from 'auto/autolmsclient-abstractions';
import { UserRoleService } from 'src/services/user-role.service';

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

  constructor(@Inject(AuthService) private authService: AuthService, @Inject(UserRoleService) private roleService: UserRoleService,
   private router: Router, private store:Store) {}

  selectTab(tab: string) {
    this.selectedTab = tab;
  }

  async onSubmit() {
    try {
      const user = await this.authService.login(this.username, this.password);
      this.store.dispatch(new SetUser(user.username ?? '', user.userRoles ?? [], user.userID ?? ''));
      this.authService.setToken(user.token ?? '');
      
      this.roleService.setUserRoles(user.userID ?? '', user.userRoles ?? []);

      if (user.userRoles?.includes("admin") || user.userRoles?.includes("librarian")) {
        this.router.navigate(['/dashboard']);
        return;
      } else {
        this.router.navigate(['/books']);
      }
    } catch (error) {
      this.errorMessage = 'Invalid username or password.';
    }
  }
}