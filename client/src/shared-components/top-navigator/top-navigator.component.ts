import { Component, OnInit, Inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-top-navigator',
  templateUrl: './top-navigator.component.html',
  styleUrls: ['./top-navigator.component.less']
})
export class TopNavigatorComponent implements OnInit {
  displayConstants = DisplayConstants;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
  }

  isLoggedIn(): boolean {
    return true;
  }

  logout(){
    this.authService.clearToken();
  }
}
