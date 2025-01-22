import { Component, Inject, OnInit } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  selectedBook!: BookDto;
  showModal: boolean = false;
  displayConstants: any = DisplayConstants;
  books: BookDto[] = [];
  filteredBooks: BookDto[] = [];
  genres: string[] = [
    'Fiction', 'Non-Fiction', 'Science Fiction', 'Fantasy', 'Mystery', 'Thriller',
    'Romance', 'Horror', 'Biography', 'History', 'Self-Help', 'Children'
  ];

  constructor(@Inject(BooksService) private book: BooksService) {}

  ngOnInit() {
    this.returnBooks();
  }

  async returnBooks() {
    try {
      this.books = await this.book.books().toPromise();
      this.filteredBooks = this.books; // Initialize filteredBooks with all books
    } catch (error) {
      console.error('Error fetching books:', error);
    }
  }

  filterBooksByGenre(genre: string) {
    this.filteredBooks = this.books.filter(book => book.category === genre);
  }

  openModal(book: BookDto) {
    this.selectedBook = book;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }
}