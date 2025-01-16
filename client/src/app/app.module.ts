import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksService } from 'src/services/books.services';
import { BookDetailsModalComponent } from 'src/shared-components/book-details-modal/book-details-modal.component';
import { Client, API_BASE_URL } from 'auto/autolmsclient-module';

@NgModule({
  declarations: [
    AppComponent,
    BookDetailsModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [
    BooksService,
    { provide: 'IClient', useClass: Client },
    { provide: API_BASE_URL, useValue: 'https://localhost:7025' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }