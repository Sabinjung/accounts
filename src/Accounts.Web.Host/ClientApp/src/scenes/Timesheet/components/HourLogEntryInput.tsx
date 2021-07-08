/** @jsx jsx */
import _ from 'lodash';
import React from 'react';
import { Form, Input } from 'antd';
import { ContextMenuTrigger } from 'react-contextmenu';
import classNames from 'classnames';
import { observer } from 'mobx-react';
import { jsx, css } from '@emotion/core';
import moment from 'moment';
const EditableContext = React.createContext({});

function collect(props: any) {
  return { projectId: props.attributes['data-row-key'] };
}

export const EditableRow = ({ form, index, children, ...props }: any) => {
  const attributes = {
    ...props,
    projectId: props['data-row-key'],
  };
  // const handleClick = (e: any, data: any, target: any) => {};
  return (
    <EditableContext.Provider value={form}>
      <ContextMenuTrigger renderTag="tr" id="SIMPLE" holdToDisplay={500} attributes={attributes} key={props['data-row-key']} collect={collect}>
        {children}
      </ContextMenuTrigger>
    </EditableContext.Provider>
  );
};

export const HourLogEntryRow = observer(Form.create()(EditableRow));
@observer
export class HourLogEntryInput extends React.Component<any> {
  state = {
    editing: false,
    hours: 0,
  };

  input: any;
  form: any;

  toggleEdit = () => {
    const editing = !this.state.editing;
    this.setState({ editing }, () => {
      if (editing && this.input) {
        this.input.focus();
      }
    });
  };

  save = (e: any) => {
    const { record, handleSave, day, hourEntry, createHourLogEntry, projectId } = this.props;
    const fieldId = `${projectId}-${day}`;
    this.form.validateFields([fieldId], (error: any, values: any) => {
      if (error && error[e.currentTarget.id]) {
        return;
      }
      this.toggleEdit();
      const intVal = parseFloat(values[fieldId]);
      if (_.isNumber(intVal)) {
        const hours = _.isNaN(intVal) ? null : intVal;
        if (hourEntry) {
          hourEntry.update({ hours });
        } else {
          createHourLogEntry({
            projectId,
            day: day.format('MM/DD/YYYY'),
            hours,
          });
        }

        handleSave({ ...record });
      }
    });
  };

  renderCell = (form: any) => {
    this.form = form;
    const { hourEntry, projectId, day, projectStartDt, projectEndDt } = this.props;
    const { editing } = this.state;
    const cellContent = hourEntry && hourEntry.hours;
    const { getFieldError, isFieldTouched } = form;
    const fieldId = `${projectId}-${day}`;
    const isEntryError = isFieldTouched(fieldId) && getFieldError(fieldId);
    
    const handleHourChange = (e: any) => {
        let val: number = e.target.value === '' ? 0 : parseFloat(e.target.value);
        let strVal: string = e.target.value;
        if (val > 24 || (strVal.includes('.') && strVal.split('.')[1].length > 1)) {
          this.setState((prevState: any) => {
            this.form.setFieldsValue({ [fieldId]: prevState.hours.toString() });
            return { hours: prevState.hours };
          });
       } else {
          this.setState(() => {
            this.form.setFieldsValue({ [fieldId]: val.toString() });
            return { hours: val };
          });
        }
      };
    
      const handleCharacters = (e: any) => {
        const invalidChars = ['-', '+', 'e'];
        if (invalidChars.includes(e.key)) {
          e.preventDefault();
        }
      };

    const isEditable =
      editing &&
      (hourEntry ? !hourEntry.isAssociatedWithTimesheet : true) &&
      !moment(day).isBefore(projectStartDt) &&
      (projectEndDt ? !moment(day).isAfter(projectEndDt) : true) &&
      moment(day).isBefore(moment());

    return isEditable ? (
      <Form.Item
        css={css`
          margin: -1px -5px !important;
          border-spacing: 0px !important;
        `}
        validateStatus={isEntryError ? 'error' : ''}
        help=""
      >
        {form.getFieldDecorator(fieldId, {
          rules: [
            {
              validator(rule: any, value: any, callback: any) {
                callback();
              },
            },
          ],
          initialValue: cellContent,
        })(<Input ref={(node) => (this.input = node)} type="number" onKeyDown={handleCharacters} onPressEnter={this.save} onBlur={this.save} onChange={handleHourChange} maxLength={5} />)}
      </Form.Item>
    ) : (
      <div
        css={css`
          min-width: 1em;
          min-height: 1em;
        `}
        className={classNames('editable-cell-value-wrap')}
      >
        <Input
          css={css`
            border: none !important;
            background-color: inherit !important;
            &:focus {
              box-shadow: none !important;
            }
          `}
          value={cellContent}
          type="button"
          onClick={this.toggleEdit}
          onFocus={this.toggleEdit}
        />
      </div>
    );
  };

  render() {
    const {
      editable,
      dataIndex,
      title,
      record,
      index,
      day,
      handleSave,
      children,
      className,
      hourEntry,
      createHourLogEntry,
      projectId,
      projectStartDt,
      projectEndDt,
      upcomingTimesheetSummary,
      ...restProps
    } = this.props;
    const btw =
      upcomingTimesheetSummary &&
      (moment(day).isSame(upcomingTimesheetSummary.startDt) ||
        moment(day).isSame(upcomingTimesheetSummary.endDt) ||
        moment(day).isBetween(upcomingTimesheetSummary.startDt, upcomingTimesheetSummary.endDt));
    const isActive: boolean =
      upcomingTimesheetSummary && moment(upcomingTimesheetSummary.startDt) <= moment(upcomingTimesheetSummary.endDt) ? true : false;
    return (
      <td
        className={classNames(className, {
          'is-modified': hourEntry && hourEntry.isModified,
          'is-associated': hourEntry && hourEntry.isAssociatedWithTimesheet,
          'is-current-date': day && moment(day).format('MM/DD/YYYY') === moment().format('MM/DD/YYYY'),
          'is-upcoming-startDt': upcomingTimesheetSummary && isActive && moment(day).isSame(upcomingTimesheetSummary.startDt),
          'is-upcoming-endDt': upcomingTimesheetSummary && isActive && moment(day).isSame(upcomingTimesheetSummary.endDt),
          'is-upcoming-btw': upcomingTimesheetSummary && moment(day).isBetween(upcomingTimesheetSummary.startDt, upcomingTimesheetSummary.endDt),
          btw: btw,
          'is-disabled':
            dataIndex === 'hourLogEntries' &&
            (moment(day).isBefore(projectStartDt) || (projectEndDt && moment(day).isAfter(projectEndDt)) || moment(day).isAfter(moment())),
        })}
        {...restProps}
      >
        {editable ? <EditableContext.Consumer>{this.renderCell}</EditableContext.Consumer> : children}
        {btw && isActive && (
          <div
            className={
              Number.isInteger(upcomingTimesheetSummary.totalHrs) && upcomingTimesheetSummary.totalHrs < 100
                ? 'upcoming-timesheet-totalhrs'
                : 'upcoming-timesheet-totalhrs three-digits-totalhrs'
            }
          >
            {upcomingTimesheetSummary.totalHrs && parseFloat(upcomingTimesheetSummary.totalHrs.toFixed(1))} hrs
          </div>
        )}
      </td>
    );
  }
}
