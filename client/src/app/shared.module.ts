import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksService } from 'src/services/books.services';
import { TopNavigatorComponent } from 'src/shared-components/top-navigator/top-navigator.component';
import { BookDetailsModalComponent } from 'src/shared-components/book-details-modal/book-details-modal.component';
import { Client, API_BASE_URL } from 'auto/autolmsclient-module';
import { BookListViewComponent } from 'src/shared-components/book-list-view/book-list-view.component';
import { BookSearchComponent } from 'src/shared-components/book-search/book-search.component';
import { PaginationComponent } from 'src/shared-components/book-list-view/pagination/pagination.component';
import { CategoryFilterComponent } from 'src/shared-components/category-filter/category-filter.component';
import { TruncatePipe } from 'src/shared-components/book-details-modal/truncate.pipe';
import { SearchResultsOverlayComponent } from 'src/shared-components/search-results-overlay/search-results-overlay.component';

@NgModule({
  declarations: [
    AppComponent,
    BookDetailsModalComponent,
    TopNavigatorComponent,
    BookListViewComponent,
    BookSearchComponent,
    PaginationComponent,
    CategoryFilterComponent,
    SearchResultsOverlayComponent,
    TruncatePipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
  ],
  providers: [
    BooksService,
    { provide: 'IClient', useClass: Client },
    { provide: API_BASE_URL, useValue: 'https://localhost:7025' }
  ],
  bootstrap: [AppComponent]
})
export class SharedModule { }