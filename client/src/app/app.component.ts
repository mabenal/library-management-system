import { Component, OnInit } from '@angular/core';
import { BooksService } from 'src/services/books.services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  constructor(private book: BooksService) {}

  ngOnInit() {
    this.returnBooks();
  }

  returnBooks() {
    this.book.books().subscribe(((res: any) => {
      console.log('Books returned successfully');
      console.log("response hello: ", res);
    }));
  }
}
