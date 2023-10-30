import {AppMenuState} from 'src/app/state/app-menu/app-menu.reducer';
import {AuthState} from 'src/app/state/auth/auth.recuder';
import {EformsState} from 'src/app/state/eform/eform.reducer';
import {DeviceUsersState} from 'src/app/state/device-user/device-user.reducer';


export interface AppState {
  appMenus: AppMenuState;
  auth: AuthState;
  eforms: EformsState
  deviceUsers: DeviceUsersState;
}
