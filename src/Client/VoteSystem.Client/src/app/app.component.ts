import { Component } from '@angular/core';
import { NavigationComponent } from './navigation/navigation.component';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [NavigationComponent, NgxSpinnerComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'VoteSystem.Client';
}
