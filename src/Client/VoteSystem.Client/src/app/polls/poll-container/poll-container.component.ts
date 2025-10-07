import { Component, computed, inject, OnInit } from '@angular/core';
import { PollComponent } from '../poll/poll.component';
import { PollService } from '../../_services/poll.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { delay, finalize } from 'rxjs';

@Component({
  selector: 'app-poll-container',
  imports: [PollComponent],
  templateUrl: './poll-container.component.html',
  styleUrl: './poll-container.component.css',
})
export class PollContainerComponent implements OnInit {
  private pollService = inject(PollService);
  private spinner = inject(NgxSpinnerService);
  polls = computed(() => this.pollService.polls());
  ngOnInit(): void {
    this.pollService.loadPolls().subscribe();
  }
  loadOptions(id: string) {
    this.spinner.show(`poll-${id}`, {
      type: 'ball-fall',
      bdColor: 'rgba(255,255,255,0.1)',
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
  sh() {
    this.spinner.show();
  }
}
