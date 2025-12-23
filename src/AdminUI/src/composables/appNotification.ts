import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";

export function useAppNotification() {
  const $t = useI18n().t;
  const toast = useToast();

  const success = (message: string) => {
    toast.add({ severity: "success", summary: $t("Common.ToastSuccess"), detail: message, life: 5000 });
  };

  const info = (message: string) => {
    toast.add({ severity: "info", summary: $t("Common.ToastInfo"), detail: message, life: 5000 });
  };

  const warning = (message: string) => {
    toast.add({ severity: "warn", summary: $t("Common.ToastWarning"), detail: message, life: 5000 });
  };

  const error = (message: string) => {
    toast.add({ severity: "error", summary: $t("Common.ToastError"), detail: message, life: 5000 });
  };

  return {
    error,
    info,
    success,
    warning
  };
}
