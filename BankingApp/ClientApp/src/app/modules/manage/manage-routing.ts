import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ManageHomeComponent } from './manage-home/manage-home.component';
import { AuthGuard } from '../../guards/authGuard';
import { DepositFundsComponent } from './deposit-funds/deposit-funds.component';
import { WithdrawFundsComponent } from './withdraw-funds/withdraw-funds.component';

const routes: Routes = [
  {
    path : '',
    component: ManageHomeComponent,
    canActivate: [AuthGuard],
    canActivateChild : [AuthGuard],
    children : [
      {
        path: 'deposit',
        component: DepositFundsComponent
      },
      {
        path: 'withdraw',
        component: WithdrawFundsComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManageRoutingModule { }