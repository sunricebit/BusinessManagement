import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { SnackbarService } from './snackbar.service';
import jwt_decode from "jwt-decode";
import { GlobalContants } from '../shared/global-constants';
@Injectable({
  providedIn: 'root'
})
export class RouteGuardService {

  constructor(public auth: AuthService,
    public router:Router,
    private snackbarService: SnackbarService) { }
    
    canActivate(router:ActivatedRouteSnapshot):boolean{
      let expectedRoleArray = router.data;
      expectedRoleArray = expectedRoleArray.expectedRole;

      const token:any = localStorage.getItem('token');

      var tokenPayLoad:any;
      try{
        tokenPayLoad = jwt_decode(token);
      }
      catch(error){
        localStorage.clear();
        this.router.navigate(['/']);
      }
      
      let expectedRole = '';
      
      for (let i=0; i < expectedRoleArray.length;i++){
        if(expectedRoleArray[i] == tokenPayLoad.role){
          expectedRole = tokenPayLoad.role;
        }
      }

      if(tokenPayLoad.role == 'user' || tokenPayLoad.role == 'admin'){
        console.log('den day roi');
        if (this.auth.isAuthenticated() && tokenPayLoad.role == expectedRole){
          return true;
        }

        this.snackbarService.openSnackBar(GlobalContants.unauthorized, GlobalContants.error);
        this.router.navigate(['/business/dashboard']);
        return false;
      }
      else{
        this.router.navigate(['/']);
        localStorage.clear();
        return false;
      }
    }
}
