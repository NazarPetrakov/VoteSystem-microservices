import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { CreatePollRequest, Poll } from '../_models/poll';
import { delay, of, tap } from 'rxjs';
import { SKIP_LOADING } from '../_interceptors/loading-context';
import { Pagination } from '../_models/pagination';
import { PaginationQueryParams } from '../_models/_contracts/queryParams/paginationQueryParams';
import { setPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class PollService {
  private client = inject(HttpClient);
  private baseUrl = environment.pollBaseUrl;

  readonly polls = signal<Poll[]>([]);
  readonly pagination = signal<Pagination | undefined>(undefined);

  readonly createRefreshTrigger = signal(0);

  triggerRefresh() {
    this.createRefreshTrigger.update((v) => v + 1);
  }

  createPoll(createPollRequest: CreatePollRequest) {
    return this.client
      .post<Poll>(`${this.baseUrl}polls`, createPollRequest)
      .pipe(
        tap(() => {
          this.triggerRefresh();
        })
      );
  }
  loadPolls(params: PaginationQueryParams) {
    let httpParams = setPaginationHeaders(params.pageNumber, params.pageSize);

    return this.client
      .get<Poll[]>(`${this.baseUrl}polls`, {
        observe: 'response',
        params: httpParams,
      })
      .pipe(
        tap((response) => {
          // 1. Extract the Poll items from the body
          const polls = response.body || [];

          // 2. Extract the Pagination header
          const paginationHeader = response.headers.get('Pagination');

          if (paginationHeader) {
            // The header value is usually a JSON string, so parse it
            const pagination: Pagination = JSON.parse(paginationHeader);
            this.pagination.set(pagination);
          }

          // 3. Update the polls signal
          this.polls.set(polls);
        })
      );
  }
  loadOptions(id: string) {
    const currentPolls = this.polls();
    const existingPoll = currentPolls.find((p) => p.id === id);

    if (existingPoll?.pollOptions?.length) {
      return of();
    }

    return this.client
      .get<Poll>(`${this.baseUrl}polls/${id}`, {
        context: new HttpContext().set(SKIP_LOADING, true),
      })
      .pipe(
        //only for development
        delay(1000),

        tap((poll) => {
          this.polls.update((polls) => {
            const index = polls.findIndex((p) => p.id === poll.id);
            if (index !== -1) {
              const updated = [...polls];
              updated[index] = {
                ...updated[index],
                pollOptions: poll.pollOptions,
                userName: poll.userName,
              };
              return updated;
            }
            return polls;
          });
        })
      );
  }
}
