// // import the interface
import { createReducer, on } from '@ngrx/store';

import { UserProfile } from '../models/UserProfile';
import { UserState } from '../states/user.state';
import { UserProfilesFetchAPI_Success, UserProfileCreateAPI_Success, UserProfileFetchAPI_Success, UserProfileUpdateAPI_Success } from '../actions/user.action';

//create a dummy initial state
export const initialState: UserState = {
  allUserProfiles: [],
  userProfile: undefined,
  newUserProfile: undefined
};

export const userReducer = createReducer(
  initialState,
  on(UserProfilesFetchAPI_Success, (state, { allUserProfiles }) => {
    return { ...state, allUserProfiles: allUserProfiles };
  }),

  on(UserProfileFetchAPI_Success, (state, { userProfile }) => {
    return { ...state, userProfile: userProfile };
  }),

  on(UserProfileUpdateAPI_Success, (state, { updatedUserProfile }) => {
    return { ...state, userProfile: updatedUserProfile };
  }),

  on(UserProfileCreateAPI_Success, (state, { newUserProfile }) => {
    return { ...state, newUserProfile: newUserProfile };
  }),
);