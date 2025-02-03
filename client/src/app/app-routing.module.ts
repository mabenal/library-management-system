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
import { LoginComponent } from '../authentication/login/login.component';
import { CreateAccountComponent } from 'src/authentication/create-account/create-account.component';
import { ChangePasswordComponent } from 'src/authentication/change-password/change-password.component';
import { UpdateProfileComponent } from 'src/authentication/update-profile/update-profile.component';
import { AuthGuardService } from 'src/services/auth-guard.service';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [AuthGuardService] },
  { path: 'create-account', component: CreateAccountComponent},
  { path: 'update-profile', component: UpdateProfileComponent, canActivate: [AuthGuardService] },
  { path: 'search-results', component: SearchResultsComponent },
  { path: 'books', component: BooksComponent, canActivate: [AuthGuardService] },
  { path: 'clients', component: UserGroupingComponent, canActivate: [AuthGuardService] },
  { path: 'inventory', component: InventoryComponent, canActivate: [AuthGuardService] },
  { path: 'request-management', component: RequestManagementComponent, canActivate: [AuthGuardService] },
  { path: 'history', component: HistoryComponent, canActivate: [AuthGuardService] },
  { path: 'account', component: AccountComponent , canActivate: [AuthGuardService]},
  { path: 'settings', component: SettingsComponent , canActivate: [AuthGuardService]},
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuardService] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' } // Wildcard route for a 404 page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
