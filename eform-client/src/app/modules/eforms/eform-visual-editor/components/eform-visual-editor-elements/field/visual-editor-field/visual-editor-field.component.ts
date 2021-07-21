import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  applicationLanguages,
  EformFieldTypesEnum,
} from 'src/app/common/const';
import {
  EformVisualEditorFieldModel,
  EformVisualEditorFieldsDnDRecursionModel,
  EformVisualEditorRecursionFieldModel,
} from 'src/app/common/models';
import { eformVisualEditorElementColors } from 'src/app/modules/eforms/eform-visual-editor/const/eform-visual-editor-element-types';
import { LocaleService } from 'src/app/common/services';
import * as R from 'ramda';
import { CollapseComponent } from 'angular-bootstrap-md';

@Component({
  selector: 'app-visual-editor-field',
  templateUrl: './visual-editor-field.component.html',
  styleUrls: ['./visual-editor-field.component.scss'],
})
export class VisualEditorFieldComponent implements OnInit, OnDestroy {
  @ViewChild('nestedFields') nestedFields: CollapseComponent;
  @Input() field: EformVisualEditorFieldModel;
  @Input() fieldIndex: number;
  @Input() checklistRecursionIndexes = [];
  @Input() parentFieldIndex?: number;
  @Input() fieldIsNested = false;
  @Output()
  addNewField: EventEmitter<EformVisualEditorRecursionFieldModel> = new EventEmitter();
  @Output()
  // tslint:disable-next-line:max-line-length
  fieldPositionChanged: EventEmitter<EformVisualEditorFieldsDnDRecursionModel> = new EventEmitter<EformVisualEditorFieldsDnDRecursionModel>();
  @Output()
  deleteField: EventEmitter<EformVisualEditorRecursionFieldModel> = new EventEmitter();
  @Output()
  editField: EventEmitter<EformVisualEditorRecursionFieldModel> = new EventEmitter();
  @Output()
  changeColor: EventEmitter<EformVisualEditorRecursionFieldModel> = new EventEmitter();
  @Output()
  copyField: EventEmitter<EformVisualEditorRecursionFieldModel> = new EventEmitter();

  get fieldTypes() {
    return EformFieldTypesEnum;
  }

  get fieldColors() {
    return eformVisualEditorElementColors;
  }

  get isFieldComplete() {
    return (
      this.field.translations.find((x) => x.name !== '') && this.field.fieldType
    );
  }

  get getTranslation(): string {
    const languageId = applicationLanguages.find(
      (x) => x.locale === this.localeService.getCurrentUserLocale()
    ).id;
    return this.field.translations.find((x) => x.languageId === languageId)
      .name;
  }

  constructor(private localeService: LocaleService) {}

  ngOnInit() {}

  onAddNewField() {
    this.addNewField.emit({
      fieldIndex: this.fieldIndex,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
    });
  }

  onEditField() {
    this.editField.emit({
      field: { ...this.field },
      fieldIndex: this.fieldIndex,
      parentFieldIndex: this.parentFieldIndex,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
    });
  }

  onDeleteField() {
    this.deleteField.emit({
      field: { ...this.field },
      fieldIndex: this.fieldIndex,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
      parentFieldIndex: this.parentFieldIndex,
    });
  }

  ngOnDestroy(): void {}

  onChangeColor(color: string) {
    this.changeColor.emit({
      field: { ...this.field, color: color },
      fieldIndex: this.fieldIndex,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
      parentFieldIndex: this.parentFieldIndex,
    });
  }

  onCopyField(field) {
    this.copyField.emit({
      field: R.clone(field),
      fieldIndex: this.fieldIsNested ? this.fieldIndex : null,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
      parentFieldIndex: this.parentFieldIndex,
    });
  }

  toggleCollapse(field: EformVisualEditorFieldModel) {
    field.collapsed = !field.collapsed;
    this.nestedFields.toggle();
  }

  onChangeColorOnNestedField(fieldModel: EformVisualEditorRecursionFieldModel) {
    this.changeColor.emit(fieldModel);
  }

  onCopyFieldOnNestedField(fieldModel: EformVisualEditorRecursionFieldModel) {
    this.copyField.emit(fieldModel);
  }

  onDeleteFieldOnNestedField(fieldModel: EformVisualEditorRecursionFieldModel) {
    this.deleteField.emit(fieldModel);
  }

  onEditFieldOnNestedField(fieldModel: EformVisualEditorRecursionFieldModel) {
    this.editField.emit(fieldModel);
  }

  onFieldPositionChangedOnNestedField(
    fieldModel: EformVisualEditorFieldModel[]
  ) {
    this.fieldPositionChanged.emit({
      fields: fieldModel,
      fieldIndex: this.fieldIndex,
      checklistRecursionIndexes: this.checklistRecursionIndexes,
    });
  }
}
