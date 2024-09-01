import { UserProfile } from '../models/UserProfile';

// state interface
export interface UserState {
    loggedInUser?: UserProfile;
    allUserProfiles: UserProfile[];
    userProfile?: UserProfile;
    newUserProfile?: UserProfile;
}