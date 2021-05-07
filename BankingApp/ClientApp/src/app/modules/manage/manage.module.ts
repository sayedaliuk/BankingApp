import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManageRoutingModule } from './manage-routing';
import { ManageHomeComponent } from './manage-home/manage-home.component';
import { ManageToolbarComponent } from './manage-toolbar/manage-toolbar.component';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { DepositFundsComponent } from './deposit-funds/deposit-funds.component';
import { WithdrawFundsComponent } from './withdraw-funds/withdraw-funds.component';


@NgModule({
  declarations: [
    ManageHomeComponent,
    ManageToolbarComponent,
    DepositFundsComponent,
    WithdrawFundsComponent,
  ],
  imports: [
    CommonModule,
    ManageRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [ ManageHomeComponent ]
})
export class ManageModule { }
