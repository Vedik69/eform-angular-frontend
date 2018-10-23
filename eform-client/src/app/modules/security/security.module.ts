import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgSelectModule} from '@ng-select/ng-select';
import {TranslateModule} from '@ngx-translate/core';
import {MDBBootstrapModule} from 'port/angular-bootstrap-md';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {SecurityGroupsService} from 'src/app/common/services';
import {SecurityRouting} from './security.routing';
import {
  SecurityPageComponent,
  SecurityGroupCreateComponent,
  SecurityGroupRemoveComponent,
  SecurityGroupUpdateComponent,
  SecurityGroupGeneralPermissionsComponent,
  SecurityGroupEformsPermissionsComponent
} from './components';

@NgModule({
  imports: [
    EformSharedModule,
    CommonModule,
    SecurityRouting,
    NgSelectModule,
    TranslateModule,
    MDBBootstrapModule,
    FormsModule
  ],
  declarations: [
    SecurityPageComponent,
    SecurityGroupCreateComponent,
    SecurityGroupRemoveComponent,
    SecurityGroupUpdateComponent,
    SecurityGroupGeneralPermissionsComponent,
    SecurityGroupEformsPermissionsComponent
  ],
  providers: [SecurityGroupsService]
})
export class SecurityModule {
}
