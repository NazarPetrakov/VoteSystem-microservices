import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FormInputComponent } from '../../_common/form-input/form-input.component';
import { AuthService } from '../../_services/auth.service';
import { UserRegisterRequest } from '../../_models/_contracts/userRegisterRequest';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  imports: [ReactiveFormsModule, FormInputComponent],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css',
})
export class SignUpComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  fb = inject(FormBuilder);

  registerForm = this.fb.group({
    userName: this.fb.control('', [
      Validators.required,
      Validators.minLength(4),
      Validators.maxLength(256),
    ]),
    email: this.fb.control('', [Validators.required, Validators.email]),
    password: this.fb.control('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(64),
    ]),
  });

  get userNameControl() {
    return this.registerForm.get('userName') as FormControl;
  }
  get passwordControl() {
    return this.registerForm.get('password') as FormControl;
  }
  get emailControl() {
    return this.registerForm.get('email') as FormControl;
  }

  register() {
    const registerUser = this.registerForm.value as UserRegisterRequest;
    this.authService.registerUser(registerUser).subscribe({
      complete: () => this.router.navigateByUrl(''),
    });
  }
}
