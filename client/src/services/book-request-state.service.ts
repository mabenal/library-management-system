import { Injectable } from '@angular/core';
import { RequestService } from './request.service';
import { BookRequestDto } from 'auto/autolmsclient-abstractions';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export enum ButtonState {
  Request = 'Request',
  Pending = 'Pending',
  Approved = 'Approved',
  Overdue = 'Overdue',
  Returned = 'Returned'
}

@Injectable({
  providedIn: 'root'
})
export class BookRequestStateService {
  constructor(private requestService: RequestService) {}

  checkRequestState(bookId: string): Observable<{ buttonState: ButtonState, buttonTitle: string, showCancelButton: boolean }> {
    return this.requestService.getBookRequestsByClient().pipe(
      map((requests: BookRequestDto[]) => {
        const request = requests.find(r => r.bookId === bookId);
        if (request) {
          switch (request.status) {
            case 'Pending':
              return { buttonState: ButtonState.Pending, buttonTitle: 'Pending Request', showCancelButton: true };
            case 'Approved':
              return { buttonState: ButtonState.Approved, buttonTitle: 'Approved Request', showCancelButton: false };
            case 'Overdue':
              return { buttonState: ButtonState.Overdue, buttonTitle: 'Overdue Request', showCancelButton: false };
            case 'Returned':
              return { buttonState: ButtonState.Returned, buttonTitle: 'Returned Request', showCancelButton: false };
            default:
              return { buttonState: ButtonState.Request, buttonTitle: 'Request', showCancelButton: false };
          }
        } else {
          return { buttonState: ButtonState.Request, buttonTitle: 'Request', showCancelButton: false };
        }
      })
    );
  }
}