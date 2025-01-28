import { Component, OnInit } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.less']
})
export class BooksComponent implements OnInit {
  selectedBook: BookDto | null = null;
  showModal: boolean = false;
  displayConstants: typeof DisplayConstants = DisplayConstants;
  books: BookDto[] = [];
  filteredBooks: BookDto[] = [];
  pickOfTheWeekBooks: BookDto[] = [];
  filterCategory: string | null = null;

  constructor(private bookService: BooksService) {}

  ngOnInit(): void {
    this.fetchBooks();
  }

  // Fetches the list of books from the service
  async fetchBooks() {
    try {
      this.books = await this.bookService.books().toPromise();
      this.filteredBooks = this.books;
      this.pickOfTheWeekBooks = this.getPickOfTheWeekBooks();
    } catch (error) {
      console.error('Error fetching books:', error);
    }
  }

  updateFilteredBooks(filteredBooks: BookDto[], category: string | null) {
    this.filteredBooks = filteredBooks;
    this.filterCategory = category;
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

  // Returns the last three books from the list
  getPickOfTheWeekBooks(): BookDto[] {
    return this.books.slice(-3).reverse();
  }
}