import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, tap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from '../components/dialog/dialog.component';
import { DialogDataItem } from '../models/dialog-data-item';

@Injectable({
  providedIn: 'root'
})
export class ServiceInterceptor implements HttpInterceptor {

  constructor(private dialog: MatDialog) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    return next.handle(request).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse) {
          switch(error.status) {
            case 500:
            case 400:
              this.dialog.open<DialogComponent, DialogDataItem, boolean>(DialogComponent, 
                {
                  data: {Title: 'Error', Content: error.message, Type: 'message'},
                  width: '400px',
                  height: '300px'
                }
              )
              break;
          }
        }
        throw ''
      })
      )

  }
}
