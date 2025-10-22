import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { CreatePollRequest, Poll } from '../_models/poll';
import { delay, of, tap } from 'rxjs';
import { SKIP_LOADING } from '../_interceptors/loading-context';

@Injectable({
  providedIn: 'root',
})
export class PollService {
  private client = inject(HttpClient);
  private baseUrl = environment.pollBaseUrl;

  readonly polls = signal<Poll[]>([]);

  createPoll(createPollRequest: CreatePollRequest) {
    return this.client
      .post<Poll>(`${this.baseUrl}polls`, createPollRequest)
      .pipe(
        tap((poll) => {
          this.polls.update((polls) => {
            return [...polls, poll];
          });
        })
      );
  }
  loadPolls() {
    return this.client.get<Poll[]>(`${this.baseUrl}polls`).pipe(
      tap((polls) => {
        if (this.polls().length === 0) {
          this.polls.set(polls);
        }
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
          console.log('Loaded poll options for', poll.id);
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
