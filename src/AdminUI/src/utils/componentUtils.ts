import { MessageStatus, UserStatus } from "@/contracts/Enums";

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
  getMessageStatusSeverity(statusId: MessageStatus) {
    switch (statusId) {
      case MessageStatus.New:
        return "info";
      case MessageStatus.Sending:
        return "primary";
      case MessageStatus.UserReceived:
        return "success";
      case MessageStatus.Succeeded:
        return "warning";
      case MessageStatus.Failed:
        return "danger";
    }
  }
}
export default new ComponentUtils();
