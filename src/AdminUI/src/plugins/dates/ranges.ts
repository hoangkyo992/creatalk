import { endOfMonth, endOfYear, startOfMonth, startOfYear, subMonths, startOfDay, endOfDay, addDays } from "date-fns";
import { PresetRange } from "../../contracts/Enums";

class DatePresetRange {
  ranges() {
    return [
      {
        id: "Today",
        label: "Common.DatePresetRanges.Today"
      },
      {
        id: "Next7Days",
        label: "Common.DatePresetRanges.Next7Days"
      },
      {
        id: "ThisMonth",
        label: "Common.DatePresetRanges.ThisMonth"
      },
      {
        id: "LastMonth",
        label: "Common.DatePresetRanges.LastMonth"
      },
      {
        id: "ThisYear",
        label: "Common.DatePresetRanges.ThisYear"
      }
    ];
  }
  getDates(range: PresetRange | string) {
    switch (range) {
      case PresetRange.Today:
        return [startOfDay(new Date()), endOfDay(new Date())];
      case PresetRange.Next7Days:
        return [startOfDay(new Date()), endOfDay(addDays(new Date(), 6))];
      case PresetRange.ThisMonth:
        return [startOfMonth(new Date()), endOfMonth(new Date())];
      case PresetRange.LastMonth:
        return [startOfMonth(subMonths(new Date(), 1)), endOfMonth(subMonths(new Date(), 1))];
      case PresetRange.ThisYear:
        return [startOfYear(new Date()), endOfYear(new Date())];
    }
    return [startOfDay(new Date()), endOfDay(new Date())];
  }
}

export default new DatePresetRange();
