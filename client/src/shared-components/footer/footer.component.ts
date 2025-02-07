import { Component, OnInit } from '@angular/core';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.less']
})
export class FooterComponent implements OnInit {
  displayConstants = DisplayConstants;

  constructor() { }

  ngOnInit(): void {
  }
}