import { CommonModule, DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  effect,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  signal,
  SimpleChanges,
} from '@angular/core';
import { PollOption } from '../../_models/poll';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { MatIconModule } from '@angular/material/icon';
import { TopicPipe } from '../../_pipes/topic.pipe';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-poll',
  imports: [
    DatePipe,
    FormsModule,
    NgxSpinnerComponent,
    ReactiveFormsModule,
    MatIconModule,
    TopicPipe,
    MatIconModule,
    CommonModule,
    MatTooltipModule,
  ],
  templateUrl: './poll.component.html',
  styleUrl: './poll.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PollComponent implements OnChanges, OnDestroy, OnInit {
  @Input() pollId: string = '';
  @Input() title = '';
  @Input() topic = '';
  @Input() createdTime!: Date;
  @Input() endTime!: Date;
  @Input() isExpired!: boolean;
  @Input() isActive!: boolean;
  @Input() author!: string;
  @Input() options: PollOption[] = [];
  @Input() votedOptionId: string | null = null;
  @Output() drop = new EventEmitter();
  @Output() vote = new EventEmitter<PollOption>();
  @Output() cancel = new EventEmitter<PollOption>();

  fb = inject(FormBuilder);

  pollForm = this.fb.group({
    option: [null as PollOption | null, Validators.required],
  });

  isDropped = false;
  isVoted = signal(false);

  votedOption = signal<PollOption | null>(null);

  constructor() {
    effect(() => {
      if (this.isVoted()) {
        this.pollForm.disable();
        return;
      }
      this.pollForm.enable();
    });
  }

  timeLeftFormatted = signal('');
  private intervalId: any;

  ngOnInit(): void {
    this.updateTimeLeft();
    this.intervalId = setInterval(() => this.updateTimeLeft(), 60_000);
  }

  ngOnDestroy() {
    clearInterval(this.intervalId);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['options'] || changes['votedOptionId']) {
      this.setVotedOption();
    }
  }
  dropEvent() {
    if (!this.isDropped) {
      this.drop.emit();
    }
    this.isDropped = !this.isDropped;
  }
  voteEvent() {
    const selectedOption = this.pollForm.get('option')?.value;
    if (selectedOption) {
      this.vote.emit(selectedOption as PollOption);
    }
  }
  cancelVoteEvent() {
    const selectedOption = this.pollForm.get('option')?.value;
    if (selectedOption) {
      this.cancel.emit(selectedOption as PollOption);
    }
  }
  private updateTimeLeft() {
    const diff = this.endTime
      ? new Date(this.endTime).getTime() - Date.now()
      : 0;

    if (diff <= 0) {
      this.timeLeftFormatted.set('Poll ended');
      this.isActive = false;
      return;
    }

    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    let formatted = '';
    if (days > 0) formatted = `${days}d ${hours % 24}h ${minutes % 60}m`;
    else if (hours > 0) formatted = `${hours}h ${minutes % 60}m`;
    else if (minutes > 0) formatted = `${minutes}m`;
    else formatted = '<1m';

    this.timeLeftFormatted.set(formatted);
  }
  private setVotedOption() {
    console.log('setVotedOption');
    if (this.votedOptionId) {
      this.isVoted.set(true);
      if (this.options.length > 0) {
        const votedOption = this.options.find(
          (o) => o.id === this.votedOptionId
        );
        if (votedOption) {
          this.votedOption.set(votedOption);
          this.pollForm.patchValue({ option: votedOption });
        }
      }
    } else {
      this.isVoted.set(false);
      this.pollForm.patchValue({ option: null });
      this.votedOption.set(null);
    }
  }
}
