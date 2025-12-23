import { useI18n } from "vue-i18n";
import { useConfirm } from "primevue/useconfirm";
import localStorageUtils from "../utils/localStorageUtils";
import { PresetRangeOptions, SortDirection } from "../contracts/Enums";
import type { DataSourceRequestDto, ColumnOrder, ColumnFilter } from "../contracts/Common";
import { startOfDay, endOfDay, subDays } from "date-fns";
import DatePresetRange from "@/plugins/dates/ranges";
import { ref } from "vue";

export function useAppDataTable(page: number = 1, pageSize: number = 20) {
  const confirm = useConfirm();
  const $t = useI18n().t;
  const dataSourceRequest = ref<DataSourceRequestDto>({ page: page, pageSize: pageSize });

  const getStoreItem = (key: string, filterTimeTypeOptions: Array<string>) => {
    const cache = localStorageUtils.getItem(key);
    const cacheData = JSON.parse(cache?.length > 0 ? cache : "{}");

    let presetRange = "";
    let startTime = startOfDay(subDays(new Date(), 7));
    let endTime = endOfDay(new Date());
    if (cacheData.presetRange != null && PresetRangeOptions.find((o) => o.value === cacheData.presetRange)) {
      presetRange = cacheData.presetRange;
      const range = DatePresetRange.getDates(presetRange);
      startTime = range[0];
      endTime = range[1];
    }

    let filterTimeType = filterTimeTypeOptions[0];
    if (cacheData.filterTimeType != null && filterTimeTypeOptions.find((o) => o === cacheData.filterTimeType)) {
      filterTimeType = cacheData.filterTimeType;
    }
    return {
      presetRange: presetRange,
      filterTimeType: filterTimeType,
      startTime: startTime,
      endTime: endTime
    };
  };
  const resetState = (key: string) => {
    confirm.require({
      message: $t("Common.Messages.ResetConfigWarning"),
      header: $t("Dialog.Title.Confirm"),
      acceptLabel: $t("Dialog.Button.Accept"),
      rejectLabel: $t("Dialog.Button.Reject"),
      acceptClass: "p-button-danger p-button-sm w-auto",
      rejectClass: "p-button-outlined p-button-sm w-auto",
      icon: "pi pi-question-circle",
      accept: () => {
        const keys = key.split(",");
        keys.forEach((k) => {
          localStorageUtils.removeItem(k);
        });
        location.reload();
      }
    });
  };

  const onRestore = ($event) => {
    setDataSourceQuery($event);
  };
  const onPageChanged = async ($event, callback: any) => {
    dataSourceRequest.value.page = $event.page + 1;
    dataSourceRequest.value.pageSize = $event.rows;
    if (callback) await callback(dataSourceRequest);
  };
  const onSort = async ($event, callback: any) => {
    if (setDataSourceQuery($event)) {
      if (callback) await callback(dataSourceRequest);
    }
  };
  const onFilter = async ($event, callback: any) => {
    if (setDataSourceQuery($event)) {
      if (callback) await callback(dataSourceRequest);
    }
  };
  const setDataSourceQuery = ($event) => {
    let sorts: ColumnOrder[] = [];
    if ($event.sortField != null && $event.sortField.length > 0) {
      let field = $event.sortField;
      field = field[0].toUpperCase() + field.substring(1);
      sorts = [{ k: field, d: $event.sortOrder == 1 ? SortDirection.ASC.toString() : SortDirection.DESC.toString(), i: 0 }];
    } else if ($event.multiSortMeta != null && $event.multiSortMeta.length > 0) {
      sorts = $event.multiSortMeta.map((s, i) => {
        let field = s.field;
        field = field[0].toUpperCase() + field.substring(1);
        return { k: field, d: s.order == 1 ? SortDirection.ASC.toString() : SortDirection.DESC.toString(), i: i };
      });
    }

    let changed = false;
    const sortString = JSON.stringify(sorts.sort((a, b) => (a.k > b.k ? 1 : b.k > a.k ? -1 : 0)));
    if (sortString !== dataSourceRequest.value.sort) changed = true;
    dataSourceRequest.value.sort = sortString;

    let filters: ColumnFilter[] = [];
    if ($event.filters != null && $event.filters != undefined) {
      filters = Object.entries($event.filters)
        .map((f) => {
          let key = f[0];
          key = key[0].toUpperCase() + key.substring(1);
          const value: any = f[1];
          let v = value.value;
          let matchMode = value.matchMode;
          let allowEmpty = false;
          if (value.matchMode == "In" && value.value != null && value.value.length > 0) {
            v = value.value.join(",");
          } else if (value.matchMode == "Contains" && hasValue(value.value) && stringInDoubleQuote(value.value)) {
            v = value.value.slice(1, -1);
            matchMode = "==";
            allowEmpty = true;
          }
          return { k: key, v: v, c: matchMode, allowEmpty: allowEmpty };
        })
        .filter((f) => f.allowEmpty || hasValue(f.v))
        .map((o) => {
          return { k: o.k, v: o.v, c: o.c };
        });
    }
    const filterString = JSON.stringify(filters.sort((a, b) => (a.k > b.k ? 1 : b.k > a.k ? -1 : 0)));
    if (filterString !== dataSourceRequest.value.filter) changed = true;
    dataSourceRequest.value.filter = filterString;
    if ($event.rows) dataSourceRequest.value.pageSize = $event.rows;
    if ($event.first || $event.first == 0) dataSourceRequest.value.page = Math.round($event.first / $event.rows) + 1;

    return changed;
  };
  const stringInDoubleQuote = (val) => {
    if (typeof val !== "string") return false;
    return val.startsWith('"') && val.endsWith('"');
  };
  const hasValue = (val) => {
    return val !== undefined && val !== null && val != "";
  };

  return {
    dataSourceRequest,
    onRestore,
    onSort,
    onFilter,
    getStoreItem,
    resetState,
    onPageChanged
  };
}
