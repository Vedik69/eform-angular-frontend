import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthStateService } from 'src/app/common/store';
import {Store} from '@ngrx/store';
import {selectAuthIsAuth, selectLoginRedirectUrl} from 'src/app/state/auth/auth.selector';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    //private authStateService: AuthStateService
    private store: Store
  ) {}

  private isAuth$ = this.store.select(selectAuthIsAuth);
  private loginRedirectUrl$ = this.store.select(selectLoginRedirectUrl);
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // TODO: Fix this
    //   if (!this.isAuth$) {
    //     console.debug(`Let's kick the user out auth.guard`);
    //     this.router.navigate(['/auth']).then();
    //     return false;
    //   }
    //   if (
    //     this.loginRedirectUrl$ &&
    //     (state.url === '/' || state.url === '/auth')
    //   ) {
    //     this.router
    //       .navigate([`/${this.loginRedirectUrl$}`])
    //       .then();
    //     return false;
    //   }
     return true;
  }
}
