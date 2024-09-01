import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { UserProfile }  from '../models/UserProfile';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private _http: HttpClient) { }

  getUserProfiles(): Observable<UserProfile[]> {
    console.log("User service :", environment.baseURL);
    return this._http.get<UserProfile[]>(environment.baseURL + "User/profile");
  }

  getUserProfile(userId: number): Observable<UserProfile> {
    console.log("User service :", environment.baseURL);
    return this._http.get<UserProfile>(environment.baseURL + "User/profile/" + userId);
  }

  authenticateUser(userName: string, password: string): Observable<UserProfile> {
    console.log("User service :", environment.baseURL, userName);
    return this._http.post<UserProfile>(environment.baseURL + "User/authenticate", { userName, password });
  }
}
