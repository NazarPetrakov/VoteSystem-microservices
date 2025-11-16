import { Component, inject } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { FormInputComponent } from '../../_common/form-input/form-input.component';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TopicGroups } from '../../_models/_contracts/topic/topic';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-create-poll-dialog',
  imports: [
    MatDialogModule,
    ReactiveFormsModule,
    FormInputComponent,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule,
  ],
  templateUrl: './create-poll-dialog.component.html',
  styleUrl: './create-poll-dialog.component.css',
})
export class CreatePollDialogComponent {
  private dialogRef = inject(MatDialogRef<CreatePollDialogComponent>);
  topicGroups = TopicGroups;
  fb = inject(FormBuilder);

  createPollForm = this.fb.group({
    title: ['', Validators.required],
    topic: [''],
    timeCount: [
      1,
      [Validators.required, Validators.min(1), Validators.max(1000)],
    ],
    timeUnit: ['', Validators.required],
    options: this.fb.array(
      [
        this.fb.control('', Validators.required),
        this.fb.control('', Validators.required),
      ],
      [Validators.required, Validators.minLength(2)]
    ),
  });
  get titleControl() {
    return this.createPollForm.get('title') as FormControl;
  }
  get topicControl() {
    return this.createPollForm.get('topic') as FormControl;
  }
  get timeCountControl() {
    return this.createPollForm.get('timeCount') as FormControl;
  }
  get timeUnitControl() {
    return this.createPollForm.get('timeUnit') as FormControl;
  }
  get formOptions() {
    return this.createPollForm.get('options') as FormArray;
  }
  get formValue() {
    return this.createPollForm.value;
  }
  addOption() {
    this.formOptions.push(this.fb.control('', Validators.required));
  }
  removeOption(index: number) {
    this.formOptions.removeAt(index);
  }
  onSubmit() {
    if (this.createPollForm.invalid) {
      this.createPollForm.markAllAsTouched();
      return;
    }

    const formValue = this.createPollForm.value;
    this.dialogRef.close(formValue);
  }
}
