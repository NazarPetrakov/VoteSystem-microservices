import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private spinner = inject(NgxSpinnerService);

  loadingOn() {
    this.spinner.show('mainSpinner', {
      type: 'ball-fall',
      bdColor: 'rgba(255,255,255,0)',
      color: '#ffffff',
    });
  }

  loadingOff() {
    this.spinner.hide('mainSpinner');
  }
}
