<template>
  <div>
    <InputText
      v-if="showInput"
      v-model="text"
      :placeholder="placeholder"
      :style="showTime ? 'min-width: 290px' : 'min-width: 100px'"
      :class="{
        'p-invalid': invalid,
        'w-auto': width === 'auto',
        'w-full': width === 'full'
      }"
      name="text"
      :size="size == 'small' ? 'small' : undefined"
      type="input"
      autocomplete="off"
      @click="open"
    />
    <Button
      v-else
      :icon="buttonIcon"
      :size="size == 'small' ? 'small' : undefined"
      outlined
      :label="showText ? text : ''"
      :class="{
        'p-invalid': invalid,
        'w-auto': width === 'auto',
        'w-full': width === 'full'
      }"
      @click="open"
    ></Button>
    <Popover ref="overlayPanel" class="flex" unstyled @hide="reset">
      <div class="flex align-content-center flex-wrap date-range-panel-body">
        <div class="flex flex-1 flex-wrap text-left preset-range-panel w-full sm:max-w-16rem" style="margin: 0 0.5rem">
          <div class="flex flex-wrap flex-1 p-4 gap-4">
            <Listbox :options="presetRanges" option-label="label" class="preset-range-list" @change="updateTimes"></Listbox>
            <div v-if="filterTimeTypeOptions.length > 1" class="time-types-select-box">
              <div v-for="item in filterTimeTypeOptions" :key="item" class="flex align-items-center p-2">
                <RadioButton v-model="selectedTypeOfTime" :input-id="`filterTimeType_${item}`" name="filterTimeType" :value="item" />
                <label :for="`filterTimeType_${item}`" class="ml-4">{{ $t(`FilterTimeType.${item}`) }}</label>
              </div>
            </div>
          </div>
          <div class="actions flex justify-content-center align-items-center">
            <Button
              type="button"
              outlined
              icon="pi pi-check"
              size="small"
              severity="primary"
              :label="$t('Dialog.Button.Accept')"
              text
              @click="confirm"
            ></Button>
            <Button
              type="button"
              outlined
              icon="pi pi-times"
              size="small"
              severity="danger"
              :label="$t('Dialog.Button.Close')"
              text
              @click="close(true)"
            ></Button>
          </div>
        </div>
        <div class="flex flex-1">
          <DatePicker
            ref="calendar"
            v-bind="$attrs"
            v-model="selectedDates"
            class="w-full"
            inline
            :show-time="showTime"
            selection-mode="range"
            hour-format="24"
            :manual-input="false"
          />
        </div>
      </div>
    </Popover>
  </div>
</template>

<script lang="ts">
import moment from "moment";
import { defineComponent } from "vue";
import formatDate from "@/plugins/dates/formatDate";
import DatePresetRange from "@/plugins/dates/ranges";

import { debounce } from "lodash";
import { startOfDay, endOfDay } from "date-fns";
import { useI18n } from "vue-i18n";

