/* eslint-disable @typescript-eslint/no-unused-vars */
declare interface String {
  toDateString(): string;
}

String.prototype.toDateString = function (): string {
  return new Date(this?.toString()).toDateString();
};
