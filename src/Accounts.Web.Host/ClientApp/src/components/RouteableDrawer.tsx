import _ from 'lodash';
import React from 'react';
import { Drawer } from 'antd';
import { Route } from 'react-router';
import $ from 'jquery';

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
  const { title, exact, ...rest } = props;
  return (
    <Route path={props.path} exact={exact}>
      {para => {
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
            para.history.goBack();
          },
        };
        return <Drawer {...drawerProps}>{props.children({ ...para, onClose: drawerProps.onClose })}</Drawer>;
      }}
    </Route>
  );
};

export default RouteableDrawer;
