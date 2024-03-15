import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from 'rxjs';
import { inject } from "@angular/core";
import { CommonService } from "src/app/services/common.service";
import { LoginResponse } from "src/app/models/user-item";
import { Router } from "@angular/router";

export const HomeGuard = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const commonService = inject(CommonService);
    const router = inject(Router);

    if (typeof commonService.userData !== 'undefined' && typeof  commonService.token !== 'undefined') {
        return true;
    } else {
        const strUserData: string = sessionStorage.getItem('UserData') || '';

        if (strUserData !== '') {
            const userData = JSON.parse(strUserData) as LoginResponse;
            commonService.token = userData.token;
            commonService.userData = userData.user
            return true;
        } else {
            return router.navigate(['/', 'login']).then((value: boolean) => false);
        }
    }
}

export const LoginGuard = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const commonService = inject(CommonService);
    const router = inject(Router);

    if (typeof commonService.userData !== 'undefined' && typeof  commonService.token !== 'undefined') {
        return router.navigate(['/', 'home', 'inicio']).then((value: boolean) => false);
    } else {
        const strUserData: string = sessionStorage.getItem('UserData') || '';

        if (strUserData !== '') {
            const userData = JSON.parse(strUserData) as LoginResponse;
            commonService.token = userData.token;
            commonService.userData = userData.user
            return router.navigate(['/', 'home', 'inicio']).then((value: boolean) => false);
        } else {
            return true;
        }
    }
}