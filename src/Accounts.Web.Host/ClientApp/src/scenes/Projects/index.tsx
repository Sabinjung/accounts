import React, { useState, useEffect } from 'react';
import { Portal } from 'react-portal';
import { ProjectTable } from '../../components/Domain/ProjectTable';
import { Row, Col, Button, Card, Input, Badge } from 'antd';
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

const Search = Input.Search;

export default () => {
  const pageSize = 10;
  const [searchText, setSearchText] = useState('');
  const [skipCount, setSkipCount] = useState(1);
  const [queryName, setQueryName] = useState('Active');
  const [termFilterText, setTermFilterText] = useState(undefined);
  const [invoiceCycleId, setInvoiceCycleId] = useState(undefined);
  const [{ data, loading }, makeRequest] = useAxios({
    url: 'api/services/app/project/search',
    params: { isActive: true, name: queryName, TermId: termFilterText, InvoiceCyclesId: invoiceCycleId },
  });
  const result = (data && data.result) || { results: [], recordCounts: [], totalCount: 0 };
  const { results: dataSource, recordCounts: predefinedQueries, recordCount } = result;

  const history = useHistory();
  useEffect(() => {
    makeRequest({ params: { pageSize, pageNumber: skipCount, keyword: searchText, name: queryName, TermId: termFilterText, InvoiceCyclesId: invoiceCycleId } });
  }, [searchText, skipCount, queryName, termFilterText, invoiceCycleId]);

  const handleTermFilter = (value: any) => {
    setTermFilterText(value);
  }
  const handleInvoiceCycleFilter = (value: any) => {
    setInvoiceCycleId(value);
  }

  return (
    <StoreProvider
      store={() => ({
        consultants: [],
        newProject: {
          consultantId: null,
        },
        async getConsultants() {
          const response = await http.get('api/services/app/consultant/search');
          runInAction(() => {
            this.consultants = response && response.data && response.data.result.results;
          });
        },
      })}
    >
      {({ store }: any) => (
        <Card>
          <Row>
            <Col
              xs={{ span: 4, offset: 0 }}
              sm={{ span: 4, offset: 0 }}
              md={{ span: 4, offset: 0 }}
              lg={{ span: 2, offset: 0 }}
              xl={{ span: 2, offset: 0 }}
              xxl={{ span: 2, offset: 0 }}
            >
              <h2>{L('Projects')}</h2>
            </Col>
            <Col
              xs={{ span: 14, offset: 0 }}
              sm={{ span: 15, offset: 0 }}
              md={{ span: 15, offset: 0 }}
              lg={{ span: 1, offset: 21 }}
              xl={{ span: 1, offset: 21 }}
              xxl={{ span: 1, offset: 21 }}
            >
              <Button type="primary" shape="circle" icon="plus" onClick={() => history.push('/projects/new')}></Button>
            </Col>
          </Row>
          <Row>
            <Col sm={{ span: 10, offset: 0 }}>
              <Search placeholder={L('Filter')} onSearch={setSearchText} allowClear />
            </Col>
          </Row>
          <Row gutter={16} type="flex" style={{ margin: '25px 0px 15px 0px' }}>
            {predefinedQueries.map((q: any) => (
              <Col>
                <Badge count={q.count}>
                  <Button type="primary" shape="round" size="small" onClick={() => (setQueryName(q.name))}>
                    {q.name}
                  </Button>
                </Badge>
              </Col>
            ))}
            <Col lg={{span: 3, offset:  15}}>
              <EntityPicker
                url="api/services/app/Term/GetAll"
                mapFun={r => ({ value: r.id, text: `${r.name}` })}
                style={{ width: '180px' }}
                value={termFilterText}
                placeholder="Filter Term"
                onChange={handleTermFilter}
              />
            </Col>
            <Col lg={{span: 3}}>
              <EntityPicker
                url="api/services/app/InvoiceCycle/GetAll"
                mapFun={r => ({ value: r.id, text: `${r.name}` })}
                style={{ width: '180px' }}
                value={invoiceCycleId}
                placeholder="Filter Invoice Cycle"
                onChange={handleInvoiceCycleFilter}
              />
            </Col>
          </Row>
          <Row type="flex" >
            <Col style={{ overflowX: "auto" }}>
              <ProjectTable
                dataSource={dataSource}
                predefinedQueries={predefinedQueries}
                isLoading={loading}
                pagination={{ pageSize, total: data === undefined ? 0 : recordCount, defaultCurrent: 1 }}
                onChange={(pagination: any) => {
                  setSkipCount(pagination.current);
                }}
              />
            </Col>
          </Row>
          <Portal>
            <Authorize permissions={['Project.Create']}>
              <RouteableDrawer path={[`/projects/new`]} width={'30vw'} title="Project">
                {({ onClose }: any) => {
                  return (
                    <ProjectCreateUpdate
                      onClose={onClose}
                      project={store.newProject}
                      onProjectAdded={() => {
                        onClose();
                        makeRequest({});
                        store.newProject = {};
                      }}
                    />
                  );
                }}
              </RouteableDrawer>
            </Authorize>
            <Authorize permissions={['Project.Update']}>
              <RouteableDrawer path={['/projects/:projectId/edit']} width={'30vw'} title="Project" exact={true}>
                {({
                  match: {
                    params: { projectId },
                  },
                  onClose,
                }: any) => {
                  return (
                    <Get url="api/services/app/project/get" params={{ id: projectId }}>
                      {({ error, data, isLoading }: any) => {
                        return (
                          <ProjectCreateUpdate
                            onClose={onClose}
                            project={data && data.result}
                            onProjectAdded={() => {
                              onClose();
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
                      onConsultantAddedOrUpdated={consultant => {
                        onClose();
                        store.getConsultants();
                        store.newProject.consultantId = consultant.id;
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