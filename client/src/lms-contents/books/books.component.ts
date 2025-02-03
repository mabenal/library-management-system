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
  latestBooks: BookDto[] = [];
  filterCategory: string | null = null;

  constructor(private bookService: BooksService) {}

  ngOnInit(): void {
    this.fetchBooks();
  }

  async fetchBooks() {
    try {
      this.books = await this.bookService.books().toPromise();
      this.filteredBooks = this.books;
      this.latestBooks = this.getlatestBooks();
    } catch (error) {
      console.error('Error fetching books:', error);
    }
  }

  updateFilteredBooks(filteredBooks: BookDto[], category: string | null) {
    this.filteredBooks = filteredBooks;
    this.filterCategory = category;
  }

  showBookDetailsModal(book: BookDto) {
    this.selectedBook = book;
    this.showModal = true;
  }

  closeBookDetailsModal() {
    this.selectedBook = null;
    this.showModal = false;
  }

  getlatestBooks(): BookDto[] {
    return this.books.slice(-12).reverse();
  }
}