import React, { useState } from 'react';
import { Get } from '../lib/axios';
import { Select } from 'antd';
import { debounce } from 'lodash';
const { Option } = Select;

export interface IEntityPickerProps {
  onChange?: (value: any) => void;
  mapFun: (r: any) => { value: number; text: string };
  url: string;
  value?: any;
  mode?: 'default' | 'multiple';
  placeholder?: string;
  style?: any;
  size?: any;
}

export default ({ onChange, url, mapFun, value, mode = 'default', style, placeholder, size }: IEntityPickerProps) => {
  const [searchText, setSearchText] = useState('');
  const debouncedSetSearchText = debounce(setSearchText, 500);
  return (
    <Get url={url} params={{ searchText }}>
      {({ data }: any) => {
        let dataSource: any = [];
        if (data && data.result) {
          dataSource = data.result.results ? data.result.results.map(mapFun) : data.result.items.map(mapFun);
        }
        return (
          <Select
            showSearch
            size={size}
            style={{ ...style, boxShadow: '0px 3px 10px #0000000D' }}
            placeholder={placeholder}
            optionFilterProp="children"
            mode={mode}
            getPopupContainer={(trigger: any) => trigger.parentNode}
            onSearch={debouncedSetSearchText}
            onChange={onChange}
            value={value}
            allowClear
          >
            {dataSource.map((d: any) => (
              <Option key={d.value} value={d.value}>
                {d.text}
              </Option>
            ))}
          </Select>
        );
      }}
    </Get>
  );
};

export type IConnectedEntityPickerProps = {
  store: string;
  method: string;
  value?: any;
  onChange?: (value: any) => void;
};
