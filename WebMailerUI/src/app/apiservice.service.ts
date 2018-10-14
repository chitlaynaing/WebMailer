import { Mail } from './mail';
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class APIServiceService {

  _url = 'http://localhost:50129/api/Mail';
  
  constructor(private _http: HttpClient) { }

  sendMail (email: Mail) {

    return this._http.post<any>(this._url, email)
      .pipe(catchError(this.errorHandler))
  }

  errorHandler(error: HttpErrorResponse) {
    return throwError(error)
  }
}
