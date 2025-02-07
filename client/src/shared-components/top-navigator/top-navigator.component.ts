import { Component, OnInit, Inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserRoleService } from 'src/services/user-role.service';
import { DisplayConstants } from 'src/constants/constants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-top-navigator',
  templateUrl: './top-navigator.component.html',
  styleUrls: ['./top-navigator.component.less']
})
export class TopNavigatorComponent implements OnInit {
  displayConstants = DisplayConstants;
  menuOpen = false;
  searchOpen = false;
  userRoles: string[] = [];

  constructor(private authService: AuthService,
    private router: Router,
    @Inject(UserRoleService) private userRoleService: UserRoleService ) { }

  ngOnInit(): void {
    this.userRoles = this.userRoleService.getUserRoles();
  }

  navigateHome(): string {
    if (this.userRoles?.includes('admin') || this.userRoles?.includes('librarian')) {
       return '/dashboard';
    }else{
      return '/books';
    }
    
  }

  isAdminOrLibrarian(): boolean {
    return this.userRoles.includes('admin') || this.userRoles.includes('librarian');
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.authService.clearToken();
    this.router.navigate(['/books'])
  }

  register():void {
    this.router.navigate(['/create-account']);
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  toggleSearch() {
    this.searchOpen = !this.searchOpen;
  }
}