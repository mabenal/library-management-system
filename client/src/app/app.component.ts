import { Component, OnInit } from '@angular/core';
import { BooksService } from 'src/services/books.services';
import { DisplayConstants } from 'src/constants/constants';
import { BookDto } from 'auto/autolmsclient-abstractions';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  selectedBook!: BookDto;
  showModal: boolean = false;
  displayConstants: any = DisplayConstants;

  constructor(private book: BooksService) {}

  ngOnInit() {
    this.returnBooks();
  }

  returnBooks() {
    this.book.books().subscribe((res: BookDto[]) => {
      this.selectedBook = res[0]; // Select the first book for demonstration
    });
  }

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }
}