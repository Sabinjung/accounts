export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface PagedResultDto2<T> {
  results: T[];
  currentPage: number;
  totalCount: number;
  pageCount: number;
  pageSize: number;
  recordCount: number;
  recordCounts: RecordCount[];
}

export interface RecordCount {
  name: string;
  count: number;
}
