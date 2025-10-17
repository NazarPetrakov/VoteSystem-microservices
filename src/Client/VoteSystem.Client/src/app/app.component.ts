import { Component, inject, OnInit } from '@angular/core';
import { NavigationComponent } from './navigation/navigation.component';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './_services/auth.service';
import { UserToken } from './_models/userToken';

@Component({
  selector: 'app-root',
  imports: [NavigationComponent, NgxSpinnerComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  private authService = inject(AuthService);
  private userJson = localStorage.getItem('user');

  ngOnInit(): void {
    if (this.userJson) {
      const user = JSON.parse(this.userJson) as UserToken;
      this.authService.currentUser.set(user);
    }
  }
}
