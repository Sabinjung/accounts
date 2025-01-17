import React from 'react';
import { Input, Select } from 'antd';
import CustomInput from './../Custom/CustomInput';
import styled from '@emotion/styled';

const { Option } = Select;
const InputGroup = Input.Group;

const StyledSelect = styled(Select)`
  box-shadow: 0px 3px 10px #0000000d;
`;

export default class DiscountInput extends React.Component<any, any> {
  static getDerivedStateFromProps(nextProps: any) {
    // Should be a controlled component.
    if ('value' in nextProps) {
      return {
        ...(nextProps.value || {}),
      };
    }
    return null;
  }

  constructor(props: any) {
    super(props);

    const propVal = props.value || {};
    this.state = {
      discountValue: propVal.value || 0,
      discountType: propVal.discountType || 1,
    };
  }

  handleDiscountValueChange = (e: any) => {
    let discountValue = e.target.value;
    let rx = /^\d*\.?\d{0,2}$/;
    if (!rx.test(discountValue)) {
      return;
    }
    if (!('value' in this.props)) {
      this.setState({ discountValue });
    }
    this.triggerChange({ discountValue });
  };

  handleDiscountTypeChange = (discountType: any) => {
    if (!('value' in this.props)) {
      this.setState({ discountType });
    }
    this.triggerChange({ discountType });
  };

  triggerChange = (changedValue: any) => {
    // Should provide an event to pass value to Form.
    const { onChange } = this.props;
    if (onChange) {
      onChange({
        ...this.state,
        ...changedValue,
      });
    }
  };

  render() {
    const { discountType, discountValue } = this.state;
    return (
      <InputGroup compact>
        <StyledSelect value={discountType} onChange={this.handleDiscountTypeChange} getPopupContainer={(trigger: any) => trigger.parentNode} style={{ width: 100 }} allowClear>
          <Option value={1}>Percentage</Option>
          <Option value={2}>Value</Option>
        </StyledSelect>
        <CustomInput style={{ width: '50%' }} value={discountValue} onChange={this.handleDiscountValueChange} />
      </InputGroup>
    );
  }
}
