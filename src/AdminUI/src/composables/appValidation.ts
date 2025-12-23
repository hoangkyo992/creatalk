import { useI18n } from "vue-i18n";
import { helpers, maxLength, minValue } from "@vuelidate/validators";

export function useAppValidation() {
  const $t = useI18n().t;

  const getErrorMessage = function (validation: any): string {
    return validation.$error ? validation.$errors[0].$message : "";
  };
  const getErrorMessages = function (validation: any): string[] {
    return validation.$error ? validation.$errors.map((x) => x.$message) : [];
  };
  const maxLengthValidator = function (length: number) {
    return helpers.withMessage(
      ({ $model }) =>
        $t("Common.Validations.MaxLength", {
          maxLength: length,
          totalLength: $model.length
        }),
      maxLength(length)
    );
  };
  const minValueValidator = function (min: number) {
    return helpers.withMessage(
      () =>
        $t("Common.Validations.MinValue", {
          min: min
        }),
      minValue(min)
    );
  };
  return {
    getErrorMessage,
    getErrorMessages,
    minValue: minValueValidator,
    maxLength: maxLengthValidator
  };
}
