import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../app/guards/authGuard';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
  {
    path : 'accountManagement',
    canActivateChild : [AuthGuard],
    loadChildren : () => import('./modules/manage/manage.module').then(m => m.ManageModule)
  },
  {
    path : 'home',
    canActivate : [AuthGuard],
    component : DashboardComponent  
  },
  {
    path: '**',
    redirectTo: 'home',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes,  { enableTracing: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
