import { Role } from "./Role";
import { UserProfile } from "./UserProfile";

export interface User {
    userId: number;                 // Maps to `UserId`
    username?: string;              // Maps to `Username`
    password?: string;              // Maps to `Password`
    
    email?: string;                 // Maps to `Email`-
    isApproved?: boolean;           // Maps to `IsApproved
    approveRejectReason?: string;   // Maps to `ApproveRejectReason`
    approvedRejectedDate?: Date;    // Maps to `ApprovedRejectedDate`, nullable Date
    approvedRejectedBy?: string;    // Maps to `ApprovedRejectedBy`

    isLocked?: boolean;             // Maps to `IsLocked`
    isDeleted?: boolean;            // Maps to `IsDeleted`
    createdAt: Date;                // Maps to `CreatedAt`
    updatedAt?: Date;               // Maps to `UpdatedAt`, nullable Date

    userProfile? : UserProfile;
    roles?: Role[];                 // Maps to `Roles`,
  }
  