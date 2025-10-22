import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  effect,
  EventEmitter,
  inject,
  Input,
  OnChanges,
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

@Component({
  selector: 'app-poll',
  imports: [
    DatePipe,
    FormsModule,
    NgxSpinnerComponent,
    ReactiveFormsModule,
    MatIconModule,
    TopicPipe,
  ],
  templateUrl: './poll.component.html',
  styleUrl: './poll.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PollComponent implements OnChanges {
  @Input() pollId: string = '';
  @Input() title = '';
  @Input() topic = '';
  @Input() createdTime!: Date;
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['options'] || changes['votedOptionId']) {
      console.log('PollComponent: options or votedOptionId input changed');
      this.setVotedOption();
    }
  }
  private setVotedOption() {
    console.log('voted option id', this.votedOptionId);
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
}
