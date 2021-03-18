import React, { useState } from 'react';
import { Portal } from 'react-portal';
import { ProjectTable } from '../../components/Domain/ProjectTable';
import { Row, Col, Card } from 'antd';
import { useHistory } from 'react-router';
import useAxios from '../../lib/axios/useAxios';
import RouteableDrawer from '../../components/RouteableDrawer';
import ProjectCreateUpdate from '../../components/Domain/ProjectCreateUpdate';
import ConsultantCreateUpdate from '../../components/Domain/ConsultantCreateUpdate';
import EntityPicker from '../../components/EntityPicker';

import { StoreProvider } from '../../components/hooks';
import http from '../../services/httpService';
import { runInAction } from 'mobx';
import { L } from '../../lib/abpUtility';
import { Get } from '../../lib/axios';
import Authorize from '../../components/Authorize';
import CustomSearch from '../../components/Custom/CustomSearch';
import CustomButton from '../../components/Custom/CustomButton';
import PredefinedQueryPills from '../../components/PredefinedQueryPills';
import EndClientCreateUpdate from '../EndClients/components/EndClientCreateUpdate';

export default () => {
  const pageSize = 10;
  const [searchText, setSearchText] = useState('');
  const [skipCount, setSkipCount] = useState(1);
  const [queryName, setQueryName] = useState('Active');
  const [termFilterText, setTermFilterText] = useState(undefined);
  const [invoiceCycleId, setInvoiceCycleId] = useState(undefined);
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/project/search',
    params: {
      isActive: true,
      name: queryName,
      TermId: termFilterText,
      InvoiceCyclesId: invoiceCycleId,
      pageSize,
      pageNumber: skipCount,
      keyword: searchText,
    },
  });
  const result = (data && data.result) || { results: [], recordCounts: [], totalCount: 0 };
  const { results: dataSource, recordCounts: predefinedQueries, recordCount } = result;

  const history = useHistory();

  const handleTermFilter = (value: any) => {
    setTermFilterText(value);
  };
  const handleInvoiceCycleFilter = (value: any) => {
    setInvoiceCycleId(value);
  };

  return (
    <StoreProvider
      store={() => ({
        consultants: [],
        endClients: [],
        newProject: {
          consultantId: null,
          endClientId: null,
        },
        async getConsultants() {
          const response = await http.get('api/services/app/consultant/search');
          runInAction(() => {
            this.consultants = response && response.data && response.data.result.results;
          });
        },
        async getEndClients() {
          const response = await http.get('/api/services/app/EndClient/Search');
          runInAction(() => {
            this.endClients = response && response.data && response.data.result.results;
          });
        },
      })}
    >
      {({ store }: any) => (
        <Card>
          <Row type="flex" align="middle" justify="space-between">
            <Col>
              <h1>{L('PROJECTS')}</h1>
            </Col>
            <Col>
              <CustomButton type="primary" icon="plus" onClick={() => history.push('/projects/new')}>
                Add Project
              </CustomButton>
            </Col>
          </Row>
          <Row>
            <Col sm={{ span: 8, offset: 0 }}>
              <CustomSearch placeholder={L('Search')} onSearch={setSearchText} allowClear />
            </Col>
          </Row>
          <Row gutter={16} type="flex" justify="space-between" style={{ margin: '25px 0px 15px 0px' }} align="middle">
            <Col>
              <PredefinedQueryPills selectedFilter={queryName} size="small" dataSource={predefinedQueries} onClick={(name) => setQueryName(name)} />
            </Col>
            <Col>
              <EntityPicker
                url="api/services/app/Term/GetAll?MaxResultCount=25"
                size="large"
                mapFun={(r) => ({ value: r.id, text: `${r.name}` })}
                style={{ width: '180px', marginRight: '20px' }}
                value={termFilterText}
                placeholder="Filter Term"
                onChange={handleTermFilter}
              />
              <EntityPicker
                url="api/services/app/InvoiceCycle/GetAll"
                size="large"
                mapFun={(r) => ({ value: r.id, text: `${r.name}` })}
                style={{ width: '180px' }}
                value={invoiceCycleId}
                placeholder="Filter Invoice Cycle"
                onChange={handleInvoiceCycleFilter}
              />
            </Col>
          </Row>
          <ProjectTable
            dataSource={dataSource}
            predefinedQueries={predefinedQueries}
            isLoading={loading}
            pagination={{ pageSize, total: data === undefined ? 0 : recordCount, defaultCurrent: 1 }}
            onChange={(pagination: any) => {
              setSkipCount(pagination.current);
            }}
          />
          <Portal>
            <Authorize permissions={['Project.Create']}>
              <RouteableDrawer path={[`/projects/new`]} width={'30vw'} title="Add New Project" clearValues={() => (store.newProject = {})}>
                {({ onClose }: any) => {
                  return (
                    <ProjectCreateUpdate
                      onClose={() => {
                        onClose();
                      }}
                      project={store.newProject}
                      onProjectAdded={() => {
                        makeRequest({});
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Project.Update']}>
              <RouteableDrawer path={['/projects/:projectId/edit']} width={'30vw'} title="Project" clearValues={() => (store.newProject = {})}>
                {({
                  match: {
                    params: { projectId },
                  },
                  onClose,
                }: any) => {
                  return (
                    <Get url="api/services/app/project/get" params={{ id: projectId }}>
                      {({ error, data, isLoading }: any) => {
                        let result = data && data.result;
                        store.newProject = store.newProject.id ? store.newProject : { ...result };
                        return (
                          <ProjectCreateUpdate
                            onClose={() => {
                              onClose();
                            }}
                            project={store.newProject}
                            onProjectAdded={() => {
                              makeRequest({});
                            }}
                          />
                        );
                      }}
                    </Get>
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Consultant.Create']}>
              <RouteableDrawer path={[`/projects/new/consultants/new`]} width={'25vw'} title="Consultant">
                {({ onClose }: any) => {
                  return (
                    <ConsultantCreateUpdate
                      onClose={onClose}
                      onConsultantAddedOrUpdated={(consultant) => {
                        onClose();
                        store.getConsultants();
                        store.newProject.consultantId = consultant.id;
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Endclient.Create']}>
              <RouteableDrawer path={[`/projects/new/endClients/new`]} width={'25vw'} title="EndClient">
                {({ onClose }: any) => {
                  return (
                    <EndClientCreateUpdate
                      onClose={onClose}
                      onEndClientAddedOrUpdated={(endClient) => {
                        onClose();
                        store.getEndClients();
                        store.newProject.endClientId = endClient.id;
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Consultant.Create']}>
              <RouteableDrawer path={[`/projects/:projectId/edit/consultants/new`]} width={'25vw'} title="Consultant">
                {({ onClose }: any) => {
                  return (
                    <ConsultantCreateUpdate
                      onClose={onClose}
                      onConsultantAddedOrUpdated={(consultant) => {
                        onClose();
                        store.getConsultants();
                        store.newProject.consultantId = consultant.id;
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Endclient.Create']}>
              <RouteableDrawer path={[`/projects/:projectId/edit/endClients/new`]} width={'25vw'} title="EndClient">
                {({ onClose }: any) => {
                  return (
                    <EndClientCreateUpdate
                      onClose={onClose}
                      onEndClientAddedOrUpdated={(endClient) => {
                        onClose();
                        store.getEndClients();
                        store.newProject.endClientId = endClient.id;
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
          </Portal>
        </Card>
      )}
    </StoreProvider>
  );
};
