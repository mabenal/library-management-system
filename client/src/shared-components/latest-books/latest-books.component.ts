import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { BookDto } from 'auto/autolmsclient-abstractions';

@Component({
  selector: 'app-latest-books',
  templateUrl: './latest-books.component.html',
  styleUrls: ['./latest-books.component.less']
})
export class LatestBooksComponent implements OnInit {

  @Input() latestBooks: BookDto[] = [];
  @Output() showBookDetails = new EventEmitter<BookDto>();

  constructor(private router: Router) {}

  ngOnInit(): void {}

  onShowBookDetails(book: BookDto) {
    this.router.navigate(['/book', book.id]);
  }

}