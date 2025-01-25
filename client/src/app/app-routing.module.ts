import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
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


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'search-results', component: SearchResultsComponent },
  { path: 'books', component: BooksComponent },
  { path: 'clients', component: UserGroupingComponent },
  { path: 'inventory', component: InventoryComponent },
  { path: 'request-management', component: RequestManagementComponent },
  { path: 'history', component: HistoryComponent },
  { path: 'account', component: AccountComponent },
  { path: 'settings', component: SettingsComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' } // Wildcard route for a 404 page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
