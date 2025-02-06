import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { BooksService } from 'src/services/books.services';

@Component({
  selector: 'app-update-book',
  templateUrl: './update-book.component.html',
  styleUrls: ['./update-book.component.less']
})
export class UpdateBookComponent implements OnInit {

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
    const bookId = this.route.snapshot.paramMap.get('id');
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

  updateBook(): void {
    this.booksService.updateBook(this.book.id, this.book).subscribe(
      updatedBook => {
        alert('Book updated successfully');
        this.router.navigate(['/dashboard']); 
      },
      error => {
        console.error('Error updating book:', error);
        this.errorMessage = 'Failed to update book';
      }
    );
  }
}
