import { Component, OnInit } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  selectedBook: BookDto | null = null;
  showModal: boolean = false;
  displayConstants: typeof DisplayConstants = DisplayConstants;
  books: BookDto[] = [];
  filteredBooks: BookDto[] = [];

  constructor(private bookService: BooksService) {}

  ngOnInit() {
    this.fetchBooks();
  }

  // Fetches the list of books from the service
  async fetchBooks() {
    try {
      this.books = await this.bookService.books().toPromise();
      this.filteredBooks = this.books;
    } catch (error) {
      console.error('Error fetching books:', error);
    }
  }

  updateFilteredBooks(filteredBooks: BookDto[]) {
    this.filteredBooks = filteredBooks;
  }

  openModal(book: BookDto) {
    if (book) {
      this.selectedBook = book;
      this.showModal = true;
    }
  }

  closeModal() {
    this.showModal = false;
  }
}