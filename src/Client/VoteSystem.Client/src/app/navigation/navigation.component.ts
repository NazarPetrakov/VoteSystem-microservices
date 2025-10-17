import { Component, inject, Input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { MatIconModule } from '@angular/material/icon';
@Component({
  selector: 'app-navigation',
  imports: [RouterLink, MatIconModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css',
})
export class NavigationComponent {
  authService = inject(AuthService);
  router = inject(Router);

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('sign-in');
  }
}
