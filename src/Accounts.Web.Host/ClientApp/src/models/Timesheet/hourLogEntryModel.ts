import { action, observable } from 'mobx';
import { assign } from 'lodash';

class HourLogEntryModel {
  projectId!: number;
  @observable day!: string;
  @observable hours!: number;
  isAssociatedWithTimesheet!: boolean;
  id!: number;
  @observable isModified: boolean = false;

  constructor(args: any) {
    assign(this, args);
  }

  @action
  update(args: any) {
    assign(this, args, { isModified: true });
  }
}

export default HourLogEntryModel;
