import './index.less';

import * as React from 'react';

import { Avatar, Col, Icon, Layout, Menu } from 'antd';
import { L, isGranted } from '../../lib/abpUtility';

import AbpLogo from '../../images/itsutra.png';
import { appRouters } from '../../components/Router/router.config';

const { Sider } = Layout;

const IconFont = Icon.createFromIconfontCN({
  scriptUrl: '//at.alicdn.com/t/font_2286956_e26vnp2hrt.js',
});

export interface ISiderMenuProps {
  path: any;
  collapsed: boolean;
  onCollapse: any;
  history: any;
  pathname:any;
}

const SiderMenu = (props: ISiderMenuProps) => {
  const { collapsed, history, onCollapse, pathname } = props;
  return (
    <Sider trigger={null} className={'sidebar'} width={256} collapsible collapsed={collapsed} onCollapse={onCollapse}>
      {collapsed ? (
        <Col style={{ textAlign: 'center', marginTop: 15, marginBottom: 10 }}>
          <Avatar shape="square" style={{ height: 27, width: 64 }} src={AbpLogo} />
        </Col>
      ) : (
        <Col style={{ textAlign: 'center', marginTop: 15, marginBottom: 10 }}>
          <Avatar shape="square" style={{ height: 54, width: 128 }} src={AbpLogo} />
        </Col>
      )}

      <Menu mode="inline" defaultSelectedKeys={[pathname]}>
        {appRouters
          .filter((item: any) => !item.isLayout && item.showInMenu)
          .map((route: any, index: number) => {
            if (route.permission && !isGranted(route.permission)) return null;

            return (
              <Menu.Item key={route.path} onClick={() => history.push(route.path)}>
                <IconFont type={route.icon} />
                <span>{L(route.title)}</span>
              </Menu.Item>
            );
          })}
      </Menu>
    </Sider>
  );
};

export default SiderMenu;
