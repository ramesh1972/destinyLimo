import { createFeatureSelector, createSelector } from '@ngrx/store';

import { UserState } from '../states/user.state';
import { UserProfile } from '../models/UserProfile';

export const UserExamFeatureKey = 'users';

export const selectUserExamState = createFeatureSelector<UserState>(UserExamFeatureKey);

export const selectUserProfiles = createSelector(
  selectUserExamState,
  (state: UserState) => state.allUserProfiles
);

export const selectUserProfile = createSelector(
  selectUserExamState,
  (state: UserState) => state.userProfile
);

export const selectNewUserProfile = createSelector(
  selectUserExamState,
  (state: UserState) => state.newUserProfile
);

export const selectUserProfileById = (userId: number) => createSelector(
  selectUserExamState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.userId === userId)
);

export const selectUserProfileByUserName = (userName: string) => createSelector(
  selectUserExamState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.username === userName)
);

export const selectUserProfileByEmail = (email: string) => createSelector(
  selectUserExamState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.email === email)
);

export const selectUserProfileByPhone = (phone: string) => createSelector(
  selectUserExamState,
  (state: UserState) => state.allUserProfiles.find(userProfile => userProfile.phoneNumber === phone)
);