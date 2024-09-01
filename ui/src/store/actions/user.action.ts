import { createAction, props } from '@ngrx/store';

import { UserProfile } from '../models/UserProfile';

export const invokeAuthenticateUser = createAction(
  '[UserProfile API] Invoke Get Login User Fetch API',
  props<{ userName: string, password: string }>()
);

export const AuthenticateUser_Success = createAction(
  '[UserProfile API] Fetch API Success',
  props<{ loggedInUser: UserProfile }>()
);

export const logoutUser = createAction(
  '[UserProfile API] Logout User'
);

// fetch UserProfile
export const invokeUserProfilesFetchAPI = createAction(
  '[UserProfile API] Invoke UserProfiles Fetch API'
);

export const UserProfilesFetchAPI_Success = createAction(
  '[UserProfile API] Fetch API Success',
  props<{ allUserProfiles: UserProfile[] }>()
);

export const invokeUserProfileFetchAPI = createAction(
  '[UserProfile API] Invoke UserProfile Fetch API',
  props<{ userId: number }>()
);

export const UserProfileFetchAPI_Success = createAction(
  '[UserProfile API] Fetch API Success',
  props<{ userProfile: UserProfile }>()
);

export const invokeUserProfileUpdateAPI = createAction(
  '[UserProfile API] Invoke Update UserProfile API',
  props<{ userProfile: UserProfile }>()
);

export const UserProfileUpdateAPI_Success = createAction(
  '[UserProfile API] Update API Success',
  props<{ updatedUserProfile: UserProfile }>()
);

export const invokeUserProfileCreateAPI = createAction(
  '[UserProfile API] Invoke Create UserProfile API',
  props<{ userProfile: UserProfile }>()
);

export const UserProfileCreateAPI_Success = createAction(
  '[UserProfile API] Create API Success',
  props<{ newUserProfile: UserProfile }>()
);

export const invokeUserProfileDeleteAPI = createAction(
  '[UserProfile API] Invoke Delete UserProfile API',
  props<{ userId: number }>()
);
