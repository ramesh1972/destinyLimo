import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { User }  from '../models/User';
import { UserProfile } from '../models/UserProfile';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private _http: HttpClient) { }

  // --------------------------------------------
  registerUser(user: User, userProfile: UserProfile): Observable<User> {
    console.log("User service :", environment.baseURL);

    const userRegisterJSON = {
      "user": user,
      "userProfile": userProfile
    };

    return this._http.post<User>(environment.baseURL + "User", userRegisterJSON);
  }

  // --------------------------------------------
  authenticateUser(userName: string, password: string): Observable<User> {
    console.log("User service :", environment.baseURL, userName);
    return this._http.post<User>(environment.baseURL + "User/authenticate", { userName, password });
  }

  logoutUser(): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.post<User>(environment.baseURL + "User/logout", {});
  }

  changePassword(userId: number, oldPassword: string, newPassword: string): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.put<User>(environment.baseURL + "User/password", { userId, oldPassword, newPassword });
  }

  forgotPassword(email: string): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.post<User>(environment.baseURL + "User/forgotPassword", { email });
  }

  resetPassword(userId: number, newPassword: string): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.put<User>(environment.baseURL + "User/resetPassword", { userId, newPassword });
  }

  // --------------------------------------------
  approveRejectUser(userId: number, isApproved: boolean, approveRejectReason: string, approvedRejectedBy: number): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.put<User>(environment.baseURL + "User/approveReject", { userId, isApproved, approveRejectReason, approvedRejectedBy });
  }

  lockUser(userId: number, isLocked: boolean): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.put<User>(environment.baseURL + "User/lock", { userId, isLocked });
  }

  // --------------------------------------------
  getUsers(): Observable<User[]> {
    console.log("User service :", environment.baseURL);
    return this._http.get<User[]>(environment.baseURL + "User");
  }

  updateUser(user: User): Observable<User> {
    console.log("User service :", environment.baseURL);
    return this._http.put<User>(environment.baseURL + "User", user);
  }
}
