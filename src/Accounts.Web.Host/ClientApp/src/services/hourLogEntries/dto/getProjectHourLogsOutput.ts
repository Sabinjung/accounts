import ProjectModel from '../../../models/Timesheet/projectModel';
import HourLogEntryModel from '../../../models/Timesheet/hourLogEntryModel';


export interface GetProjectHourLogsOutput {
  project: ProjectModel;
  hourLogEntries: [HourLogEntryModel];

}
