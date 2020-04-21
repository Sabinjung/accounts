import React from 'react';
import { Select, Button, Input } from 'antd';
import { useLoad } from './hooks';
import { observer } from 'mobx-react-lite';
import { useHistory } from 'react-router';

const { Option } = Select;
const InputGroup = Input.Group;

const ConnectedEntityPicker = observer(function <TStore>({ loader, selector, value, onChange, mapFunc, addUrl }: any) {
  const history = useHistory();
  const store = useLoad<TStore>(loader);
  const dataSource = selector(store);
  return (
    <InputGroup compact style={{ display: 'flex' }}>
      <Select showSearch optionFilterProp="children" onChange={onChange} value={value} style={{ flex: 1 }} defaultValue={value} allowClear>
        {dataSource.map(mapFunc).map((d: any) => (
          <Option value={d.value} key={d.value}>
            {d.text}
          </Option>
        ))}
      </Select>
      <Button icon="plus" onClick={() => history.push(addUrl)} />
    </InputGroup>
  );
});

export default ConnectedEntityPicker;
