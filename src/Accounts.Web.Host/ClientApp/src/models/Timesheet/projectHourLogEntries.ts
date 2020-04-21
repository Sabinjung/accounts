import ProjectModel from './projectModel';
import HourLogEntryModel from './hourLogEntryModel';
import { observable, computed, action } from 'mobx';
import _ from 'lodash';
class ProjectHourLogEntries {
  @observable project: ProjectModel;
  @observable hourLogEntries: [HourLogEntryModel];

  constructor({ project, hourLogEntries }: any) {
    this.project = project;
    this.hourLogEntries = hourLogEntries.map((x: any) => new HourLogEntryModel(x));
    this.createHourLogEntry = this.createHourLogEntry.bind(this);
  }

  @computed get totalHrs() {
    return this.hourLogEntries.reduce((totalHrs, h) => {
      if (_.isFinite(h.hours)) {
        totalHrs += h.hours;
        return totalHrs;
      } else {
        return totalHrs;
      }
    }, 0);
  }

  @action
  async createHourLogEntry(args: any) {
    const hourLogEntry = new HourLogEntryModel(args);
    hourLogEntry.isModified = true;

    const existingHourLogEntry = _.find(this.hourLogEntries, h => h.projectId == args.projectId && h.day === args.day);
    if (!existingHourLogEntry) {
      this.hourLogEntries.push(hourLogEntry);
    }
  }
}

export default ProjectHourLogEntries;
