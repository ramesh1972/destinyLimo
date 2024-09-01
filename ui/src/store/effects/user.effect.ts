import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { select, Store, Action } from '@ngrx/store';
import { catchError, map, mergeMap, switchMap, withLatestFrom } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

import { UserProfile } from '../models/UserProfile';
import { UserService } from '../apis/user.service';

import { invokeUserProfilesFetchAPI, UserProfilesFetchAPI_Success, invokeUserProfileFetchAPI, UserProfileFetchAPI_Success } from '../actions/user.action';

@Injectable()
export class UserEffect {
  constructor(
    private actions$: Actions,
    private userService: UserService,
    private store: Store
  ) { }

  loadAllUserProfiles$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeUserProfilesFetchAPI),
      mergeMap(() => {
        return this.userService
          .getUserProfiles()
          .pipe(map((data: UserProfile[]) => UserProfilesFetchAPI_Success({ allUserProfiles: data as UserProfile[] }))
          );
      }));
  });

  loadUserProfile$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeUserProfileFetchAPI),
      mergeMap((action) => {
        return this.userService
          .getUserProfile(action.userId)
          .pipe(map((data: UserProfile) => UserProfileFetchAPI_Success({ userProfile: data as UserProfile }))
          );
      }));
  });
}