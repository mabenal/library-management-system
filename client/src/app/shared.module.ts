import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { Client, API_BASE_URL } from 'auto/autolmsclient-module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksService } from '../services/books.services';
import { AuthService } from '../services/auth.service';

import { TopNavigatorComponent } from '../shared-components/top-navigator/top-navigator.component';
import { BookDetailsModalComponent } from '../shared-components//book-details-modal/book-details-modal.component';
import { BookListViewComponent } from '../shared-components//book-list-view/book-list-view.component';
import { BookSearchComponent } from '../shared-components/book-search/book-search.component';
import { PaginationComponent } from '../shared-components/book-list-view/pagination/pagination.component';
import { CategoryFilterComponent } from '../shared-components/category-filter/category-filter.component';
import { TruncatePipe } from '../shared-components/book-details-modal/truncate.pipe';
import { SearchResultsOverlayComponent } from '../shared-components/search-results-overlay/search-results-overlay.component';
import { AccountComponent } from '../lms-contents/account/account.component';
import { BooksComponent } from '../lms-contents/books/books.component';
import { DashboardComponent } from '../lms-contents/dashboard/dashboard.component';
import { InventoryComponent } from '../lms-contents/inventory/inventory.component';
import { RequestManagementComponent } from '../lms-contents/request-management/request-management.component';
import { SearchResultsComponent } from '../lms-contents/search-results/search-results.component';
import { SettingsComponent } from '../lms-contents/settings/settings.component';
import { UserGroupingComponent } from '../lms-contents/user-grouping/user-grouping.component';
import { HistoryComponent } from '../lms-contents/history/history.component';
import { LoginComponent } from '../authentication/lms-contents/login/login.component';
import { FormsModule } from '@angular/forms';

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
    AccountComponent,
    BooksComponent,
    DashboardComponent,
    InventoryComponent,
    RequestManagementComponent,
    SearchResultsComponent,
    SettingsComponent,
    UserGroupingComponent,
    HistoryComponent,
    LoginComponent,
    TruncatePipe,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    BooksService,
    AuthService,
    Client,
    { provide: 'IClient', useClass: Client },
    { provide: API_BASE_URL, useValue: 'https://localhost:7025' }
  ],
  bootstrap: [AppComponent]
})
export class SharedModule { }