export interface TimesheetListItem {
  id: number;
  project: {
    consultantName:string;
    companyName:string;
  };
  statusId: number;
  startDt: Date;
  endDt: Date;
  projectId: number;
  totalHrs: number;
  createdDt: Date;
}
