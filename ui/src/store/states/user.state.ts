import { UserProfile } from '../models/UserProfile';

// state interface
export interface UserState {
    allUserProfiles: UserProfile[];
    userProfile?: UserProfile;
    newUserProfile?: UserProfile;
}