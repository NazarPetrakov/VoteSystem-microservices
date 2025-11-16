import { QueryParams } from "./queryParams";

export class PollQueryParams extends QueryParams {
  topic?: string;
  isClosed?: boolean = false;
  isExpired?: boolean = false;
}
