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
  @Output() filteredBooksChange = new EventEmitter<{ filteredBooks: BookDto[], category: string | null }>();

  categoryKeys: string[] = [];
  selectedCategories: string[] = [];
  CATEGORIES = CATEGORIES;

  constructor() {}

  ngOnInit(): void {
    this.categoryKeys = Object.keys(CATEGORIES);
  }

  onToggleCategory(category: string) {
    this.selectedCategories = [];

    this.selectedCategories.push(category);

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
        return CATEGORIES[category].subCategories.some(subCategory => {
          return book.category && book.category.includes(subCategory);
        });
      });
    });
  }

  filterBooks() {
    const filteredBooks = this.getFilteredBooks();
    const category = this.selectedCategories.length > 0 ? this.selectedCategories[0] : null;
    this.filteredBooksChange.emit({ filteredBooks, category });
  }

  clearFilters() {
    this.selectedCategories = [];
    this.filterBooks();
  }
}