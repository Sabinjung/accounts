import _ from 'lodash';
import { action, observable, runInAction } from 'mobx';
import { chain } from 'lodash';
import hourLogEntryService from '../services/hourLogEntries/hourLogEntriesService';
import { GetProjectHourLogsOutput } from '../services/hourLogEntries/dto/getProjectHourLogsOutput';
import { GetProjectHourLogsInput } from '../services/hourLogEntries/dto/getProjectHourLogsInput';
import HourLogEntryModel from '../models/Timesheet/hourLogEntryModel';

class HourLogEntryStore {
  @observable projectHourLogEntries!: Array<GetProjectHourLogsOutput>;
  @observable recordCount!: number;

  @action
  async getAll(args: GetProjectHourLogsInput) {
    let result: any = await hourLogEntryService.getProjectHourLogs(args);
    runInAction(() => {
      this.projectHourLogEntries = result[0];
      this.recordCount = result[1];
    });
  }

  @action
  async saveHourLogEntries(projectIds: Array<number>) {
    const hourLogEntries = chain(this.projectHourLogEntries)
      .map((x: GetProjectHourLogsOutput) => x.hourLogEntries)
      .flatten()
      .uniqBy(v => [v.day, v.projectId].join())
      .filter((x: HourLogEntryModel) =>  x.isModified && projectIds.includes(x.projectId))
      .value();

    let result = await hourLogEntryService.saveHourEntryLogs(hourLogEntries);
    hourLogEntries.forEach(l => (l.isModified = false));
    return result;
  }
}

export default HourLogEntryStore;
