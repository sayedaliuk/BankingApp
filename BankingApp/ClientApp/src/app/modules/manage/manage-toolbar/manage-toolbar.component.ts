import { Component, OnInit,Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BankAccountService } from 'src/app/service/bank-account-service';

@Component({
  selector: 'app-manage-toolbar',
  templateUrl: './manage-toolbar.component.html',
  styleUrls: ['./manage-toolbar.component.css']
})
export class ManageToolbarComponent implements OnInit, OnDestroy {
  constructor(private bankSvc: BankAccountService) { }

  private sessionObs : Subscription = null;
  private svcErrorObs: Subscription = null;
  private accBalObs: Subscription = null;

  @Input() moduleHeading : string;

  accountNumber: string = null;
  sortCode: string = null;
  accountBalance: number = null;
  messages: string[] = [];

  ngOnInit(): void {

    this.sessionObs = this.bankSvc.userSessionChanged.subscribe(x => {
      if(x !== null){
        this.accountNumber = x.accountNumber;
        this.sortCode = x.sortCode;
        this.accountBalance = x.accountBalance;
      }
    });

    this.svcErrorObs = this.bankSvc.errorsChanged.subscribe(x => {
      this.messages.length = 0;
      if(x !== null){       
        this.messages.push(x);
      }
    });

    this.accBalObs = this.bankSvc.currentUserAccountBalanceUpdated.subscribe(x => {
      if(x !== null){
        this.accountBalance = x;
      }
    });
  }

  ngOnDestroy(): void {
    this.sessionObs?.unsubscribe();
    this.svcErrorObs?.unsubscribe();
    this.accBalObs?.unsubscribe();
  }
}
