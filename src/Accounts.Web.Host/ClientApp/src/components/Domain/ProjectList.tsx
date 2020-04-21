/** @jsx jsx */
import React, { useState } from 'react';
import { Badge, Button, Row, Col, List } from 'antd';
import moment from 'moment';
import _ from 'lodash';
import classNames from 'classnames';
import { jsx, ClassNames } from '@emotion/core';
import { Get } from '../../lib/axios';

export type IProjectListProps = {
  onSelectionChange?: (projectId: number) => void;
};

const projectListStyles = (theme: any) => ({
  '.ant-list-item': {
    padding: '5px 10px',
    color: 'black',
    '&:hover': {
      cursor: 'pointer',
      background: '#e8e8e8',
    },
    '&.active': {
      background: '#d9d9d9',
    },
    '.ant-list-item-meta-title,.ant-list-item-meta-description': {
      color: theme.colors.secondary,
    },
  },
});

export const ProjectList = ({ isLoading, predefinedQueries, onFilterChanged, dataSource, onSelectionChange, selectedProjectId }: any) => {
  return (
    <ClassNames>
      {({ css }) => (
        <List
          css={projectListStyles}
          size="small"
          loading={isLoading}
          header={
            <Row gutter={10} type="flex" justify="space-between">
              <Col>
                <Row gutter={10} type="flex">
                  {predefinedQueries.map((q: any) => (
                    <Col>
                      <Badge count={q.count}>
                        <Button type="primary" shape="round" size="small" onClick={() => onFilterChanged(q.name)}>
                          {q.name}
                        </Button>
                      </Badge>
                    </Col>
                  ))}
                </Row>
              </Col>
            </Row>
          }
          dataSource={dataSource}
          renderItem={(project: any) => (
            <List.Item
              onClick={() => {
                onSelectionChange && onSelectionChange(project.id);
              }}
              className={classNames({ active: selectedProjectId == project.id })}
            >
              <List.Item.Meta title={project.consultantName} description={project.companyName} />
              <div>
                <div style={{ textAlign: 'right' }}>
                  {moment(project.startDt).format('MM/DD')} - {project.endDt ? moment(project.endDt).format('MM/DD') : 'TBD'}
                </div>
                <div style={{ textAlign: 'right' }}>{project.totalHoursBilled} hrs</div>
                <div style={{ textAlign: 'right' }}>${project.totalAmountBilled} </div>
              </div>
            </List.Item>
          )}
        />
      )}
    </ClassNames>
  );
};

const ConnectedProjectList: React.FunctionComponent<IProjectListProps> = props => {
  const { onSelectionChange } = props;
  const [selectedFilter, setSelectedFilter] = useState('Active');

  return (
    <Get url="api/services/app/project/search" params={{ name: selectedFilter, isActive: true }}>
      {({ error, data, isLoading }: any) => {
        const result = (data && data.result) || { results: [], recordCounts: [] };
        const { results: dataSource, recordCounts: predefinedQueries } = result;

        return (
          <ProjectList
            dataSource={dataSource}
            predefinedQueries={predefinedQueries}
            onSelectionChange={onSelectionChange}
            isLoading={isLoading}
            onFilterChanged={(filter: any) => setSelectedFilter(filter)}
          />
        );
      }}
    </Get>
  );
};

export default ConnectedProjectList;
