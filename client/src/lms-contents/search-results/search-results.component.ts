import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BooksService } from 'src/services/books.services';
import { BookDto } from 'auto/autolmsclient-abstractions';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.less']
})
export class SearchResultsComponent implements OnInit {
  searchTerm: string = '';
  searchResults: BookDto[] = [];
  filteredResults: BookDto[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 15;
  searchTime: number = 0;
  publicationYears: number[] = [];
  exactMatch: boolean = false;
  sortOrder: string = 'asc';

  pageCountOptions: { value: string, label: string }[] = [
    { value: '', label: 'Any Pages' },
    { value: '0-100', label: '0-100' },
    { value: '101-200', label: '101-200' },
    { value: '201-300', label: '201-300' },
    { value: '301-400', label: '301-400' },
    { value: '401+', label: '401+' }
  ];
  sortOptions: { value: string, label: string }[] = [
    { value: 'asc', label: 'Ascending' },
    { value: 'desc', label: 'Descending' }
  ];

  constructor(private route: ActivatedRoute, private booksService: BooksService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.searchTerm = params['q'] || '';
      this.searchBooks();
    });
  }

  searchBooks(): void {
    const startTime = performance.now();
    if (this.searchTerm.trim()) {
      this.booksService.searchBooks(this.searchTerm).subscribe((results: BookDto[]) => {
        const endTime = performance.now();
        this.searchTime = Number(((endTime - startTime) / 1000).toFixed(2)); // Ensure searchTime is a number
        this.searchResults = results;
        this.filteredResults = results;
        this.publicationYears = [...new Set(results.map((book: BookDto) => parseInt(book.yearPublished)))].sort((a, b) => b - a); // Handle book type and ensure publicationYears is a number array
        this.sortResults();
      });
    }
  }

  toggleExactMatch(event: Event): void {
    this.exactMatch = (event.target as HTMLInputElement).checked;
    this.filterResults();
  }

  filterByPageCount(event: Event): void {
    const range = (event.target as HTMLSelectElement).value;
    this.filterResults(range, 'pageCount');
  }

  filterByYear(event: Event): void {
    const year = (event.target as HTMLSelectElement).value;
    this.filterResults(year, 'yearPublished');
  }

  sortResults(event?: Event): void {
    if (event) {
      this.sortOrder = (event.target as HTMLSelectElement).value;
    }
    this.filteredResults.sort((a, b) => {
      if (this.sortOrder === 'asc') {
        return a.title.localeCompare(b.title);
      } else {
        return b.title.localeCompare(a.title);
      }
    });
  }

  filterResults(value: string = '', type: string = ''): void {
    this.filteredResults = this.searchResults.filter(book => {
      let matches = true;
      if (this.exactMatch) {
        matches = book.title === this.searchTerm;
      }
      if (type === 'pageCount' && value) {
        const [min, max] = value.split('-').map(Number);
        matches = matches && book.pageCount >= min && (max ? book.pageCount <= max : true);
      }
      if (type === 'yearPublished' && value) {
        matches = matches && book.yearPublished === value;
      }
      return matches;
    });
    this.sortResults();
  }

  get paginatedResults(): BookDto[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredResults.slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredResults.length / this.itemsPerPage);
  }

  navigatePage(page: number): void {
    this.currentPage = page;
  }
}