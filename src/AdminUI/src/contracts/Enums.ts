export enum SortDirection {
  ASC = "ASC",
  DESC = "DESC"
}

export enum ValueComparison {
  Equal = "==",
  NotEqual = "!=",
  GreaterThan = ">",
  GreaterThanOrEqual = ">=",
  LessThan = "<",
  LessThanOrEqual = "<=",
  On = "On",
  Contains = "Contains",
  StartsWith = "StartsWith",
  EndsWith = "EndsWith",
  In = "In"
}

export enum FilterTimeType {
  CreatedTime = "CreatedTime"
}

export const FilterTimeTypeOptions: {
  text: string;
  value: string;
}[] = Object.entries(FilterTimeType).map(([text, value]) => ({
  text: `FilterTimeType.${text}`,
  value: value
}));

export enum PresetRange {
  Today = "Today",
  Next7Days = "Next7Days",
  ThisMonth = "ThisMonth",
  LastMonth = "LastMonth",
  ThisYear = "ThisYear"
}

export const PresetRangeOptions: {
  text: string;
  value: string;
}[] = Object.entries(PresetRange).map(([text, value]) => ({
  text: `PresetRange.${text}`,
  value: value
}));

export enum UserStatus {
  PendingApproval = "PendingApproval",
  Active = "Active",
  Locked = "Locked",
  Disabled = "Disabled"
}

export const UserStatusOptions: {
  text: string;
  value: string;
}[] = Object.entries(UserStatus).map(([text, value]) => ({
  text: `UserStatus.${text}`,
  value: value
}));

export enum LogLabel {
  CreateUser = 1,
  UpdateUser = 2,
  DeleteUser = 3,
  ResetUserPassword = 4,
  CreateUserRole = 6,
  UpdateUserRole = 7,
  DeleteUserRole = 8,
  UpdateUserRoleFeatures = 9,
  CreateSetting,
  UpdateSetting,
  DeleteSetting,
  CreateAlbum,
  UpdateAlbum,
  DeleteAlbum,
  CreateAttendee,
  UpdateAttendee,
  DeleteAttendee,
  CancelAttendee,
  RestoreAttendee,
  CreateMessageProvider,
  UpdateMessageProvider,
  DeleteMessageProvider
}

export const LogLabelOptions: {
  text: string;
  value: string;
  valueInt: number;
}[] = Object.entries(LogLabel)
  .filter((e) => {
    return !isNaN(parseInt(e[0]));
  })
  .map(([value, text]) => {
    return {
      text: `LogLabel.${text}`,
      value: value,
      valueInt: parseInt(value)
    };
  });

export enum FolderStatus {
  Active = "Active",
  Locked = "Locked",
  Archived = "Archived"
}

export const FolderStatusOptions: {
  text: string;
  value: string;
}[] = Object.entries(FolderStatus).map(([text, value]) => ({
  text: `FolderStatus.${text}`,
  value: value
}));

export enum FileStatus {
  Active = "Active",
  Locked = "Locked",
  Archived = "Archived"
}

export const FileStatusOptions: {
  text: string;
  value: string;
}[] = Object.entries(FileStatus).map(([text, value]) => ({
  text: `FileStatus.${text}`,
  value: value
}));

export enum FileType {
  None = "None",
  Image = "Image",
  Audio = "Audio",
  Video = "Video",
  Document = "Document"
}

export enum AttendeeStatus {
  Cancelled = "Cancelled",
  Default = "Default"
}

export const AttendeeStatusOptions: {
  text: string;
  value: string;
}[] = Object.entries(AttendeeStatus).map(([text, value]) => ({
  text: `AttendeeStatus.${text}`,
  value: value
}));

export enum MessageStatus {
  New = "New",
  Sending = "Sending",
  Succeeded = "Succeeded",
  Failed = "Failed",
  UserReceived = "UserReceived"
}

export const MessageStatusOptions: {
  text: string;
  value: string;
}[] = Object.entries(MessageStatus).map(([text, value]) => ({
  text: `MessageStatus.${text}`,
  value: value
}));
