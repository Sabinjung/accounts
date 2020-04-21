import { action, observable } from 'mobx';
import { assign } from 'lodash';

class AttachmentModel {
  id!: number;
  contentType!: string;
  name!: string;
  originalName!: string;
  projectId!: number;
  @observable isSelected: boolean = false;
  constructor(args: any) {
    assign(this, args);
  }

  @action
  select(isSelected: boolean) {
    this.isSelected = isSelected;
  }
}

export default AttachmentModel;
