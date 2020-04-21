import http from '../httpService';
import { GetProjectHourLogsInput } from './dto/getProjectHourLogsInput';
import HourLogEntryModel from '../../models/Timesheet/hourLogEntryModel';
import ProjectHourLogEntries from '../../models/Timesheet/projectHourLogEntries';

class HourLogEntryService {
  public async getProjectHourLogs(input: GetProjectHourLogsInput): Promise<Array<ProjectHourLogEntries>> {
    let result = await http.get('api/services/app/HourLogEntry/GetProjectHourLogs', { params: input });
    return (result.data && result.data.result && result.data.result.map((x: any) => new ProjectHourLogEntries(x))) || [];
  }

  public async saveHourEntryLogs(logs: Array<HourLogEntryModel>): Promise<boolean> {
    let result = await http.post('api/services/app/HourLogEntry/AddUpdateHourLogs', logs);
    return result.data.result;
  }
}

export default new HourLogEntryService();
