import {UserClaimsModel} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {authenticate, loadAuthFailure, loadAuthSuccess, refreshToken} from 'src/app/state/auth/auth.actions';

export interface AuthState {
  token: {
    accessToken: string;
    expiresIn: any;
    tokenType: string;
    role: string;
  };
  currentUser: {
    firstName: string;
    lastName: string;
    id: number;
    userName: string;
    locale: string;
    darkTheme: boolean;
    loginRedirectUrl: string;
    claims: UserClaimsModel;
  };
  connectionString: {
    isConnectionStringExist: boolean;
    count: number;
  };
  sideMenuOpened: boolean;
  error: string;
  status: 'pending' | 'loading' | 'error' | 'success';
}

export const createInitialState: AuthState = {
  token: {
    accessToken: '',
    expiresIn: '',
    tokenType: '',
    role: '',
  },
  currentUser: {
    firstName: '',
    lastName: '',
    id: 0,
    userName: '',
    locale: 'da', // TODO add env for test run
    darkTheme: false,
    loginRedirectUrl: '',
    claims: {
      unitsRead: false,
      unitsUpdate: false,
      workersCreate: false,
      workersRead: false,
      workersUpdate: false,
      workersDelete: false,
      sitesCreate: false,
      sitesRead: false,
      sitesUpdate: false,
      sitesDelete: false,
      entitySearchCreate: false,
      entitySearchRead: false,
      entitySearchUpdate: false,
      entitySearchDelete: false,
      entitySelectCreate: false,
      entitySelectRead: false,
      entitySelectUpdate: false,
      entitySelectDelete: false,
      deviceUsersCreate: false,
      deviceUsersRead: false,
      deviceUsersUpdate: false,
      deviceUsersDelete: false,
      usersCreate: false,
      usersRead: false,
      usersUpdate: false,
      usersDelete: false,
      eformsCreate: false,
      eformsDelete: false,
      eformsRead: false,
      eformsUpdateColumns: false,
      eformsDownloadXml: false,
      eformsUploadZip: false,
      casesRead: false,
      caseRead: false,
      caseUpdate: false,
      caseDelete: false,
      caseGetPdf: false,
      caseGetDocx: false,
      caseGetPptx: false,
      eformsPairingUpdate: false,
      eformsUpdateTags: false,
      eformsPairingRead: false,
      eformsReadTags: false,
      eformsGetCsv: false,
      eformsReadJasperReport: false,
      eformsUpdateJasperReport: false,
      eformAllowManagingEformTags: false,
    },
  },
  connectionString: {
    isConnectionStringExist: false,
    count: 0,
  },
  sideMenuOpened: false,
  error: null,
  status: 'pending',
};

export const _authReducer = createReducer(
  createInitialState,
  on(authenticate, (state) => ({
    ...state,
    status: 'loading',
    }),
  ),
  on(refreshToken, (state) => ({
    ...state,
    status: 'loading',
    }),
  ),
  on(loadAuthSuccess, (state, {payload}) => ({
    ...state,
    status: 'success',
    token: payload.model.token,
    currentUser: payload.model.currentUser,
    connectionString: payload.model.connectionString,
    })),
  on(loadAuthFailure, (state, {payload}) => ({
    ...state,
    error: payload,
    status: 'error',
    })),
);
export function reducer(state: AuthState | undefined, action: any) {
  return _authReducer(state, action);
}
