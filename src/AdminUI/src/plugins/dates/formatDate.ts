import moment from "moment";

class DateFormat {
  formatDate(value: Date): any {
    if (value) {
      return moment(value).format("DD/MM/YYYY HH:mm:ss");
    }
  }
  formatDateOnly(value: Date): any {
    if (value) {
      return moment(value).format("DD/MM/YYYY");
    }
  }
  formatDateHM(value: Date): any {
    if (value) {
      return moment(value).format("DD/MM HH:mm");
    }
  }
  toDateOnly(value: Date): any {
    if (value) {
      return moment(value).format("YYYY-MM-DD");
    }
  }
}

export default new DateFormat();
