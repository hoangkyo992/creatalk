class AppLocalization {
  setPrimevueLanguage(primevue: any, currentLanguage: string): any {
    if (!primevue.config.locale) return;
    if (currentLanguage == "vi-VN") {
      primevue.config.locale.dayNames = ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ 7"];
      primevue.config.locale.dayNamesShort = ["CN", "T2", "T3", "T4", "T5", "T6", "T7"];
      primevue.config.locale.dayNamesMin = ["CN", "T2", "T3", "T4", "T5", "T6", "T7"];
      primevue.config.locale.monthNamesShort = ["Th1", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7", "Th8", "Th9", "Th10", "Th11", "Th12"];
      primevue.config.locale.monthNames = [
        "Tháng một",
        "Tháng hai",
        "Tháng ba",
        "Tháng tư",
        "Tháng năm",
        "Tháng sáu",
        "Tháng bảy",
        "Tháng tám",
        "Tháng chín",
        "Tháng mười",
        "Tháng mười một",
        "Tháng mười hai"
      ];
    } else {
      primevue.config.locale.dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
      primevue.config.locale.dayNamesShort = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
      primevue.config.locale.dayNamesMin = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];
      primevue.config.locale.monthNamesShort = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
      primevue.config.locale.monthNames = [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
      ];
    }
  }
}

export default new AppLocalization();
