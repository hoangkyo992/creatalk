import { UserStatus } from "@/contracts/Enums";

class ComponentUtils {
  getUserStatusSeverity(statusId: UserStatus) {
    switch (statusId) {
      case UserStatus.PendingApproval:
        return "help";
      case UserStatus.Active:
        return "primary";
      case UserStatus.Locked:
        return "warning";
      case UserStatus.Disabled:
        return "secondary";
    }
  }
}
export default new ComponentUtils();
