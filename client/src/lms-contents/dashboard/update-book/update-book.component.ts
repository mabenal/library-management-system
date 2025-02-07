import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';

@Component({
  selector: 'app-update-book',
  templateUrl: './update-book.component.html',
  styleUrls: ['./update-book.component.less']
})
export class UpdateBookComponent implements OnInit {
  @Input() bookId!: string;
  @Input() isAddMode: boolean = false;
  @Output() close = new EventEmitter<void>();

  book: BookDto = {
    id: '',
    title: '',
    author: '',
    yearPublished: '',
    publisher: '',
    description: '',
    category: '',
    isbn: '',
    pageCount: 0,
    thumbnail: '',
    numberOfCopies: 0
  };
  errorMessage: string = '';

  constructor(
    private booksService: BooksService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (!this.isAddMode) {
      const bookId = this.bookId || this.route.snapshot.paramMap.get('id');
      if (bookId) {
        this.booksService.getBook(bookId).subscribe(
          book => {
            this.book = book;
          },
          error => {
            console.error('Error fetching book:', error);
            this.errorMessage = 'Failed to load book data';
          }
        );
      }
    }
  }

  closeModal() {
    this.close.emit();
  }

  saveBook(): void {
    if (this.isAddMode) {
      console.log('Adding book:', this.book);
      this.booksService.addBook(this.book).subscribe(
        newBook => {
          alert('Book added successfully');
          this.closeModal();
        },
        error => {
          console.error('Error adding book:', error);
          this.errorMessage = 'Failed to add book';
        }
      );
    } else {
      if (!this.book.id) {
        console.error('Book ID is undefined');
        this.errorMessage = 'Book ID is undefined';
        return;
      }
      this.booksService.updateBook(this.book.id, this.book).subscribe(
        updatedBook => {
          alert('Book updated successfully');
          this.closeModal();
        },
        error => {
          console.error('Error updating book:', error);
          this.errorMessage = 'Failed to update book';
        }
      );
    }
  }
}