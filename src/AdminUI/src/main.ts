/* eslint-disable vue/no-reserved-component-names */
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { SnackbarService } from "vue3-snackbar";
import "vue3-snackbar/dist/style.css";
import "@/styles/index.scss";

import Vuelidate from "@vuelidate/core";
import Lara from "@primeuix/themes/lara";
import PrimeVue from "primevue/config";

import CKEditor from "@ckeditor/ckeditor5-vue";
import { createI18n } from "vue-i18n";
import { createPinia } from "pinia";
import localStorageUtils from "./utils/localStorageUtils";
import StorageKeys from "./models/StorageKeys";
import translations from "./locales/index";
import VueLazyLoad from "vue3-lazyload";
import { definePreset } from "@primeuix/themes";

import {
  Accordion,
  AccordionPanel,
  AccordionHeader,
  AccordionContent,
  AutoComplete,
  Avatar,
  Badge,
  Breadcrumb,
  Button,
  Card,
  Carousel,
  Checkbox,
  ColorPicker,
  Column,
  ConfirmationService,
  ConfirmDialog,
  DataTable,
  DataView,
  DatePicker,
  Dialog,
  DialogService,
  Divider,
  Drawer,
  Fieldset,
  FileUpload,
  FloatLabel,
  FocusTrap,
  Fluid,
  Galleria,
  IconField,
  Image,
  InputGroup,
  InputGroupAddon,
  InputIcon,
  InputNumber,
  InputText,
  KeyFilter,
  Listbox,
  Menu,
  Menubar,
  Message,
  MultiSelect,
  OverlayBadge,
  Paginator,
  Panel,
  PanelMenu,
  Password,
  Popover,
  ProgressBar,
  ProgressSpinner,
  RadioButton,
  Rating,
  Ripple,
  ScrollPanel,
  Select,
  SelectButton,
  Skeleton,
  SpeedDial,
  SplitButton,
  TabPanel,
  Tabs,
  TabList,
  TabPanels,
  Tab,
  Tag,
  Textarea,
  Timeline,
  Toast,
  ToastService,
  ToggleSwitch,
  ToggleButton,
  Toolbar,
  Tooltip,
  Tree,
  TreeSelect,
  TreeTable
} from "primevue";
import Editor from "primevue/editor";
import Chart from "primevue/chart";

const primaryColor = "indigo"; // indigo | blue
const VikiworldPreset = definePreset(Lara, {
  components: {
    scrollpanel: {
      bar: {
        size: "var(--p-scrollbar-bar-size)"
      }
    },
    tabs: {
      tab: {
        borderWidth: "0 0 3px 0",
        padding: "1rem"
      }
    },
    popover: {
      root: {},
      content: {
        padding: "0"
      }
    }
  },
  semantic: {
    ...Lara.semantic,
    options: {},
    focusRing: { width: "1px", style: "dashed", color: "{primary.color}", offset: "1px" },
    primary: {
      50: `{${primaryColor}.50}`,
      100: `{${primaryColor}.100}`,
      200: `{${primaryColor}.200}`,
      300: `{${primaryColor}.300}`,
      400: `{${primaryColor}.400}`,
      500: `{${primaryColor}.500}`,
      600: `{${primaryColor}.600}`,
      700: `{${primaryColor}.700}`,
      800: `{${primaryColor}.800}`,
      900: `{${primaryColor}.900}`,
      950: `{${primaryColor}.950}`
    }
  }
});

const selectedLanguage = localStorageUtils.getItem(StorageKeys.Language) || import.meta.env.VITE_APP_DEFAULT_LANGUAGE;
const i18n = createI18n({
  locale: selectedLanguage,
  fallbackLocale: "en-US",
  legacy: false,
  globalInjection: true,
  allowComposition: true,
  flatJson: true,
  numberFormats: { "en-US": { currency: { style: "currency", currency: "VND" } }, "vi-VN": { currency: { style: "currency", currency: "VND" } } },
  messages: translations
});

const pinia = createPinia();
const app = createApp(App);
app.use(pinia);
app.use(router);
app.use(PrimeVue, { ripple: true, theme: { preset: VikiworldPreset, options: { prefix: "p", darkModeSelector: false, cssLayer: false } } });
app.use(SnackbarService);
app.use(CKEditor);
app.use(i18n);
app.use(Vuelidate);
app.use(ToastService);
app.use(DialogService);
app.use(ConfirmationService);
app.use(VueLazyLoad, {});

app.directive("tooltip", Tooltip);
app.directive("focustrap", FocusTrap);
app.directive("ripple", Ripple);
app.directive("keyfilter", KeyFilter);

app.component("Button", Button);
app.component("Card", Card);
app.component("Carousel", Carousel);
app.component("Accordion", Accordion);
app.component("Editor", Editor);
app.component("AccordionPanel", AccordionPanel);
app.component("AccordionHeader", AccordionHeader);
app.component("AccordionContent", AccordionContent);
app.component("Checkbox", Checkbox);
app.component("InputText", InputText);
app.component("InputNumber", InputNumber);
app.component("InputGroup", InputGroup);
app.component("InputGroupAddon", InputGroupAddon);
app.component("Popover", Popover);
app.component("Drawer", Drawer);
app.component("Menubar", Menubar);
app.component("Breadcrumb", Breadcrumb);
app.component("DataTable", DataTable);
app.component("Column", Column);
app.component("Paginator", Paginator);
app.component("DatePicker", DatePicker);
app.component("Dialog", Dialog);
app.component("Toast", Toast);
app.component("Toolbar", Toolbar);
app.component("Menu", Menu);
app.component("FileUpload", FileUpload);
app.component("MultiSelect", MultiSelect);
app.component("ConfirmDialog", ConfirmDialog);
app.component("Listbox", Listbox);
app.component("TabPanel", TabPanel);
app.component("Skeleton", Skeleton);
app.component("Panel", Panel);
app.component("PanelMenu", PanelMenu);
app.component("Select", Select);
app.component("SelectButton", SelectButton);
app.component("Textarea", Textarea);
app.component("AutoComplete", AutoComplete);
app.component("Timeline", Timeline);
app.component("Badge", Badge);
app.component("RadioButton", RadioButton);
app.component("DataView", DataView);
app.component("ProgressBar", ProgressBar);
app.component("Tag", Tag);
app.component("Fluid", Fluid);
app.component("Tab", Tab);
app.component("TabList", TabList);
app.component("TabPanels", TabPanels);
app.component("Tabs", Tabs);
app.component("Password", Password);
app.component("Avatar", Avatar);
app.component("Divider", Divider);
app.component("ProgressSpinner", ProgressSpinner);
app.component("SpeedDial", SpeedDial);
app.component("Tree", Tree);
app.component("Image", Image);
app.component("Galleria", Galleria);
app.component("Rating", Rating);
app.component("Chart", Chart);
app.component("ScrollPanel", ScrollPanel);
app.component("Fieldset", Fieldset);
app.component("SplitButton", SplitButton);
app.component("ToggleSwitch", ToggleSwitch);
app.component("TreeTable", TreeTable);
app.component("TreeSelect", TreeSelect);
app.component("IconField", IconField);
app.component("InputIcon", InputIcon);
app.component("Message", Message);
app.component("FloatLabel", FloatLabel);
app.component("OverlayBadge", OverlayBadge);
app.component("ColorPicker", ColorPicker);
app.component("ToggleButton", ToggleButton);

app.mount("#app");
