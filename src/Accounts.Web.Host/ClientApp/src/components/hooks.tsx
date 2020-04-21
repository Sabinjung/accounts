import React, { useRef, useEffect } from 'react';
// import { observable, extendObservable } from 'mobx';
import { useLocalStore } from 'mobx-react-lite';
import _ from 'lodash';

// import http from '../services/httpService';

// // const rootStore = {
// //   get: function({ url, params }: any) {
// //     const result = observable({});
// //     http.get(url, { params }).then(response => {
// //       extendObservable(result, response.data);
// //     });
// //     return result;
// //   },
// // };

const storeContext = React.createContext<any>(null);

export const StoreProvider = ({ children, store: localStore }: any) => {
  const store = useLocalStore(localStore);
  return <storeContext.Provider value={store}>{_.isFunction(children) ? children({ store }) : children}</storeContext.Provider>;
};

export function useStore<TStore>(): TStore {
  const store = React.useContext<TStore>(storeContext);
  if (!store) {
    // this is especially useful in TypeScript so you don't need to be checking for null all the time
    throw new Error('useStore must be used within a StoreProvider.');
  }
  return store;
}

export function useLoad<TStore>(func: any) {
  const runCount = useRef(0);
  const store = useStore<TStore>();
  useEffect(() => {
    func(store);
  }, [runCount]);

  return store;
}
