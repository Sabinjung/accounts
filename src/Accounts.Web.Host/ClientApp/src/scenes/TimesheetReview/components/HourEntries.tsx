import React from 'react';
import { Table } from 'antd';
import moment from 'moment';
import HourLogEntryModel from '../../../models/Timesheet/hourLogEntryModel';

var enumerateDaysBetweenDates = function(startDate: any, endDate: any) {
  var dates = [];

  var currDate = moment(startDate)
    .subtract(1, 'day')
    .startOf('day');
  var lastDate = moment(endDate)
    .add(1, 'day')
    .startOf('day');

  while (currDate.add(1, 'days').diff(lastDate) < 0) {
    dates.push(currDate.clone().toDate());
  }
  return dates;
};

export type IHourLogEntriesProps = {
  hourLogEntries: Array<HourLogEntryModel>;
  startDt: Date;
  endDt: Date;
  totalHrs?: number;
};

export default ({ hourLogEntries = [], startDt, endDt, totalHrs }: IHourLogEntriesProps) => {
  var dates = enumerateDaysBetweenDates(moment(startDt), moment(endDt));
  const columns: any = dates.map((d: any) => ({
    title: moment(d).format('MM/DD'),
    width: 50,
    dataIndex: moment(d).format('MM/DD'),
    render: (hours: number) => {
      let color = 'black';
      if (hours > 8) color = 'red';
      if (hours < 5) color = 'red';
      return <span style={{ color }}>{hours}</span>;
    },
    className: moment(d).isoWeekday() === 6 || moment(d).isoWeekday() === 7 ? 'is-holiday' : '',

  }));
  columns.push({
    title: 'Hrs',
    dataIndex: 'totalHrs',
    className: 'column-sum',
    width: 50,
    fixed: 'right',
  });

  const dataSource = hourLogEntries.reduce(
    (p, l) => {
      p[moment(l.day).format('MM/DD')] = l.hours;
      return p;
    },
    { totalHrs }
  );

  return (
    <div>
      <Table dataSource={[dataSource]} columns={columns} size="small" pagination={false} className="hour-entries-table" scroll={{ x: 470 }} />
    </div>
  );
};