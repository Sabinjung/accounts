import _ from 'lodash';
import React from 'react';
import { Drawer } from 'antd';
import { Route } from 'react-router';
import $ from 'jquery';
import styled from '@emotion/styled';

const StyledDrawer = styled(Drawer)`
  .ant-drawer-body {
    padding: 5px 24px;
  }
  .ant-drawer-header {
    padding: 28px 43px;
    border: none;
    .ant-drawer-title {
      font-size: 20px;
    }
    .ant-drawer-close {
      box-shadow: 1px 1px 6px #00000029;
      border-radius: 50%;
      top: 21px;
      right: 20px;
      width: 35px;
      height: 35px;
      line-height: 0px;
    }
  }
`;

function calculate() {
  const drawers = $('.ant-drawer-open .ant-drawer-content-wrapper');
  for (let i = 0; i < drawers.length; i++) {
    const width = drawers
      .slice(0, i)
      .toArray()
      .reduce((a, el) => {
        var currentElWidth = $(el).width() || 0;
        var calculatedWidth = currentElWidth + a;
        return calculatedWidth;
      }, 0);

    $(drawers[i]).css('right', width);
  }
}

const RouteableDrawer = (props: any) => {
  const { title, exact, clearValues, ...rest } = props;
  return (
    <Route path={props.path} exact={exact}>
      {(para) => {
        if (!para.match) return null;
        const drawerProps = {
          ...rest,
          afterVisibleChange: (visible: boolean) => {
            if (visible) {
              calculate();
            }
          },
          title: _.isFunction(props.title) ? props.title(para) : title,
          getContainer: false,
          style: { position: 'absolute' },
          mask: false,
          visible: true,
          closable: true,

          onClose: () => {
            clearValues && clearValues();
            para.history.goBack();
          },
        };
        return <StyledDrawer {...drawerProps}>{props.children({ ...para, onClose: drawerProps.onClose })}</StyledDrawer>;
      }}
    </Route>
  );
};

export default RouteableDrawer;
