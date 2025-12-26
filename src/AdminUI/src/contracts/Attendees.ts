import type { AttendeeStatus, MessageStatus } from "./Enums";
import type { BaseDto } from "./Common";

export interface AttendeeItemDto extends BaseDto {
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  statusId: AttendeeStatus;
  ticketNumber: string;
  ticketUrl: string;
  messages: AttendeeMessageDto[];
}

export interface AttendeeMessageDto extends BaseDto {
  providerCode: string;
  providerName: string;
  statusId: MessageStatus;
}
