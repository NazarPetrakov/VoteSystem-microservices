import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Vote } from '../_models/vote';
import { tap } from 'rxjs';
import { CreateVoteRequest } from '../_models/_contracts/vote/createVoteRequest';

@Injectable({
  providedIn: 'root',
})
export class VoteService {
  private client = inject(HttpClient);
  private baseUrl = environment.voteBaseUrl;

  readonly votes = signal<Map<number, Vote[]> | null>(new Map());

  loadUserVotes(userId: number) {
    return this.client
      .get<Vote[]>(this.baseUrl + 'votes/by-user/' + userId)
      .pipe(
        tap((votes) => {
          this.votes.update((voteMap) => new Map(voteMap).set(userId, votes));
        })
      );
  }

  createVote(request: CreateVoteRequest) {
    return this.client.post<Vote>(this.baseUrl + 'votes', request).pipe(
      tap((vote) => {
        this.votes.update((votes) => {
          const result = new Map(votes);

          const userId = vote.userId;
          const userVotes = votes?.get(userId) ?? [];

          result.set(userId, [...userVotes, vote]);

          return result;
        });
      })
    );
  }
  deleteVote(voteToDeleteId: string, userId: number) {
    return this.client.delete(this.baseUrl + 'votes/' + voteToDeleteId).pipe(
      tap(() => {
        this.votes.update((votesMap) => {
          const result = new Map(votesMap);

          const userVotes = votesMap?.get(userId) ?? [];

          const newUserVotes = userVotes.filter(
            (v) => v.voteId !== voteToDeleteId
          );

          result.set(userId, newUserVotes);

          return result;
        });
      })
    );
  }
}
