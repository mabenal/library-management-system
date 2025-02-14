import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { Client, API_BASE_URL } from 'auto/autolmsclient-abstractions';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksService } from '../services/books.services';
import { AuthService } from '../services/auth.service';
import { TopNavigatorComponent } from '../shared-components/top-navigator/top-navigator.component';
import { BookDetailsModalComponent } from '../shared-components//book-details-modal/book-details-modal.component';
import { BookListViewComponent } from '../shared-components//book-list-view/book-list-view.component';
import { BookSearchComponent } from '../shared-components/book-search/book-search.component';
import { PaginationComponent } from '../shared-components/pagination/pagination.component';
import { CategoryFilterComponent } from '../shared-components/category-filter/category-filter.component';
import { CancellationPromptComponent } from '../shared-components/cancellation-prompt/cancellation-prompt.component';
import { TruncatePipe } from '../lms-contents/book-details/truncate.pipe';
import { SearchResultsOverlayComponent } from '../shared-components/search-results-overlay/search-results-overlay.component';
import { AccountComponent } from '../lms-contents/account/account.component';
import { BooksComponent } from '../lms-contents/books/books.component';
import { DashboardComponent } from '../lms-contents/dashboard/dashboard.component';
import { RequestManagementComponent } from '../lms-contents/request-management/request-management.component';
import { SearchResultsComponent } from '../lms-contents/search-results/search-results.component';
import { BookDetailsComponent } from 'src/lms-contents/book-details/book-details.component';
import { UserGroupingComponent } from '../lms-contents/user-grouping/user-grouping.component';
import { HistoryComponent } from '../lms-contents/history/history.component';
import { LoginComponent } from '../authentication/login/login.component';
import { FormsModule } from '@angular/forms';
import { FooterComponent } from 'src/shared-components/footer/footer.component';
import { CreateAccountComponent } from 'src/authentication/create-account/create-account.component';
import { ChangePasswordComponent } from 'src/authentication/change-password/change-password.component';
import { LatestBooksComponent } from 'src/shared-components/latest-books/latest-books.component';
import { NgxsModule } from '@ngxs/store';
import { UserState } from 'src/authentication/store/state/user.state';
import { UpdateProfileComponent } from 'src/authentication/update-profile/update-profile.component';
import { CookieService } from 'ngx-cookie-service';
import { TokenInterceptor } from 'src/interceptors/token.interceptor';
import { RequestService } from 'src/services/request.service';
import { AuthGuardService } from 'src/services/auth-guard.service';
import { UserRoleService } from 'src/services/user-role.service';
import { BookRequestStateService } from 'src/services/book-request-state.service';
import { UpdateClientComponent } from 'src/lms-contents/update-client/update-client.component';
import { AdminBookViewComponent } from '../lms-contents/dashboard/admin-book-view/admin-book-view.component';
import { UpdateBookComponent} from 'src/lms-contents/dashboard/update-book/update-book.component';
import { ClientService } from 'src/services/client.service';
import { UserManagementService } from 'src/services/user-management.service';

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
    RequestManagementComponent,
    SearchResultsComponent,
    BookDetailsComponent,
    UserGroupingComponent,
    HistoryComponent,
    LoginComponent,
    TruncatePipe,
    FooterComponent,
    CreateAccountComponent,
    ChangePasswordComponent,
    UpdateProfileComponent,
    UpdateClientComponent,  
    LatestBooksComponent,
    UpdateClientComponent,
    AdminBookViewComponent,
    UpdateBookComponent,
    CancellationPromptComponent
  ],
  imports: [
    NgxsModule.forRoot([UserState]),
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    BooksService,
    AuthService,
    AuthGuardService,
    RequestService,
    Client,
    CookieService,
    UserRoleService,
    ClientService,
    BookRequestStateService,
    UserManagementService,
    { provide: 'IClient', useClass: Client },
    { provide: API_BASE_URL, useValue: 'https://localhost:7025' },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor , multi:true}
  ],
  bootstrap: [AppComponent]
})
export class SharedModule { }