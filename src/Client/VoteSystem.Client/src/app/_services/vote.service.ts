import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class VoteService {
  private client = inject(HttpClient);
  private baseUrl = environment.voteBaseUrl;

  readonly votes = signal<[]>([]);

  createVote(){
    
  }

  constructor() {}
}
