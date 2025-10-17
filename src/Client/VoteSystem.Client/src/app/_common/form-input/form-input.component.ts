import { Component, Input, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-form-input',
  imports: [ReactiveFormsModule],
  templateUrl: './form-input.component.html',
  styleUrl: './form-input.component.css',
})
export class FormInputComponent {
  @Input() label = '';
  @Input() control!: FormControl;
  @Input() id = '';
  @Input() minLength?: number;
  @Input() maxLength?: number;
  @Input() labelClass?: string = '';
  @Input() inputClass?: string = '';
  @Input() placeholder?: string = '';
}
