import { defineStore } from "pinia";

interface State {
  appName: string;
}

const useAppDataStore = defineStore("APP_DATA_STORE", {
  state: (): State => {
    return {
      appName: "Cms"
    };
  },
  getters: {},
  actions: {}
});

export default useAppDataStore;
