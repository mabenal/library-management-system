import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { UpdateUserRequestDto } from 'auto/autolmsclient-abstractions';
import { Console } from 'console';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-update-profile',
  templateUrl: './update-profile.component.html',
  styleUrls: ['./update-profile.component.less']
})
export class UpdateProfileComponent implements OnInit {
  name:string ="";
  lastName:string="";
  emailAddress:string="";
  address:string="";
  phoneNumber:string="";
  errorMessage:string="";
  id: string = '';

  constructor(private authService: AuthService, private router:Router, private store:Store) { }

  async ngOnInit() {
    try {
      const userState = this.store.selectSnapshot(state => state.user);

      if (!userState || !userState.userId) {
        throw new Error("User ID is undefined or null in the store.");
      }
  
      this.id = userState.userId;
  
      let user = await this.authService.getUserProfile(this.id);
    
  
      if (user) {
        this.name = user.name ?? '';
        this.lastName = user.lastName ?? '';
        this.emailAddress = user.email ?? '';
        this.address = user.address ?? '';
        this.phoneNumber = user.phoneNumber ?? '';
      }
    } catch (error) {
      console.error("Error fetching user profile:", error);
      this.errorMessage = "Failed to load profile data.";
    }
  }
  
  updateProfile()
  { 
    const updateUserProfile: UpdateUserRequestDto = {
      name: this.name,
      lastName: this.lastName,
      phoneNumber: this.phoneNumber,
      address: this.address,
    
    }
    this.authService.updateUserProfile(this.id, updateUserProfile).then((response) => {
      alert("The user profile has been updated successfully");
      this.router.navigate(['/login']);
    }).catch((error) => {
      this.errorMessage = "user profile update failed";
    });
  }

}