export default defineComponent({
  props: {
    buttonIcon: {
      type: String,
      default: () => {
        return "pi pi-calendar";
      },
      require: false
    },
    showText: {
      type: Boolean,
      default: () => {
        return true;
      },
      require: true
    },
    showInput: {
      type: Boolean,
      default: () => {
        return true;
      },
      require: true
    },
    size: {
      type: String,
      default: () => {
        return "small";
      },
      require: false
    },
    start: {
      type: Date || String,
      default: () => {
        return startOfDay(new Date());
      },
      require: true
    },
    end: {
      type: Date || String,
      default: () => {
        return endOfDay(new Date());
      },
      require: true
    },
    filterTimeType: {
      type: String,
      default: () => {
        return "CreatedTime";
      },
      require: true
    },
    filterTimeTypeOptions: {
      type: Array<string>,
      default: () => {
        return ["CreatedTime"];
      },
      require: true
    },
    showTime: {
      type: Boolean,
      default: () => {
        return false;
      },
      require: true
    },
    range: {
      type: String,
      default: () => {
        return "";
      },
      require: false
    },
    format: {
      type: String,
      default: (e) => {
        if (e.showTime) {
          return "DD/MM/YYYY HH:mm:ss";
        }
        return "DD/MM/YYYY";
      },
      require: false
    },
    placeholder: {
      type: String,
      require: false,
      default: () => {
        return "";
      }
    },
    width: {
      type: String,
      default: () => {
        return "auto";
      },
      require: false
    }
  },
  emits: ["valueUpdate", "onChanged", "hide"],
  data() {
    const dates: any = [null, null];
    return {
      $t: useI18n().t,
      invalid: false,
      temp: " - ",
      isOpened: false,
      presetRange: null as any,
      selectedTypeOfTime: this.filterTimeType,
      selectedDates: dates,
      dateRange: DatePresetRange,
      formatDate: formatDate
    };
  },
  computed: {
    presetRanges() {
      return this.dateRange.ranges().map((o) => {
        return {
          ...o,
          label: this.$t(o.label),
          range: this.dateRange.getDates(o.id)
        };
      });
    },
    text: {
      set: debounce(function (this: any, val) {
        this.temp = val;
        if (val == null || val == "" || val == "-") {
          this.selectedDates = [null, null];
        } else {
          try {
            const arr = val.split("-");
            const str = arr[0].trim();
            const end = arr[1].trim();
            const m1 = moment(str, this.format, true);
            const m2 = moment(end, this.format, true);
            this.invalid = !(m1.isValid() && m2.isValid());
            if (this.invalid) return;

            if (m1.toDate() != this.selectedDates[0]) {
              this.selectedDates[0] = m1.toDate();
            }
            if (m2.toDate() != this.selectedDates[1]) {
              this.selectedDates[1] = m2.toDate();
            }
          } catch (ex) {
            console.error(ex);
            this.invalid = true;
          }
        }
      }, 200),
      get: function () {
        if (this.selectedDates.length > 0 && !this.invalid) {
          const st = this.toString(this.selectedDates[0]);
          if (this.selectedDates[1] != null) {
            return st + " - " + this.toString(this.selectedDates[1]);
          }
          return st + " - ";
        }
        return this.temp;
      }
    }
  },
  mounted() {
    this.presetRange = this.range;
    this.reset();
  },
  methods: {
    toString(dt) {
      if (dt === null || dt === undefined) return "";
      return this.showTime ? this.formatDate.formatDate(dt) : this.formatDate.formatDateOnly(dt);
    },
    updateTimes(e) {
      this.invalid = false;
      this.presetRange = e.value.id;
      this.selectedDates = [...e.value.range];
    },
    notifyChanged() {
      if (this.selectedDates[0] == null || this.selectedDates[1] == null) return;
      if (this.selectedDates[0] == this.start && this.selectedDates[1] == this.end && this.selectedTypeOfTime == this.filterTimeType) return;
      const params = {
        filterTimeType: this.selectedTypeOfTime,
        start: startOfDay(this.selectedDates[0]),
        end: endOfDay(this.selectedDates[1]),
        presetRange: this.presetRange
      };
      this.$emit("valueUpdate", params);
      this.$emit("onChanged", params);
    },
    open(event) {
      const ref = this.$refs.overlayPanel as any;
      ref.show(event);
    },
    confirm() {
      this.invalid = false;
      this.close(false);
      this.notifyChanged();
    },
    close(resetBeforeClose) {
      if (resetBeforeClose) this.reset();
      const ref = this.$refs.overlayPanel as any;
      ref.hide();
    },
    reset() {
      this.invalid = false;
      this.selectedDates = [this.start, this.end];
      this.selectedTypeOfTime = this.filterTimeType;
    }
  }
});
</script>

<style scoped lang="scss">
.date-range-panel-body {
  background: white;
  border-radius: 6px;
  border: 1px solid #ced4da;
  > .preset-range-panel {
    position: relative;
    ::v-deep(.p-listbox) {
      border: none !important;
      box-shadow: none !important;
      padding-bottom: 0;
      .p-listbox-list-wrapper {
        .p-listbox-list {
          padding-top: 0;
          padding-bottom: 0;
          .p-listbox-item {
            padding: 0.5rem 0.75rem;
            height: 30px;
            display: flex;
            align-items: center;
          }
        }
      }
    }

    .p-divider {
      margin: 0.5rem 0;
    }

    .preset-range-list {
      flex: 1 0 10rem;
    }
    .time-types-select-box {
      flex: 1 0 14rem;
      div {
        height: 30px;
        align-items: center;
      }
    }
    .actions {
      width: 100%;
      margin: 0.5rem 0;
      text-align: center;
    }
  }
}
</style>
