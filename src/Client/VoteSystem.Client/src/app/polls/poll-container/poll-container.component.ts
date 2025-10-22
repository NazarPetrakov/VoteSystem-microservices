import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { PollComponent } from '../poll/poll.component';
import { PollService } from '../../_services/poll.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs';
import { VoteService } from '../../_services/vote.service';
import { AuthService } from '../../_services/auth.service';
import { MatIconModule } from '@angular/material/icon';
import { Vote } from '../../_models/vote';
import { PollOption } from '../../_models/poll';
import { CreateVoteRequest } from '../../_models/_contracts/vote/createVoteRequest';

@Component({
  selector: 'app-poll-container',
  imports: [PollComponent, MatIconModule],
  templateUrl: './poll-container.component.html',
  styleUrl: './poll-container.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PollContainerComponent implements OnInit {
  private pollService = inject(PollService);
  private voteService = inject(VoteService);
  private authService = inject(AuthService);

  private spinner = inject(NgxSpinnerService);
  private userId = computed(() => this.authService.currentUser()?.userId);

  isPollsLoaded = signal(false);
  isVotesLoaded = signal(false);

  polls = computed(() => this.pollService.polls() ?? []);
  userVotesMap = computed(() => this.voteService.votes() ?? []);

  pollVotesMap = computed(() => {
    const userId = this.userId();
    const votesMap = this.voteService.votes();
    if (!userId || !votesMap) return new Map<string, string | null>();

    const userVotes = votesMap.get(userId) || [];
    const result = new Map<string, string | null>();

    this.polls().forEach((poll) => {
      result.set(poll.id, null);
    });

    userVotes.forEach((vote) => {
      if (vote.pollId) {
        result.set(vote.pollId, vote.pollOptionId || null);
      }
    });

    return result;
  });

  constructor() {
    effect(() => {
      console.log('Votes changed:', this.userVotesMap());
    });
  }

  ngOnInit(): void {
    this.pollService.loadPolls().subscribe(() => this.isPollsLoaded.set(true));

    if (this.userId()) {
      this.voteService
        .loadUserVotes(this.userId()!)
        .subscribe(() => this.isVotesLoaded.set(true));
    }
  }

  loadUserVotes() {
    const userId = this.authService.currentUser()?.userId;

    if (this.authService.currentUser() && userId) {
      this.voteService.loadUserVotes(userId).subscribe();
    }
  }
  getVotedOptionId(pollId: string): string | null {
    return this.pollVotesMap().get(pollId) || null;
  }
  vote(option: PollOption) {
    const userId = this.userId();
    if (!userId) return;

    const votedOptionId = this.getVotedOptionId(option.pollId);

    if (votedOptionId) {
      console.log('You already voted in this poll');
      return;
    }
    const createVoteRequest: CreateVoteRequest = {
      userId: userId,
      pollId: option.pollId,
      pollOptionId: option.id,
    };
    this.voteService
      .createVote(createVoteRequest)
      .subscribe(() => console.log('User voted for:', option));
  }
  loadOptions(id: string) {
    this.spinner.show(`poll-${id}`, {
      type: 'ball-fall',
      bdColor: 'rgba(0,0,0,0.1)',
      color: 'blue',
      fullScreen: false,
      size: 'default',
    });
    this.pollService
      .loadOptions(id)
      ?.pipe(
        finalize(() => {
          this.spinner.hide(`poll-${id}`);
        })
      )
      .subscribe();
  }
  cancelVote(optionToDelete: PollOption) {
    const userId = this.userId();
    if (!userId) return;

    const vote = this.getVote(
      userId,
      (v) => v.pollOptionId === optionToDelete.id
    );
    if (!vote) return;

    this.voteService
      .deleteVote(vote.voteId, userId)
      .subscribe(() => console.log('successfully deleted'));
  }
  private getVote(
    userId: number,
    predicate: (vote: Vote) => boolean
  ): Vote | null {
    const voteMap = this.voteService.votes();
    const userVotes = voteMap?.get(userId);
    const vote = userVotes?.find(predicate);

    return vote ?? null;
  }
}
