import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';
import { applicationLanguages } from 'src/app/common/const';
import {
  CommonDictionaryModel,
  EformVisualEditorModel,
} from 'src/app/common/models';
import { SharedTagsComponent } from 'src/app/common/modules/eform-shared-tags/components';
import {
  EformTagService,
  EformVisualEditorService,
} from 'src/app/common/services';

@AutoUnsubscribe()
@Component({
  selector: 'app-eform-visual-editor-container',
  templateUrl: './eform-visual-editor-container.component.html',
  styleUrls: ['./eform-visual-editor-container.component.scss'],
})
export class EformVisualEditorContainerComponent implements OnInit, OnDestroy {
  @ViewChild('tagsModal') tagsModal: SharedTagsComponent;

  visualEditorTemplateModel: EformVisualEditorModel = new EformVisualEditorModel();
  selectedTemplateId: number;
  availableTags: CommonDictionaryModel[] = [];
  isItemsCollapsed = false;

  getTagsSub$: Subscription;
  getVisualTemplateSub$: Subscription;
  createVisualTemplateSub$: Subscription;
  updateVisualTemplateSub$: Subscription;

  constructor(
    private tagsService: EformTagService,
    private visualEditorService: EformVisualEditorService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.getTags();

    this.route.params.subscribe((params) => {
      this.selectedTemplateId = params['templateId'];
      if (this.selectedTemplateId) {
        this.getVisualTemplate(this.selectedTemplateId);
      } else {
        this.initForm();
      }
    });
  }

  getVisualTemplate(templateId: number) {
    this.getVisualTemplateSub$ = this.visualEditorService
      .getVisualEditorTemplate(templateId)
      .subscribe((data) => {
        if (data && data.success) {
          this.visualEditorTemplateModel = data.model;
        }
      });
  }

  getTags() {
    this.getTagsSub$ = this.tagsService.getAvailableTags().subscribe((data) => {
      if (data && data.success) {
        this.availableTags = data.model;
      }
    });
  }

  initForm() {
    for (const language of applicationLanguages) {
      this.visualEditorTemplateModel = {
        ...this.visualEditorTemplateModel,
        translations: [
          ...this.visualEditorTemplateModel.translations,
          { id: language.id, description: '', name: '' },
        ],
      };
    }
  }

  createVisualTemplate() {
    this.createVisualTemplateSub$ = this.visualEditorService
      .createVisualEditorTemplate(this.visualEditorTemplateModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.router.navigate(['/cases/']).then();
        }
      });
  }

  updateVisualTemplate() {
    this.updateVisualTemplateSub$ = this.visualEditorService
      .updateVisualEditorTemplate(this.visualEditorTemplateModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.router.navigate(['/cases/']).then();
        }
      });
  }

  openTagsModal() {
    this.tagsModal.show();
  }

  onAddNewElement(number: number) {}

  toggleCollapse() {}

  dragulaPositionChanged($event: any[]) {}

  onEditorElementChanged($event: any) {}

  onDeleteElement($event: number) {}

  ngOnDestroy(): void {}
}
