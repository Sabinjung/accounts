import * as React from 'react';
import { Table } from 'antd';
import styled from '@emotion/styled';
import { TableProps } from 'antd/es/table';

type CustomHoursTableProps = TableProps<any>;

const StyledTable = styled(Table)`
  .ant-table-small {
    border: none;
    table {
      border-spacing: 0 10px !important;
    }
    .ant-table-scroll {
      .ant-table-tbody > tr > td {
        border-bottom: none;
        border-right: 1px solid #f1f5f8;
      }
    }
    .ant-table-fixed-left {
      .ant-table-row {
        border-radius: 10px 0px 0px 10px !important;
      }
      .ant-table-tbody > tr > td {
        border: none;
      }
    }
    .ant-table-fixed-right {
      .ant-table-row {
        border-radius: 0px 10px 10px 0px;
      }
      .ant-table-tbody > tr > td {
        border: none;
      }
    }
    .ant-table-fixed {
      .ant-table-thead {
        tr {
          height: 0px !important;
          .ant-table-column-sorter {
            display: flex;
            .ant-table-column-sorter-inner {
              margin-top: 1px;
            }
          }
        }
      }
    }
    .ant-table-thead > tr > th {
      border: none;
      background: none;
    }

    .ant-table-row {
      box-shadow: 1px 1px 10px #1c3faa1a !important;
    }
  }

  .ant-pagination-item,
  .ant-pagination-item-link {
    border-radius: 50% !important;
  }
  .ant-pagination-item-active {
    border: none;
    background: #2680eb;
    a {
      color: white !important;
    }
  }

  .ant-checkbox-inner {
    border-spacing: 0px !important;
  }
  .ant-table-scroll table .ant-table-fixed-columns-in-body:not([colspan]){
    background: white !important;
  }
`;

const CustomHoursTable: React.FC<CustomHoursTableProps> = (props: any) => {
  return <StyledTable {...props} />;
};

export default CustomHoursTable;
