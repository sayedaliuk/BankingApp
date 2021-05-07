import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';
import { Session } from '../common/session';
import { BankAccountDto } from '../common/bank-account-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private http: HttpClient,                 
                private router: Router) 
    { 
        //
    }

  private _session : Session = null;

  isLoggedIn() : boolean {    
    return (this._session?.accountNumber !== null || this._session.sortCode !== null);
  }

  login(dto: BankAccountDto) : Session {     
    this._session = new Session();
    this._session.sortCode = dto.sortCode;
    this._session.accountNumber = dto.accountNumber;
    this._session.accountBalance = dto.balance;

    return this._session;
  }

  logout() : boolean {
      this._session = null;      
      return true;
  }

  getAsync<T>(uri: string): Observable<T> {      
      
      // if(this._session == null) {          
      //     this.router.navigate(['home']);
      //     return of<T>();
      // }

      const authHeaders = new HttpHeaders({
        'Content-Type': 'application/json',       
      });

      return this.http.get<T>(uri);
  }

  postAsync(uri: string, payload: any): Observable<any> {      
      
      if(this._session == null) {          
          this.router.navigate(['home']);
          return of<any>();
      }

      const authHeaders = new HttpHeaders({
        'Content-Type': 'application/json',        
      });

      return this.http.post(uri, payload);
  }

  checkUnauthorizedAccessAttempt(res : any) {
    if(res === null) return;
    if(res.status === 401) {
      this.router.navigate(['/home']);          
    }
  }

  checkAndRouteToHome() {    
    if(this._session == null) {
      this.router.navigate(['/']);  
    }    
  }

  navigateToAccountManagment() {        
    this.router.navigate(['/accountManagment']);   
  }

  routeToLogin() {    
    if(this._session == null) {
      this.router.navigate(['/home']);  
    }    
  }

}