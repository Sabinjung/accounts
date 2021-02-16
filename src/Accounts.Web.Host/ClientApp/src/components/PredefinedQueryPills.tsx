import * as React from 'react';
import { Button, Badge, Row, Col } from 'antd';
import styled from '@emotion/styled';

const StyledCol = styled(Col)`
  .highlight-pill {
    background: #7034bd;
    color: #fff;
    :hover {
      background: #7034bd;
      color: #fff;
    }
  }
`;

const StyledButton = styled(Button)`
  background: #7034bd1a;
  color: #7034bd;
  border: none;
  height: 30px !important;
  padding: 0 21px !important;
  :hover {
    background: #7034bd;
    color: #fff;
  }
`;

const StyledFilter = styled(Col)`
  margin-left: auto;
  padding: 0px 27px !important;
`;

export type PredefinedQueryPillsProps = {
  style?: any;
  onClick?: (value: any) => void;
  dataSource?: any;
  type?: any;
  className?: any;
  size?: any;
  shape?: any;
  selectedFilter?: any;
  searchFilters?: any;
};

const PredefinedQueryPills: React.FC<PredefinedQueryPillsProps> = ({
  style,
  onClick,
  dataSource,
  className,
  size = 'default',
  shape = 'round',
  selectedFilter,
  searchFilters,
}: any) => {
  return (
    <Row gutter={[10, 10]} type="flex" align="middle">
      {dataSource.map((d: any) => (
        <StyledCol>
          <Badge count={d.count}>
            <StyledButton
              className={selectedFilter === d.name ? 'highlight-pill' : ''}
              shape={shape}
              size={size}
              style={style}
              onClick={() => onClick(d.name)}
            >
              {d.name}
            </StyledButton>
          </Badge>
        </StyledCol>
      ))}
      <StyledFilter>{searchFilters && searchFilters()}</StyledFilter>
    </Row>
  );
};

export default PredefinedQueryPills;
