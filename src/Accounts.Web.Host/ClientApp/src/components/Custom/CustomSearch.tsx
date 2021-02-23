import * as React from 'react';
import { Input } from 'antd';
import { SearchProps } from 'antd/es/input';
import styled from '@emotion/styled';
const Search = Input.Search;

const StyledSearch = styled(Search)`
  .ant-input {
    padding: 18px 15px;
    box-shadow: 0px 3px 10px #0000000D;
    border: 1px solid #CCCCCC;
    border-radius: 4px;
  }
`;

type CustomSearchProps = SearchProps;

const CustomSearch: React.FC<CustomSearchProps> = (props) => {
  return <StyledSearch {...props} allowClear/>;
};

export default CustomSearch;
