import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { ApplicationUser } from 'auto/autolmsclient-abstractions';
import { DisplayConstants } from 'src/constants';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.less']
})
export class AccountComponent implements OnInit {
  userID: string | undefined;
  user: ApplicationUser | undefined;
  showPopup: boolean = false;
   displayConstants = DisplayConstants;

  constructor(private store: Store, private authService: AuthService, private router: Router) {

   }

  async ngOnInit(): Promise<void> {
    this.userID = this.store.selectSnapshot(state => state.user.userId);
    this.user = await this.authService.getUserProfile(this.userID ?? '');
  }

  updateProfile(): void {
    this.router.navigate(['/update-profile']);
  }

  changePassword(): void {
    this.router.navigate(['/change-password']);
  }

  deleteProfile(): void {
      this.showPopup = true;
    } 

  confirmDeleteProfile(): void {
    this.authService.deleteUser(this.userID ?? '');
  }

  closePopup() {
    this.showPopup = false;
  }

}
