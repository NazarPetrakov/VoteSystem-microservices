import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Poll, PollOption } from '../../_models/poll';
import { FormsModule } from '@angular/forms';
import { NgxSpinnerComponent } from "ngx-spinner";

@Component({
  selector: 'app-poll',
  imports: [DatePipe, FormsModule, NgxSpinnerComponent],
  templateUrl: './poll.component.html',
  styleUrl: './poll.component.css',
})
export class PollComponent {
  isDropped = false;
  @Input() id: string = '';
  @Input() title = '';
  @Input() description = '';
  @Input() createdTime!: Date;
  @Input() author!: number;
  @Input() options: PollOption[] = [];
  @Output() drop = new EventEmitter();

  dropEvent() {
    if (!this.isDropped) {
      this.drop.emit();
    }
    this.isDropped = !this.isDropped;
  }
}
