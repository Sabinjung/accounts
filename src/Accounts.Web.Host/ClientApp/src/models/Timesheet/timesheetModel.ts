import ProjectModel from './projectModel';
import HourLogEntryModel from './hourLogEntryModel';
import { assign } from 'lodash';
import { computed, observable } from 'mobx';
import moment from 'moment';
import AttachmentModel from './attachmentModel';
import ExpenseModel from './ExpenseModal';

class TimesheetModel {
  id!: number;
  projectId!: number;
  project!: ProjectModel;
  @observable startDt!: Date;
  @observable endDt!: Date;
  statusId!: number;
  createdDt!: Date;
  hourLogEntries: Array<HourLogEntryModel> = [];
  attachmentIds: Array<number> = [];
  attachments: Array<AttachmentModel> = [];
  @observable noteText!: string;
  @observable expenses: Array<ExpenseModel> = [];
  originalStartDt!: Date;
  originalEndDt!: Date;

  @computed get totalHrs() {
    return this.filteredHourLogEntries.reduce((totalHrs, h) => (totalHrs += h.hours), 0);
  }

  @computed get filteredHourLogEntries() {
    return this.hourLogEntries.filter((data: any) => moment(data.day).isBetween(this.startDt, this.endDt, 'day', '[]'));
  }

  constructor({ hourLogEntries, totalHrs, ...args }: any) {
    assign(this, args);
    let length: number = hourLogEntries.length;
    this.hourLogEntries = hourLogEntries.map((x: any) => new HourLogEntryModel(x));
    this.originalStartDt = this.startDt;
    this.originalEndDt = hourLogEntries[length - 1].day;
  }
}

export default TimesheetModel;
