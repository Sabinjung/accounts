import { RequestWrapper } from './components/Request';

export { default as AxiosProvider } from './AxiosProvider';
export { default as Request } from './components/Request';
export const Get = RequestWrapper('get');
export const Delete = RequestWrapper('delete');
export const Head = RequestWrapper('head');
export const Post = RequestWrapper('post');
export const Put = RequestWrapper('put');
export const Patch = RequestWrapper('patch');
