import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit {

  cards = [
    {
      title: 'Books',
      subTitle: 'Manage Books',
      text: 'View and manage all books.',
      buttonText: 'Go to Books',
      buttonLink: '/all-books',
      buttonClass: 'bg-primary'
    },
    {
      title: 'Clients',
      subTitle: 'Manage Clients',
      text: 'View and manage all clients.',
      buttonText: 'Go to Clients',
      buttonLink: '/clients',
      buttonClass: 'bg-success'
    },
    {
      title: 'Book Requests',
      subTitle: 'Manage Book Requests',
      text: 'View and manage all book requests.',
      buttonText: 'Go to Book Requests',
      buttonLink: '/request-management',
      buttonClass: 'bg-warning'
    },
    {
      title: 'Other',
      subTitle: 'Other Management',
      text: 'Additional management options.',
      buttonText: 'Go to Other',
      buttonLink: '/other',
      buttonClass: 'bg-danger'
    }
  ];

  constructor() { }

  ngOnInit(): void { }

}