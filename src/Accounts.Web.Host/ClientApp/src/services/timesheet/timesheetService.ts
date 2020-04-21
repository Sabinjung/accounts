import http from '../httpService';
import TimesheetModel from '../../models/Timesheet/timesheetModel';
import { PagedResultDto2 } from '../dto/pagedResultDto';
import { TimesheetListItem } from './dto/TimesheetListItem';
import { GetTimesheetsInput } from './dto/GetTimesheetsInput';
import {TimesheetQuery} from './dto/TimesheetQuery';

class TimesheetService {
  public async getUpcomingTimesheetInfo(projectId: number): Promise<TimesheetModel> {
    let response = await http.get('api/services/app/Timesheet/GetUpcomingTimesheetInfo', { params: { projectId } });
    return response.data.result;
  }

  public async createTimesheet(timesheet: TimesheetModel): Promise<boolean> {
    let response = await http.post('api/services/app/Timesheet/Create', timesheet);
    return response.data.result;
  }

  public async getAllTimesheets(input: GetTimesheetsInput): Promise<PagedResultDto2<TimesheetListItem>> {
    let response = await http.get('api/services/app/Timesheet/GetTimesheets', { params: input });
    return response.data.result || [];
  }

  public async getSavedQueries(): Promise<Array<TimesheetQuery>> {
    let response = await http.get('api/services/app/Timesheet/GetSavedQueries');
    return response.data.result || [];
  }
}

export default new TimesheetService();
