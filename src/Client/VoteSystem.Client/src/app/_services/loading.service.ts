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
      bdColor: 'rgba(0,0,0,0)',
      color: 'rgb(6, 147, 255)',
    });
  }

  loadingOff() {
    this.spinner.hide('mainSpinner');
  }
}
