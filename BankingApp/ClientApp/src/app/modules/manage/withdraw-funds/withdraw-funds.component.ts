import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AccountTransactionDto } from 'src/app/common/account-transaction-dto';
import { BankAccountService } from 'src/app/service/bank-account-service';


@Component({
  selector: 'app-withdraw-funds',
  templateUrl: './withdraw-funds.component.html',
  styleUrls: ['./withdraw-funds.component.css']
})
export class WithdrawFundsComponent implements OnInit, OnDestroy {
  constructor(private bankSvc: BankAccountService) { }

  private sessionObs : Subscription = null;

  currency: string = null;
  amount: number = 0;
  accountNumber: string = null;
  sortCode: string = null;
  
  ngOnInit(): void {
    this.sessionObs = this.bankSvc.userSessionChanged.subscribe(x => {
      if(x !== null){
        this.accountNumber = x.accountNumber;
        this.sortCode = x.sortCode;
      }
    });
  }

  ngOnDestroy(): void {
    this.sessionObs?.unsubscribe();
  }

  onSubmit(): void {
    if(!this.isValid()){
      return;
    }

    var dto =  new AccountTransactionDto();
    dto.sortCode = this.sortCode;
    dto.currency = this.currency;
    dto.amount = this.amount;
    dto.accountNumber = this.accountNumber;

    this.bankSvc.withdrawFunds(dto);
  }

  isValid(): boolean {
    return this.amount > 0 && this.accountNumber !== null && this.sortCode !== null;
  }
}

