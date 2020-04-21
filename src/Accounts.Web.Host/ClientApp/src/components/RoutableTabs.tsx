import React from 'react';
import { Tabs } from 'antd';
import { map, each } from 'lodash';
import { Route, Switch, RouteComponentProps } from 'react-router-dom';
import { TabsProps } from 'antd/lib/tabs';

const { TabPane } = Tabs;

export interface IRouteConfig {
  label: string;
  component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any>;
  getRoute: (url: string) => string;
}

export type RoutableTabsProps = {
  tabs?: TabsProps;
  routeConfig: Array<IRouteConfig>;
} & RouteComponentProps;

const RoutableTabs: React.FC<RoutableTabsProps> = ({ tabs, routeConfig, history, match }) => {
  const tabToRouteMap = {};
  const routeToTabsMap = {};

  const { url, path } = match;

  each(routeConfig, (configObj, routeKey) => {
    const routeURL = configObj.getRoute(url);
    tabToRouteMap[routeKey] = routeURL;
    routeToTabsMap[routeURL] = routeKey;
  });

  const defaultActiveKey = routeToTabsMap[history.location.pathname];
  const tabPaneNodes = map(routeConfig, (configObj, routeKey) => <TabPane tab={configObj.label} key={routeKey.toString()} />);
  const routeNodes = map(routeConfig, (configObj, routeKey) => (
    <Route path={configObj.getRoute(path)} exact key={routeKey} component={configObj.component} />
  ));
  const onTabChange = (activeKey: any) => {
    history.push(tabToRouteMap[activeKey]);
  };
  return (
    <>
      <Tabs {...tabs} onChange={onTabChange} defaultActiveKey={defaultActiveKey.toString()}>
        {tabPaneNodes}
      </Tabs>
      <Switch>{routeNodes}</Switch>
    </>
  );
};

export default RoutableTabs;
