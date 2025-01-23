import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.less']
})
export class PaginationComponent {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 3;
  @Output() pageChange = new EventEmitter<number>();

  /**
   * Generates an array of page numbers.
   * @returns An array of page numbers.
   */
  pagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  /**
   * Navigates to a specific page if the page number is within valid bounds.
   * @param page - The page number to navigate to.
   */
  navigatePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageChange.emit(page);
    this.currentPage = page; // Update currentPage after emitting the event
  }
}