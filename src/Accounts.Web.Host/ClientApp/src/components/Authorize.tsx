import { isGranted } from '../lib/abpUtility';

export type IAuthorizeProps = {
  permissions: Array<string>;
  children: any;
};

const Authorize = ({ permissions, children }: IAuthorizeProps) => {
  if (!permissions || permissions.length == 0) return children;
  if (permissions && permissions.some(p => isGranted(p))) return children;
  return null;
};

export default Authorize;
