import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { BankAccountService } from '../../service/bank-account-service';
import { Session } from '../../common/session';
import { AuthenticationService } from 'src/app/service/authentication-service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  constructor(private bankSvc : BankAccountService, private authSvc: AuthenticationService) { }

  private userSessionObs : Subscription = null;
  private serviceErrorObs : Subscription = null;

  messages : string[] = [];
  sortCode : string = "";
  accountNumber : string = "";
  isLoggedIn: boolean = false;


  ngOnInit(): void {
    this.userSessionObs = this.bankSvc.userSessionChanged.subscribe(x => {
      this.isLoggedIn = x !== null;

      if(this.isLoggedIn) {
        this.authSvc.navigateToAccountManagment();
      }
    });
    this.serviceErrorObs = this.bankSvc.errorsChanged.subscribe(x => {
      this.messages.length = 0;
      this.messages.push(x);
    });
  }

  ngOnDestroy(): void {    
    this.userSessionObs?.unsubscribe();
    this.serviceErrorObs?.unsubscribe();
  }

  onSubmit(): void {
    let isValid = true;
    this.messages.length = 0;

    if(this.sortCode.toString().length != 6) {
      this.messages.push('Invalid Sort Code');
      isValid = false;
    }

    if(this.accountNumber.toString().length != 8) {
      this.messages.push('Invalid Account Noe');
      isValid = false;
    }

    if(!isValid) {
      return;
    }

    this.bankSvc.getAccountDetails(this.sortCode, this.accountNumber);    
  }

}
