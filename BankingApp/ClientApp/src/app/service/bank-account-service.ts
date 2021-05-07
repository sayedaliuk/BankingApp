import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import config from '../../assets/config.json';
import { BankAccountDto } from '../common/bank-account-dto';
import { AuthenticationService } from './authentication-service';
import { AccountTransactionDto } from '../common/account-transaction-dto';
import { Session } from '../common/session';


@Injectable({
  providedIn: 'root'
})
export class BankAccountService {
  constructor(private http: HttpClient, private authSvc : AuthenticationService) { }

  private _userSessionChanged : BehaviorSubject<Session> = new BehaviorSubject<Session>(null);
  private _accountBalanceUpdated : BehaviorSubject<number> = new BehaviorSubject<number>(null);  
  private _errorsChanged : BehaviorSubject<string> = new BehaviorSubject<string>(null);

  userSessionChanged = this._userSessionChanged.asObservable();
  currentUserAccountBalanceUpdated = this._accountBalanceUpdated.asObservable();
  errorsChanged = this._errorsChanged.asObservable();

  getAccountDetails(sortCode: string, accountNumber: string) : void {

    if(!this.authSvc.isLoggedIn()){
      this.authSvc.checkAndRouteToHome();
    }

    let uri = `${config.apiEndPoints.bankAccount}?accountNumber=${accountNumber}&sortCode=${sortCode}`;    

    this.authSvc.getAsync<BankAccountDto>(uri)
      .subscribe(x => {
          let session = this.authSvc.login(x);          
          this._userSessionChanged.next(session);

      }, err => {        
        this.handleApiError(err);
        this.authSvc.logout();
      });
  }

  depositFunds(dto : AccountTransactionDto) : void {
    if(!this.authSvc.isLoggedIn()){
      this.authSvc.checkAndRouteToHome();
    }

    let uri = `${config.apiEndPoints.bankAccount}/deposit`;  

    this.authSvc.postAsync(uri, dto)
      .subscribe(x => this._accountBalanceUpdated.next(x), err => this.handleApiError(err));
  }

  withdrawFunds(dto : AccountTransactionDto): void {

    if(!this.authSvc.isLoggedIn()){
      this.authSvc.checkAndRouteToHome();
    }

    let uri = `${config.apiEndPoints.bankAccount}/withdraw`;    

    this.authSvc.postAsync(uri, dto)
      .subscribe(x => this._accountBalanceUpdated.next(x), err => this.handleApiError(err));
  }

  handleApiError(err: any): void {

    if(err.status === 404) {
      this._errorsChanged.next('Account Not Found! Operation canceled!');

    } else if(err.status === 406) {
      this._errorsChanged.next('Invalid operation')
    }

    else {
      this._errorsChanged.next(`Server Errror : ${err.statusCode} - ${err.status}`);
    }
  }
}
