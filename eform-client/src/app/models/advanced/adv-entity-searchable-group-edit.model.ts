import {AdvEntitySearchableItemModel} from './adv-entity-searchable-item.model';
export class AdvEntitySearchableGroupEditModel {
  name: string;
  advEntitySearchableItemModels: Array<AdvEntitySearchableItemModel> = [];
  groupUid: string;
}
