import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, CanActivateChild } from '@angular/router';
import { AuthenticationService } from '../service/authentication-service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate, CanActivateChild {
    constructor(private authSvc : AuthenticationService) { }

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean|UrlTree {       
        // var isLoggedIn = this.authSvc.isLoggedIn();
        // if(!isLoggedIn) {                     
        //     return  this.authSvc.getLoginRoute();  
        // }

        return true;
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean|UrlTree {
        console.log('Can Activate Child' + state.url);
        return this.canActivate(route, state);
    }

}