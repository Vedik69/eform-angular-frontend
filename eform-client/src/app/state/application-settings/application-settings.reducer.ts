import {AdminSettingsModel, LanguagesModel, UserbackWidgetSettingModel} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {
  updateAdminSettings,
  updateLanguages,
  updateOthersSettings, updateUserbackWidgetSetting
} from 'src/app/state/application-settings/application-settings.actions';

export interface AppSettingsState {
  adminSettingsModel: AdminSettingsModel;
  othersSettings: UserbackWidgetSettingModel;
  languagesModel: LanguagesModel;
}

export const initialState: AppSettingsState = {
  adminSettingsModel: new AdminSettingsModel(),
  othersSettings: {
    isUserbackWidgetEnabled: false,
    userbackToken: ''
  },
  languagesModel: new LanguagesModel(),
};

export const _appSettingsReducer = createReducer(
  initialState,
  on(updateAdminSettings, (state, {payload}) => ({
    ...state,
    adminSettingsModel: payload,
    })
  ),
  on(updateOthersSettings, (state, {payload}) => ({
    ...state,
    othersSettings: payload,
    }
    )),
  on(updateLanguages, (state, {payload}) => ({
    ...state,
    languagesModel: payload,
    })
  ),
  on(updateUserbackWidgetSetting, (state, {payload}) => ({
    ...state,
    othersSettings: {
      ...state.othersSettings,
      isUserbackWidgetEnabled: payload.isUserbackWidgetEnabled,
      userbackToken: payload.userbackToken,
    },
    })
  ),
);
export function reducer(state: AppSettingsState | undefined, action: any) {
  return _appSettingsReducer(state, action);
}
