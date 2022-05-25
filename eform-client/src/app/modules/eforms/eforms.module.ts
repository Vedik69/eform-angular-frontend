import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {FileUploadModule} from 'ng2-file-upload';
import {MDBBootstrapModule} from 'angular-bootstrap-md';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {EformsRouting} from './eforms.routing';
import {
  EformColumnsModalComponent,
  EformCreateModalComponent,
  EformEditParingModalComponent,
  EformEditTagsModalComponent,
  EformExcelReportModalComponent,
  EformRemoveEformModalComponent,
  EformsBulkImportModalComponent,
  EformsPageComponent,
  EformUploadZipModalComponent,
  EformDuplicateConfirmModalComponent,
} from './components';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {OwlDateTimeModule} from '@danielmoncada/angular-datetime-picker';
import {persistProvider} from 'src/app/modules/eforms/store';
import {EformSharedTagsModule} from 'src/app/common/modules/eform-shared-tags/eform-shared-tags.module';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {EformMatDateFnsDateModule} from 'src/app/common/modules/eform-date-adapter/eform-mat-datefns-date-adapter.module';
import {MatSortModule} from '@angular/material/sort';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatDialogModule} from '@angular/material/dialog';
import {MatMenuModule} from '@angular/material/menu';
import {MtxSelectModule} from '@ng-matero/extensions/select';

@NgModule({
  imports: [
    CommonModule,
    EformsRouting,
    MDBBootstrapModule,
    EformSharedModule,
    ReactiveFormsModule,
    FileUploadModule,
    FormsModule,
    TranslateModule.forChild(),
    FontAwesomeModule,
    OwlDateTimeModule,
    EformSharedTagsModule,
    MatDatepickerModule,
    MatInputModule,
    EformMatDateFnsDateModule,
    MatSortModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatMenuModule,
    MtxSelectModule
  ],
  declarations: [
    EformsPageComponent,
    EformEditParingModalComponent,
    EformCreateModalComponent,
    EformColumnsModalComponent,
    EformEditTagsModalComponent,
    EformRemoveEformModalComponent,
    EformUploadZipModalComponent,
    EformExcelReportModalComponent,
    EformsBulkImportModalComponent,
    EformDuplicateConfirmModalComponent,
  ],
  providers: [persistProvider],
  exports: []
})
export class EFormsModule {
}
