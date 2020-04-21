import { action, runInAction, observable } from 'mobx';
import timesheetService from '../services/timesheet/timesheetService';
import TimesheetModel from '../models/Timesheet/timesheetModel';
import { RootStore } from './storeInitializer';
import AttachmentModel from '../models/Timesheet/attachmentModel';
import { TimesheetListItem } from '../services/timesheet/dto/TimesheetListItem';
import { PagedResultDto2 } from '../services/dto/pagedResultDto';
import { GetTimesheetsInput} from '../services/timesheet/dto/GetTimesheetsInput';
import { TimesheetQuery } from '../services/timesheet/dto/TimesheetQuery';

class TimesheetStore {
  @observable upcomingTimesheet!: TimesheetModel | null;

  @observable timesheets!: PagedResultDto2<TimesheetListItem>;

  @observable savedQueries! : Array<TimesheetQuery>;

  constructor(private rootStore: RootStore) {
    this.createTimesheet = this.createTimesheet.bind(this);
  }

  
  @action
  public async getSavedQueries() {
    const result = await timesheetService.getSavedQueries();
    runInAction(() => {
      this.savedQueries = result;
    });
  }

  @action
  public async getTimesheets(input:GetTimesheetsInput) {
    const result = await timesheetService.getAllTimesheets(input);
    runInAction(() => {
      this.timesheets = result;
    });
  }

  @action
  public async getUpcomingTimesheetInfo(projectId: number) {
    const result = await timesheetService.getUpcomingTimesheetInfo(projectId);

    runInAction(() => {
      this.upcomingTimesheet = new TimesheetModel(result);
      this.upcomingTimesheet.expenses = [];  // Hacky Way
    });
  }

  @action
  public async createTimesheet() {
    const { attachments } = this.rootStore.projectStore;
    const { upcomingTimesheet: newTimesheet } = this;
    if (newTimesheet) {
      newTimesheet.attachmentIds = attachments.filter((a: AttachmentModel) => a.isSelected && a.projectId == newTimesheet.projectId).map(a => a.id);

      await timesheetService.createTimesheet(newTimesheet);
      runInAction(() => {
        this.upcomingTimesheet = null;
      });
    }
  }
}

export default TimesheetStore;
