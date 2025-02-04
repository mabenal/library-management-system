import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
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

  constructor(private authService: AuthService,private router: Router) { }

  ngOnInit(): void {
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