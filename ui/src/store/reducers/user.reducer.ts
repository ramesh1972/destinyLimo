// // import the interface
import { createReducer, on } from '@ngrx/store';

import { UserState } from '../states/user.state';
import { approveRejectUser, approveRejectUser_Success, AuthenticateUser_Success, changePassword_Success, forgotPassword_Success, lockUser_Success, logoutUser, logoutUser_Success, registerUser_Success, resetPassword_Success, UsersFetchAPI_Success } from '../actions/user.action';

//create a dummy initial state
export const initialState: UserState = {
  loggedInUser: undefined,
  allUsers: [],
  newUser: undefined,
  currentUser: undefined
};


export const userReducer = createReducer(
  initialState,

  on(AuthenticateUser_Success, (state, { loggedInUser }) => {
    return { ...state, loggedInUser: loggedInUser };
  }),

  on(registerUser_Success, (state, { registeredUser }) => {
    return { ...state, newUser: registeredUser };
  }),

  on(resetPassword_Success, (state, { updatedUser }) => {
    return { ...state, currentUser: updatedUser };
  }),

  on(changePassword_Success, (state, { updatedUser }) => {
    return { ...state, currentUser: updatedUser };
  }),

  on(forgotPassword_Success, (state, { updatedUser }) => {
    return { ...state, currentUser: updatedUser };
  }),
  
  on(approveRejectUser_Success, (state, { updatedUser }) => {
    return { ...state, currentUser: updatedUser };
  }),

  on(lockUser_Success, (state, { updatedUser }) => {
    return { ...state, currentUser: updatedUser };
  }),
  
  on(logoutUser_Success, (state) => {
    return { ...state, loggedInUser: undefined };
  }),

  on(UsersFetchAPI_Success, (state, { allUsers }) => {
    return { ...state, allUsers: allUsers };
  }),






);