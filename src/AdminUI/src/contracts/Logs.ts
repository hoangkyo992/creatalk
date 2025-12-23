export interface ActivityLogItemDto {
  action: string;
  time: Date;
  ipAddress: string;
  description: string;
  methodName: string;
  label: string;
  source: string;
  requestId: string;
  userId: string;
  username: string;
  logEntities: ActivityLogEntityDto[];
}

export interface ActivityLogEntityDto {
  crud: string;
  time: Date;
  entityName: string;
  id: string;
  newValue: string;
  oldValue: string;
  pk: number;
}
