import type { DefaultLocaleMessageSchema } from "vue-i18n";
import enUS from "./en-US.json";
import viVN from "./vi-VN.json";

const translations: { [key: string]: DefaultLocaleMessageSchema } = {
  "en-US": {
    ...enUS,
    ServerErrors: {
      ...enUS.ServerErrors,
      MODEL_STATE_INVALID: (ctx) => {
        const errors = ctx.named("errors") as Array<any>;
        const msg = errors.map((err) => JSON.stringify(err)).join(", ");
        return `Invalid data model. Errors: ${msg}`;
      }
    }
  },
  "vi-VN": {
    ...viVN,
    ServerErrors: {
      ...viVN.ServerErrors,
      MODEL_STATE_INVALID: (ctx) => {
        const errors = ctx.named("errors") as Array<any>;
        const msg = errors.map((err) => JSON.stringify(err)).join(", ");
        return `Dữ liệu không hợp lệ. Nội dung: ${msg}`;
      }
    }
  }
};

export default translations;
