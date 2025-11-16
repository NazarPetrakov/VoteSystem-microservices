import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  OnInit,
  signal,
  untracked,
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
import { ToastrService } from 'ngx-toastr';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import {
  FormBuilder,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TopicGroups } from '../../_models/_contracts/topic/topic';
import { PollQueryParams } from '../../_models/_contracts/queryParams/pollQueryParams';

@Component({
  selector: 'app-poll-container',
  imports: [
    PollComponent,
    MatIconModule,
    MatPaginatorModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
  ],
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
  private toastr = inject(ToastrService);
  private fb = inject(FormBuilder);

  filtersForm = this.fb.group({
    active: this.fb.control(true),
    ended: this.fb.control(true),
    topic: this.fb.control(''),
    sort: this.fb.control('title'),
  });

  topicGroups = TopicGroups;

  get topicControl() {
    return this.filtersForm.get('topic') as FormControl;
  }

  isPollsLoaded = signal(false);
  isVotesLoaded = signal(false);

  polls = computed(() => this.pollService.polls() ?? []);
  userVotesMap = computed(() => this.voteService.votes() ?? []);

  queryParams = signal<PollQueryParams>({
    pageNumber: 1,
    pageSize: 10,
    orderBy: 'title',
    isClosed: false,
    isExpired: undefined,
    topic: undefined,
  });

  isDrawerOpen = signal(false);

  searchTerm: string = '';

  pagination = computed(() => this.pollService.pagination());

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
      this.pollService.createRefreshTrigger();

      this.resetParams();

      this.isPollsLoaded.set(false);
      untracked(() => {
        this.pollService
          .loadPolls(this.queryParams())
          .subscribe(() => this.isPollsLoaded.set(true));
      });
    });
  }

  ngOnInit(): void {
    this.pollService
      .loadPolls(this.queryParams())
      .subscribe(() => this.isPollsLoaded.set(true));

    if (this.userId()) {
      this.voteService
        .loadUserVotes(this.userId()!)
        .subscribe(() => this.isVotesLoaded.set(true));
    } else {
      this.isVotesLoaded.set(true);
    }
  }
  toggleDrawer() {
    this.isDrawerOpen.set(!this.isDrawerOpen());
  }
  onSearch() {
    console.log(this.searchTerm);

    if (this.queryParams().searchTerm !== this.searchTerm) {
      this.queryParams.update((params) => {
        return { ...params, searchTerm: this.searchTerm };
      });
      this.isPollsLoaded.set(false);

      const query = this.queryParams();

      this.pollService
        .loadPolls(query)
        .subscribe(() => this.isPollsLoaded.set(true));
    }
  }
  resetSearch() {
    if (this.searchTerm !== '') {
      this.searchTerm = '';
      this.onSearch();
    }
  }
  submitFilterForm() {
    const drawerCheckbox = document.getElementById(
      'my-drawer-1'
    ) as HTMLInputElement;
    if (drawerCheckbox) drawerCheckbox.checked = false;

    if (this.filtersForm.valid) {
      const formValue = this.filtersForm.value;
      console.log('Form values:', formValue);
      let isExpired: boolean | undefined;

      if (formValue.active && !formValue.ended) {
        isExpired = false;
      } else if (!formValue.active && formValue.ended) {
        isExpired = true;
      } else {
        isExpired = undefined;
      }

      this.queryParams.update((params) => {
        return {
          pageNumber: 1,
          pageSize: params.pageSize,
          orderBy: formValue.sort ?? undefined,
          isClosed: params.isClosed,
          isExpired: isExpired,
          topic: formValue.topic ?? undefined,
        };
      });

      this.isPollsLoaded.set(false);

      const query = this.queryParams();
      this.pollService
        .loadPolls(query)
        .subscribe(() => this.isPollsLoaded.set(true));
    }
  }
  handlePageEvent(e: PageEvent) {
    this.queryParams.update((params) => {
      return {
        pageNumber: e.pageIndex + 1,
        pageSize: e.pageSize,
        orderBy: params.orderBy,
        isClosed: params.isClosed,
        isExpired: params.isExpired,
        topic: params.topic,
      };
    });
    this.isPollsLoaded.set(false);

    const query = this.queryParams();
    this.pollService
      .loadPolls(query)
      .subscribe(() => this.isPollsLoaded.set(true));
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
    if (!userId) {
      this.toastr.error('Sign in to vote');
      return;
    }

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
  private resetParams() {
    this.queryParams.set({
      pageNumber: 1,
      pageSize: 10,
      orderBy: 'title',
      isClosed: false,
      isExpired: undefined,
    });
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
