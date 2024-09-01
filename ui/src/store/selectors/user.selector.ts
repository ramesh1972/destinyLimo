import { createFeatureSelector, createSelector } from '@ngrx/store';

import { UserState } from '../states/user.state';
import { UserProfile } from '../models/UserProfile';

export const UserFeatureKey = 'users';

export const selectUserState = createFeatureSelector<UserState>(UserFeatureKey);

export const selectLoggedInUser = createSelector(
  selectUserState,
  (state: UserState) => state.loggedInUser
);

export const selectUserProfiles = createSelector(
  selectUserState,
  (state: UserState) => state.allUserProfiles
);

export const selectUserProfile = createSelector(
  selectUserState,
  (state: UserState) => state.userProfile
);

export const selectNewUserProfile = createSelector(
  selectUserState,
  (state: UserState) => state.newUserProfile
);

export const selectUserProfileById = (userId: number) => createSelector(
  selectUserState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.userId === userId)
);

export const selectUserProfileByUserName = (userName: string) => createSelector(
  selectUserState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.username === userName)
);

export const selectUserProfileByEmail = (email: string) => createSelector(
  selectUserState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.email === email)
);

export const selectUserProfileByPhone = (phone: string) => createSelector(
  selectUserState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.phoneNumber === phone)
);