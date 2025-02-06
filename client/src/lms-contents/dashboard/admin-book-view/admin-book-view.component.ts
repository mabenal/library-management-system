import { Component, OnInit, Input } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-book-view',
  templateUrl: './admin-book-view.component.html',
  styleUrls: ['./admin-book-view.component.less']
})
export class AdminBookViewComponent implements OnInit {

  @Input() books: BookDto[] = []; // List of books

  @Input() displayConstants: any;
  @Input() enablePagination: boolean = true; // Add this line

  selectedBook: BookDto | null = null;

  currentPage: number = 1;
  itemsPerPage: number = 5;

  constructor(private booksService: BooksService, private router: Router) { }

  ngOnInit(): void {
  }


  selectClient(book: BookDto) {
    this.selectedBook = book;
  }

  deleteBook(bookId: string): void {
    this.booksService.removeBook(bookId).subscribe(
      () => {
        this.books = this.books.filter(book => book.id !== bookId);
        console.log(`Book with id ${bookId} deleted successfully`);
      },
      error => {
        console.error('Error deleting book:', error);
      }
    );
  }

  editBook(bookId: string): void {
    this.router.navigate(['/update-book', bookId]);
  }

  get paginatedBooks(): BookDto[] {
    if (!this.enablePagination) {
      return this.books;
    }
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.books.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get totalPages(): number {
    return Math.ceil(this.books.length / this.itemsPerPage);
  }

}
