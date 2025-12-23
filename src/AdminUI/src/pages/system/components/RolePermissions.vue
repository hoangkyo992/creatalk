<template>
  <Dialog
    :visible="true"
    :style="{ width: '90vw', maxWidth: '720px', minWidth: '360px', maxHeight: '80vh' }"
    :header="$t('RolesPage.UpdatePermissions', [name])"
    :modal="true"
    :closable="false"
  >
    <div class="p-0">
      <Tree v-model:selection-keys="selectedIds" v-model:expanded-keys="expandedKeys" :value="appFeatures" selection-mode="checkbox" class="w-full">
        <template #default="slotProps">
          <b>{{ slotProps.node.description }}</b> <small v-if="slotProps.node.name">[{{ slotProps.node.name }}]</small>
        </template>
      </Tree>
    </div>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="grantStore.hasPermission(`Administration.Roles`, `GrantAccess`)"
        type="submit"
        icon="pi pi-save"
        :label="$t('Common.Actions.SaveChanges')"
        size="small"
        :loading="isLoading"
        @click="onUpdate"
      ></Button>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import RoleService from "@/services/RoleService";
import { useAppNotification } from "@/composables/appNotification";
import { onMounted, ref } from "vue";
import useProfileStore from "@/stores/profileStore";
import { useConfirm } from "primevue/useconfirm";
import { useI18n } from "vue-i18n";

interface Props {
  id: string;
  name?: string;
}
const props = defineProps<Props>();
const $t = useI18n().t;
const grantStore = useProfileStore();
const crudService = new RoleService();
const isLoading = ref<boolean>(false);

const selectedIds = ref<string[]>([]);
const expandedKeys = ref<any>([]);
const appFeatures = ref<any[]>([]);
const confirm = useConfirm();
const notifier = useAppNotification();

const emits = defineEmits(["close"]);
const onUpdate = async () => {
  try {
    isLoading.value = true;
    const selectedItems = Object.keys(selectedIds.value)
      .map((x) => {
        const arr = x.split("__");
        if (arr.length == 2) {
          return {
            name: arr[0],
            action: arr[1]
          };
        }
        return {
          name: "",
          action: ""
        };
      })
      .filter((x) => x.name?.length > 0);
    confirm.require({
      message: $t("RolesPage.Confirmations.UpdatePermissions", [name]),
      header: $t("Dialog.Title.Confirm"),
      acceptLabel: $t("Dialog.Button.Accept"),
      rejectLabel: $t("Dialog.Button.Reject"),
      acceptClass: "p-button-danger p-button-sm w-auto",
      rejectClass: "p-button-outlined p-button-sm w-auto",
      icon: "pi pi-question-circle",
      accept: async () => {
        await crudService.updateFeatures(props.id, { features: selectedItems });
        notifier.success($t("Dialog.Alert.UpdateSuccess"));
        emits("close", true);
      }
    });
  } finally {
    isLoading.value = false;
  }
};
const expandAll = () => {
  for (const node of appFeatures.value) {
    expandNode(node);
  }
};
const expandNode = (node) => {
  if (node.children && node.children.length) {
    expandedKeys.value[node.key] = true;

    for (const child of node.children) {
      expandNode(child);
    }
  }
};

onMounted(async () => {
  isLoading.value = true;
  const apiResponse = await crudService.getFeatures(props.id);

  function onlyUnique(value, index, array) {
    return array.indexOf(value) === index;
  }
  const modules = apiResponse.result.appFeatures
    .reverse()
    .map((x) => x.module)
    .filter(onlyUnique);

  appFeatures.value = modules.map((m) => {
    const features = apiResponse.result.appFeatures.filter((x) => x.module == m);
    const module = {
      key: m,
      description: m,
      children: features.map((x) => {
        const obj = {
          ...x,
          key: `${x.name}`,
          children: x.actions.map((z) => {
            const k = `${x.name}__${z.name}`;
            const sk = apiResponse.result.features.find((f) => f.name == x.name.toUpperCase() && f.action == z.name.toUpperCase());
            if (sk != null) {
              selectedIds.value[k] = {
                checked: true
              };
            }
            return {
              ...z,
              isSelected: sk != null,
              key: k
            };
          })
        };
        const ssk = obj.children.filter((x) => x.isSelected);
        if (ssk.length > 0) {
          selectedIds.value[obj.key] = { checked: ssk.length == obj.children.length, partialChecked: ssk.length != obj.children.length };
        }
        return {
          ...obj,
          isSelected: selectedIds.value[obj.key] || {}
        };
      })
    };
    const mk = module.children.map((x) => {
      return x.isSelected;
    });
    const checked = mk.filter((k) => k.checked).length == module.children.length;

    selectedIds.value[module.key] = {
      checked: checked,
      partialChecked:
        (!checked && mk.filter((k) => k.checked).length > 0) ||
        (mk.filter((k) => k.partialChecked).length > 0 && mk.filter((k) => k.partialChecked).length != module.children.length)
    };
    return module;
  });
  expandAll();
  isLoading.value = false;
});
</script>
