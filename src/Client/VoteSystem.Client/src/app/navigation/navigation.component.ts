import { Component, inject, Input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CreatePollDialogComponent } from '../polls/create-poll-dialog/create-poll-dialog.component';
import { PollService } from '../_services/poll.service';
@Component({
  selector: 'app-navigation',
  imports: [RouterLink, MatIconModule, MatButtonModule, MatDialogModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css',
})
export class NavigationComponent {
  private pollService = inject(PollService);
  readonly dialog = inject(MatDialog);
  authService = inject(AuthService);
  router = inject(Router);

  createPoll() {
    
  }

  openDialog() {
    const dialogRef = this.dialog.open(CreatePollDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      console.log('dialog result', result);
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('sign-in');
  }
}
