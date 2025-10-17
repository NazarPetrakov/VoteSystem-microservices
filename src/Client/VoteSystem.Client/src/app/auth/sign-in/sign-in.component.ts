import { JsonPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FormInputComponent } from '../../_common/form-input/form-input.component';
import { UserLoginRequest } from '../../_models/_contracts/userLoginRequest';
import { AuthService } from '../../_services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  imports: [ReactiveFormsModule, FormInputComponent],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css',
})
export class SignInComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  fb = inject(FormBuilder);

  loginForm = this.fb.group({
    userName: this.fb.control('', [
      Validators.required,
      Validators.minLength(4),
      Validators.maxLength(256),
    ]),
    password: this.fb.control('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(64),
    ]),
  });

  get userNameControl() {
    return this.loginForm.get('userName') as FormControl;
  }
  get passwordControl() {
    return this.loginForm.get('password') as FormControl;
  }

  login() {
    const loginUser = this.loginForm.value as UserLoginRequest;
    this.authService.loginUser(loginUser).subscribe({
      complete: () => this.router.navigateByUrl(''),
    });
  }
}
