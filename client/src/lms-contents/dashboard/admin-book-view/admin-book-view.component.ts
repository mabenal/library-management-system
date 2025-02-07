import { Component, OnInit, Input, HostListener } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';
import { Router } from '@angular/router';

type SortableFields = 'title' | 'author' | 'isbn' | 'category' | 'yearPublished' | 'numberOfCopies';

@Component({
  selector: 'app-admin-book-view',
  templateUrl: './admin-book-view.component.html',
  styleUrls: ['./admin-book-view.component.less']
})
export class AdminBookViewComponent implements OnInit {

  @Input() books: BookDto[] = [];
  @Input() displayConstants: any;
  @Input() enablePagination: boolean = true;

  filteredBooks: BookDto[] = [];
  selectedBook: BookDto | null = null;
  sortDirection: { [key in SortableFields]?: boolean } = {};
  currentPage: number = 1;
  itemsPerPage: number = 15;
  showModal: boolean = false;
  selectedBookId: string = '';
  isAddMode: boolean = false;

  constructor(private booksService: BooksService, private router: Router) { }

  ngOnInit(): void {
    this.fetchBooks();
  }

  fetchBooks(): void {
    this.booksService.books().subscribe(
      (books: BookDto[]) => {
        this.books = books;
        this.filteredBooks = this.books;
      },
      error => {
        console.error('Error fetching books:', error);
      }
    );
  }

  selectBook(book: BookDto) {
    this.selectedBook = book;
  }

  deleteBook(bookId: string): void {
    this.booksService.removeBook(bookId).subscribe(
      () => {
        this.books = this.books.filter(book => book.id !== bookId);
        this.filteredBooks = this.books;
      },
      error => {
        console.error('Error deleting book:', error);
      }
    );
  }

  openEditModal(bookId: string): void {
    this.selectedBookId = bookId;
    this.isAddMode = false;
    this.showModal = true;
    document.body.classList.add('modal-open');
  }

  openAddModal(): void {
    this.selectedBookId = '';
    this.isAddMode = true;
    this.showModal = true;
    document.body.classList.add('modal-open');
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedBookId = '';
    document.body.classList.remove('modal-open');
  }

  @HostListener('document:keydown.escape', ['$event'])
  handleEscapeKey(event: KeyboardEvent) {
    if (this.showModal) {
      this.closeModal();
    }
  }

  sortBooks(field: SortableFields): void {
    this.sortDirection[field] = !this.sortDirection[field];
    const direction = this.sortDirection[field] ? 1 : -1;
    this.filteredBooks.sort((a, b) => {
      const aField = a[field];
      const bField = b[field];
  
      if (aField === undefined || bField === undefined) {
        return 0;
      }
  
      if (typeof aField === 'string' && typeof bField === 'string') {
        return (aField as string).localeCompare(bField as string) * direction;
      } else if (!isNaN(Date.parse(aField as string)) && !isNaN(Date.parse(bField as string))) {
        return (new Date(aField).getTime() - new Date(bField).getTime()) * direction;
      } else {
        return (aField > bField ? 1 : -1) * direction;
      }
    });
  }

  filterBooks(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value.toLowerCase();
    this.filteredBooks = this.books.filter(book =>
      book.title?.toLowerCase().includes(searchTerm) ||
      book.author?.toLowerCase().includes(searchTerm) ||
      book.isbn?.toLowerCase().includes(searchTerm) ||
      book.category?.toLowerCase().includes(searchTerm)
    );
  }

  get paginatedBooks(): BookDto[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredBooks.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get totalPages(): number {
    return Math.ceil(this.filteredBooks.length / this.itemsPerPage);
  }
}