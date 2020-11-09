import * as React from 'react';
import { Button, Badge, Row, Col } from 'antd';

export type PredefinedQueryPillsProps = {
  style?: any;
  onClick?: (value: any) => void;
  dataSource?: any;
  type?: any;
  className?: any;
  size?: any;
  shape?: any;
  selectedFilter?: any;
};

const PredefinedQueryPills: React.SFC<PredefinedQueryPillsProps> = ({
  style,
  onClick,
  dataSource,
  className,
  type = 'primary',
  size = 'default',
  shape = 'round',
  selectedFilter,
}: any) => {
  return (
    <Row gutter={10} type="flex">
      {dataSource.map((d: any) => (
        <Col>
          <Badge count={d.count}>
            <Button
              className={selectedFilter === d.name ? 'highlight-tab' : ''}
              type={type}
              shape={shape}
              size={size}
              style={style}
              onClick={() => onClick(d.name)}
            >
              {d.name}
            </Button>
          </Badge>
        </Col>
      ))}
    </Row>
  );
};

export default PredefinedQueryPills;
