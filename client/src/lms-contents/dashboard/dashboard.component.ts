import { Component, OnInit } from '@angular/core';
import { BooksService } from 'src/services/books.services';
import { BookDto } from 'auto/autolmsclient-abstractions';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit {

  books: BookDto[] = [];

  constructor(private booksService: BooksService) { }

  ngOnInit(): void {
    this.fetchBooks();
  }

  fetchBooks(): void {
    this.booksService.books().subscribe(
      (books) => {
        this.books = books;
        console.log('Fetched books:', this.books); // Log the fetched books to the console
      },
      (error) => {
        console.error('Error fetching books:', error);
      }
    );
  }

}
