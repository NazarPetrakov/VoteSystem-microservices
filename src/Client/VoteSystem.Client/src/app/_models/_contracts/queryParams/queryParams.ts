import { PaginationQueryParams } from './paginationQueryParams';

export class QueryParams extends PaginationQueryParams {
  orderBy?: string;
  searchTerm?: string;
}
