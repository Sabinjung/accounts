import './AppLayout.less';

import * as React from 'react';

import { Redirect, Switch } from 'react-router-dom';

import DocumentTitle from 'react-document-title';
// import Footer from '../../components/Footer';
import Header from '../../components/Header';
import { Layout } from 'antd';
import ProtectedRoute from '../../components/Router/ProtectedRoute';
import SiderMenu from '../../components/SiderMenu';
import { appRouters } from '../Router/router.config';
import utils from '../../utils/utils';
import styled from '@emotion/styled';

const { Content } = Layout;

const StyledLayout = styled(Layout)`
  .ant-layout {
    background: #f4f6fc;
    .ant-layout-header {
      background: #f4f6fc !important;
      .header-container {
        box-shadow: none;
      }
    }
    .ant-layout-content {
      margin: 16px 25px !important;
      .ant-card {
        border-radius: 20px;
        background: #fff;
      }
      .ant-card-bordered {
        border: none;
      }
    }
  }

  .ant-layout-sider-children {
    background: #1c3faa;
    .ant-menu {
      background: #1c3faa;
      border: none;
      color: white;
      .ant-menu-item {
        width: 100%;
        padding-left: 60px !important;
        :hover {
          background: #3353b3;
          color: #fff;
        }
        ::after {
          border-radius: 5px 0px 0px 5px;
          border-right: 3px solid #fff;
        }
      }
      .ant-menu-item-selected {
        background: #3353b3;
        color: white;
      }
    }
    .ant-menu-inline-collapsed {
      .ant-menu-item {
        padding: 0 32px !important;
      }
    }
  }
`;

class AppLayout extends React.Component<any> {
  state = {
    collapsed: false,
  };

  toggle = () => {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  };

  onCollapse = (collapsed: any) => {
    this.setState({ collapsed });
  };

  render() {
    const {
      history,
      location: { pathname },
    } = this.props;

    const { path } = this.props.match;
    const { collapsed } = this.state;

    let initailPath = '/' + pathname.split('/')[1];

    const layout = (
      <StyledLayout style={{ height: '100vh' }}>
        <SiderMenu pathname={initailPath} path={path} onCollapse={this.onCollapse} history={history} collapsed={collapsed} />
        <Layout>
          <Layout.Header style={{ background: '#fff', minHeight: 52, padding: 0 }}>
            <Header collapsed={this.state.collapsed} toggle={this.toggle} />
          </Layout.Header>
          <Content style={{ margin: 16 }}>
            <Switch>
              {appRouters
                .filter((item: any) => !item.isLayout)
                .map((route: any, index: any) => (
                  <ProtectedRoute key={index} path={route.path} component={route.component} permission={route.permission} />
                ))}

              <Redirect from="/" to="/hourlogentry" />
            </Switch>
          </Content>
          {/* <Layout.Footer style={{ textAlign: 'center' }}>
            <Footer />
          </Layout.Footer> */}
        </Layout>
      </StyledLayout>
    );

    return <DocumentTitle title={utils.getPageTitle(pathname)}>{layout}</DocumentTitle>;
  }
}

export default AppLayout;
