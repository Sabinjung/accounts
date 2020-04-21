import React from 'react';
import { ContextMenu, MenuItem, connectMenu } from 'react-contextmenu';
import Authorize from '../../../components/Authorize';

const ProjectContextMenu = (props: any) => {
  const { id, trigger, handleClick, projectId } = props;
  return (
    <ContextMenu id={id}>
      {trigger && (
        <Authorize permissions={['Timesheet.LogHour']}>
          <MenuItem onClick={handleClick} data={{ item: 1, projectId }}>
            Save Entries
          </MenuItem>
        </Authorize>
      )}
      {trigger && (
        <Authorize permissions={['Timesheet.LogHour']}>
          <MenuItem onClick={handleClick} data={{ item: 2, projectId }}>
            Upload Attachments
          </MenuItem>
        </Authorize>
      )}
      {trigger && (
        <Authorize permissions={['Timesheet.Create']}>
          <MenuItem onClick={handleClick} data={{ item: 3, projectId }}>
            Create Timesheet
          </MenuItem>
        </Authorize>
      )}
      {trigger && (
        <MenuItem onClick={handleClick} data={{ item: 4, projectId }}>
          Show Timesheets
        </MenuItem>
      )}
    </ContextMenu>
  );
};

export default connectMenu('SIMPLE')(ProjectContextMenu);
