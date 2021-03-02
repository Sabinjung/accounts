import { L } from '../../../lib/abpUtility';

const rules = {
  name: [{ required: true, message: L('Please input your role name!') }],
  displayName: [{ required: true, message: L('Please input your display name!') }]
};

export default rules;
