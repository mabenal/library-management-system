import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import { BookDto } from 'auto/autolmsclient-abstractions';
import { CATEGORIES } from 'src/constants';

@Component({
  selector: 'app-category-filter',
  templateUrl: './category-filter.component.html',
  styleUrls: ['./category-filter.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryFilterComponent implements OnInit {
  @Input() books: BookDto[] = [];
  @Output() filteredBooksChange = new EventEmitter<BookDto[]>();

  categoryKeys: string[] = [];
  selectedCategories: string[] = [];

  constructor() {}

  ngOnInit(): void {
    this.categoryKeys = Object.keys(CATEGORIES);
  }

  onToggleCategory(category: string) {
    // Clear the previous filter
    this.selectedCategories = [];

    // Add the newly selected category
    this.selectedCategories.push(category);

    // Filter the books
    this.filterBooks();
  }

  private getFilteredBooks(): BookDto[] {
    if (this.selectedCategories.length === 0) {
      return this.books;
    }

    return this.books.filter(book => {
      return this.selectedCategories.some(category => {
        if (!CATEGORIES[category]) {
          console.warn(`Category ${category} not found`);
          return false;
        }
        return CATEGORIES[category].some(subCategory => {
          return book.category && book.category.includes(subCategory);
        });
      });
    });
  }

  filterBooks() {
    const filteredBooks = this.getFilteredBooks();
    this.filteredBooksChange.emit(filteredBooks);
  }

  clearFilters() {
    this.selectedCategories = [];
    this.filterBooks();
  }
}