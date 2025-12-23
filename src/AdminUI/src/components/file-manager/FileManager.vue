<template>
  <div class="file-manager--container" :class="`${fileProps.size}-icon`" v-bind="$attrs">
    <Toolbar :model="paths" class="file-manager--toolbar">
      <template #start>
        <Button
          v-tooltip="`${$t('FileManager.GoBack')}`"
          :disabled="!canGoBack"
          text
          icon="pi pi-arrow-left"
          class="mr-2"
          :severity="canGoBack ? 'primary' : 'secondary'"
          @click="fileManagerStore.goBack"
        ></Button>
        <div class="p-2 border-1 surface-border border-round w-full inline-flex align-items-center">
          <Button
            v-tooltip="`${$t('FileManager.GoHome')}`"
            :disabled="!canGoBack"
            text
            icon="pi pi-home"
            class="mr-2"
            :severity="canGoBack ? 'primary' : 'secondary'"
            @click="fileManagerStore.viewHome"
          ></Button>
          <a
            v-for="item in paths"
            :key="item.label"
            @click="
              async () => {
                if (item.id) {
                  await copy(item.id);
                  notifier.success($t(`FileManager.Copied`));
                }
              }
            "
            v-ripple
            v-tooltip.top="`Click to copy Id`"
            class="flex align-items-center cursor-pointer line-height-1"
          >
            <i class="pi pi-angle-right"></i>
            <span class="ml-2 p-3">{{ item.label }}</span>
          </a>
        </div>
      </template>
      <template #end>
        <div class="flex align-items-center gap-4">
          <IconField icon-position="left">
            <InputIcon>
              <i class="pi pi-search"></i>
            </InputIcon>
            <InputText
              v-model="keyword"
              :placeholder="$t('Common.Messages.SearchPlaceholder')"
              input-class="w-full p-inputtext-sm"
              size="small"
              type="text"
              class="w-8rem sm:w-auto"
            />
          </IconField>
          <Button
            v-tooltip="`${$t('FileManager.Trash')}`"
            :outlined="viewMode == 'Home'"
            icon="pi pi-trash"
            severity="primary"
            @click="fileManagerStore.viewTrash"
          />
        </div>
      </template>
    </Toolbar>
    <div class="file-manager--progress-bar" style="height: 4px">
      <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    </div>
    <div class="file-manager--viewbox flex flex-1">
      <div
        class="file-manager--listbox flex flex-wrap gap-8 p-4 mt-2 overflow-auto h-auto"
        :class="items.length == 0 ? 'empty' : ''"
        @click="
          () => {
            selectedItemId = null;
            selectedItem = undefined;
          }
        "
        @contextmenu="onItemRightClick($event, undefined)"
      >
        <div
          v-for="item in items"
          :key="item.id"
          :class="selectedItemId == item.id ? 'focused' : ''"
          class="flex flex-column file-manager--directory-item"
          @click="
            (evt) => {
              evt.stopPropagation();
              itemContextMenu.hide(evt);
              currentFolderContextMenu.hide(evt);
              selectedItemId = item.id;
              selectedItem = item;
            }
          "
          @contextmenu="onItemRightClick($event, item)"
          @dblclick="onDbClick($event, item)"
        >
          <div class="directory-thumbnail flex align-items-center justify-content-center">
            <img v-if="item.isDirectory" src="@/assets/folders/icon_128x128@2x.png" />
            <img
              v-if="!item.isDirectory && item.fileTypeId == FileType.Image"
              v-lazy="`${item.url}?size=thumb`"
              width="auto"
              height="auto"
              style="max-width: 100%; max-height: 100%"
            />
            <img
              v-if="!item.isDirectory && item.fileTypeId == FileType.Document"
              src="@/assets/folders/icon-pdf.png"
              width="auto"
              height="auto"
              style="max-width: 100%; max-height: 100%"
            />
          </div>
          <div class="directory-name">
            {{ item.name }}
          </div>
        </div>
        <div v-if="items?.length == 0" class="w-full text-center">
          <span>{{ keyword?.length ? $t("FileManager.Messages.NoData") : $t("FileManager.Messages.EmptyFolder") }}</span>
        </div>
      </div>
      <div v-if="showDetail" class="file-manager--detail-box flex flex-column surface-border border-1">
        <div class="flex flex-column w-full gap-6">
          <div class="directory-thumbnail flex align-items-center justify-content-center">
            <img v-if="!selectedItem || selectedItem?.isDirectory" src="@/assets/folders/icon_128x128@2x.png" style="max-width: 128px" />
            <img
              v-if="selectedItem && !selectedItem?.isDirectory && selectedItem?.fileTypeId == FileType.Image"
              v-lazy="`${selectedItem?.url}?size=medium`"
              width="auto"
              height="auto"
              style="max-width: 100%; max-height: 100%"
            />
            <img
              v-if="selectedItem && !selectedItem?.isDirectory && selectedItem.fileTypeId == FileType.Document"
              src="@/assets/folders/icon-pdf.png"
              width="auto"
              height="auto"
              style="max-width: 100%; max-height: 100%"
            />
          </div>
          <div class="directory-details flex flex-column gap-4">
            <div class="directory-name font-bold">
              {{ selectedItem?.name ?? currentFolder?.name }}
            </div>
            <div>{{ formatDate.formatDate(selectedItem?.createdTime || new Date()) }}</div>
            <div v-if="selectedItem">{{ toHumanFileSize(selectedItem?.size) }}</div>
            <div v-if="selectedItem?.fileTypeId == FileType.Image">
              {{ dimensions }}
            </div>
            <div v-if="!selectedItem">{{ $t("FileManager.Messages.NumberOfItems", [items.length]) }}</div>
          </div>
        </div>
        <Divider class="my-4" />
        <div>
          <div class="flex justify-content-end">
            <Button
              type="button"
              icon="pi pi-check"
              :label="$t('Common.Actions.Select')"
              severity="primary"
              class="w-auto"
              size="small"
              :loading="isLoading"
              v-if="!fileProps.multiple && selectedItem && !selectedItem.isDirectory"
              @click="onSelect"
            ></Button>
          </div>
        </div>
      </div>
    </div>
    <div class="file-manager--statistics-bar">
      <div>
        {{ $t("FileManager.Messages.NumberOfItems", [items.length]) }} | {{ $t("FileManager.Messages.NumberOfSelectedItems", [selectedItemId ? 1 : 0]) }}
      </div>
      <div class="flex align-items-center gap-2">
        <Button outlined icon="pi pi-th-large" :severity="showDetail ? 'primary' : 'secondary'" @click="showDetail = !showDetail"></Button>
      </div>
    </div>

    <div class="file-manager--selection-bar"></div>

    <ContextMenu ref="itemContextMenu" :model="contextMenuItemsForItem">
      <template #item="{ item, props }">
        <a v-if="!item.disabled" v-ripple class="flex align-items-center" v-bind="props.action">
          <span v-if="item.icon" class="mr-6" :class="item.icon" />
          <span>{{ item.label }}</span>
          <Badge v-if="item.badge" class="ml-auto" />
          <span v-if="item.shortcut" class="ml-auto border-1 surface-border border-round surface-100 text-xs p-1">{{ item.shortcut }}</span>
          <i v-if="item.items" class="pi pi-angle-right ml-auto"></i>
        </a>
      </template>
    </ContextMenu>

    <ContextMenu ref="currentFolderContextMenu" :model="contextMenuItemsForCurrentFolder">
      <template #item="{ item, props }">
        <a v-if="!item.disabled" v-ripple class="flex align-items-center" v-bind="props.action">
          <span v-if="item.icon" class="mr-6" :class="item.icon" />
          <span>{{ item.label }}</span>
          <Badge v-if="item.badge" class="ml-auto" />
          <span v-if="item.shortcut" class="ml-auto border-1 surface-border border-round surface-100 text-xs p-1">{{ item.shortcut }}</span>
          <i v-if="item.items" class="pi pi-angle-right ml-auto"></i>
        </a>
      </template>
    </ContextMenu>

    <FileUpload
      ref="uploader"
      mode="basic"
      :multiple="true"
      class="hidden"
      name="files[]"
      accept="image/*,application/pdf"
      :auto="true"
      :choose-label="$t('UploadDialog.ChooseLabel')"
      :loading="isUploading"
      :custom-upload="true"
      severity="danger"
      @uploader="customArrayBufferUploader"
    ></FileUpload>

    <FileManagerDialog v-if="showRenameDialog">
      <RenameDialog
        :dir-item="selectedItem"
        class="p-card-in-dialog"
        @close="
          () => {
            showRenameDialog = false;
          }
        "
      ></RenameDialog>
    </FileManagerDialog>

    <FileManagerDialog v-if="showNewFolderDialog">
      <NewFolderDialog
        class="p-card-in-dialog"
        @close="
          () => {
            showNewFolderDialog = false;
          }
        "
      ></NewFolderDialog>
    </FileManagerDialog>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref, computed } from "vue";
import { useAppNotification } from "@/composables/appNotification";
import { useI18n } from "vue-i18n";
import ContextMenu from "primevue/contextmenu";
import type { FolderItemItemDto } from "@/contracts/FileAndFolders";
import useFileManagerStore from "@/stores/fileManagerStore";
import { storeToRefs } from "pinia";
import { useClipboard } from "@vueuse/core";
import { FileType } from "@/contracts/Enums";
import useProfileStore from "@/stores/profileStore";
import formatDate from "@/plugins/dates/formatDate";

interface Props {
  size?: "small" | "medium";
  multiple?: boolean;
  showDetailPanel?: boolean;
}

const fileProps = withDefaults(defineProps<Props>(), {
  size: () => "medium",
  multiple: () => false
});

const showDetail = ref(fileProps.showDetailPanel);

const grantStore = useProfileStore();
const fileManagerStore = useFileManagerStore();
const { copy } = useClipboard({ legacy: true });
const { isLoading, isUploading, keyword, viewMode, paths, items, canGoBack, sortBy, itemToMove, sortDirection, currentFolderId, currentFolder } =
  storeToRefs(fileManagerStore);

const selectedItemId = ref();
const selectedItem = ref<FolderItemItemDto>();
const showRenameDialog = ref(false);
const showNewFolderDialog = ref(false);

const onDelete = async () => {
  if (!selectedItem.value) return;
  await fileManagerStore.operate(selectedItem.value.id, {}, selectedItem?.value.isDirectory ? "DELETE_FOLDER" : "DELETE_FILE");
  notifier.success($t("Dialog.Alert.DeleteSuccess"));
};

const onMove = async () => {
  if (!itemToMove?.value) return;
  await fileManagerStore.operate(itemToMove?.value?.id ?? "", {}, "MOVE_ITEM");
  notifier.success($t("Dialog.Alert.UpdateSuccess"));
};

const onRestore = async () => {
  if (!selectedItem.value) return;
  await fileManagerStore.operate(selectedItem.value.id, {}, selectedItem?.value.isDirectory ? "RESTORE_FOLDER" : "RESTORE_FILE");
  notifier.success($t("Dialog.Alert.UpdateSuccess"));
};

const dimensions = computed(() => {
  if (!selectedItem.value?.properties) {
    return "";
  }
  const properties = selectedItem.value.properties.split(";").map((x) => {
    return {
      name: x.split("=")[0],
      value: x.split("=")[1]
    };
  });
  const width = properties.find((x) => x.name == "Width")?.value ?? "-";
  const height = properties.find((x) => x.name == "Height")?.value ?? "-";
  return `${width} x ${height} pixels`;
});

const itemContextMenu = ref();
const currentFolderContextMenu = ref();
const contextMenuItemsForItem = computed(() => {
  return [
    {
      label: $t("FileManager.Delete"),
      icon: "pi pi-trash",
      disabled: viewMode.value == "Trash" || !grantStore.hasPermission(`Cdn.Library`, `Delete`),
      command: () => {
        onDelete();
      }
    },
    {
      label: $t("FileManager.Restore"),
      icon: "pi pi-replay",
      disabled: viewMode.value == "Home" || !grantStore.hasPermission(`Cdn.Library`, `Delete`),
      command: () => {
        onRestore();
      }
    },
    {
      label: $t("FileManager.CopyURL"),
      icon: "pi pi-link",
      disabled: viewMode.value == "Trash",
      command: () => {
        copy(selectedItem.value?.url ?? "");
        notifier.success($t("FileManager.Copied"));
      }
    },
    {
      label: $t("FileManager.Move"),
      icon: "pi pi-folder",
      disabled: viewMode.value == "Trash" || !grantStore.hasPermission(`Cdn.Library`, `Move`),
      command: async () => {
        if (selectedItem.value) fileManagerStore.setItemToMove(selectedItem.value);
      }
    },
    {
      label: $t("FileManager.Rename"),
      icon: "pi pi-file-edit",
      disabled: viewMode.value == "Trash" || !grantStore.hasPermission(`Cdn.Library`, `Rename`),
      command: () => {
        showRenameDialog.value = true;
      }
    },
    {
      label: $t("FileManager.Properties"),
      icon: "pi pi-info-circle",
      command: () => {
        showDetail.value = true;
      }
    }
  ];
});
const contextMenuItemsForCurrentFolder = computed(() => {
  return [
    {
      label: $t("FileManager.Upload"),
      icon: "pi pi-upload",
      disabled: viewMode.value == "Trash" || !grantStore.hasPermission(`Cdn.Library`, `Upload`),
      command: (evt) => {
        (uploader.value as any)?.$refs?.fileInput.click(evt);
      }
    },
    {
      label: $t("FileManager.NewFolder"),
      icon: "pi pi-folder",
      disabled: viewMode.value == "Trash" || !grantStore.hasPermission(`Cdn.Library`, `Create`),
      command: async () => {
        showNewFolderDialog.value = true;
      }
    },
    {
      label: $t("FileManager.Paste"),
      icon: "pi pi-folder",
      disabled: !itemToMove?.value || currentFolderId.value == itemToMove.value?.parentId || !grantStore.hasPermission(`Cdn.Library`, `Move`),
      command: async () => {
        await onMove();
      }
    },
    {
      label: $t("FileManager.Properties"),
      icon: "pi pi-info-circle",
      command: () => {
        showDetail.value = true;
      }
    },
    {
      separator: true
    },
    {
      label: $t("FileManager.SortBy"),
      icon: "pi pi-sort-alt",
      items: [
        {
          label: $t("FileManager.Name"),
          icon: "",
          badge: sortBy.value === "Name" ? 1 : false,
          command: () => {
            fileManagerStore.setSortField("Name");
          }
        },
        {
          label: $t("FileManager.CreatedDate"),
          icon: "",
          badge: sortBy.value === "Date" ? 1 : false,
          command: () => {
            fileManagerStore.setSortField("Date");
          }
        },
        {
          label: $t("FileManager.Size"),
          icon: "",
          badge: sortBy.value === "Size" ? 1 : false,
          command: () => {
            fileManagerStore.setSortField("Size");
          }
        },
        {
          separator: true
        },
        {
          label: $t("FileManager.SortAscending"),
          icon: "pi pi-sort-alpha-up",
          badge: sortDirection.value === "ASC" ? 1 : false,
          command: () => {
            fileManagerStore.setSortDirection("ASC");
          }
        },
        {
          label: $t("FileManager.SortDescending"),
          icon: "pi pi-sort-alpha-down",
          badge: sortDirection.value === "DESC" ? 1 : false,
          command: () => {
            fileManagerStore.setSortDirection("DESC");
          }
        }
      ]
    }
  ];
});

const onItemRightClick = (event, item) => {
  event.stopPropagation();
  selectedItem.value = item;
  selectedItemId.value = item?.id;
  if (item) {
    currentFolderContextMenu.value.hide(event);
    itemContextMenu.value.show(event);
  } else {
    itemContextMenu.value.hide(event);
    currentFolderContextMenu.value.show(event);
  }
};

const onDbClick = async (event, item: FolderItemItemDto) => {
  event.stopPropagation();
  if (item.isDirectory && viewMode.value != "Trash") {
    fileManagerStore.getItems(item.id);
  } else if (!item.isDirectory && !fileProps.multiple) {
    onSelect();
  }
};

const $t = useI18n().t;
const notifier = useAppNotification();
const uploader = ref(null);
const customArrayBufferUploader = async (event) => {
  const response = await fileManagerStore.upload(event.files);
  if (response?.uploadedFiles?.length) {
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
  }
};

onMounted(async () => {
  if (!currentFolderId.value) await fileManagerStore.viewHome();
});

const toHumanFileSize = (size) => {
  const i = size == 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
  return +(size / Math.pow(1024, i)).toFixed(2) * 1 + " " + ["B", "kB", "MB", "GB", "TB"][i];
};

const emits = defineEmits(["onSelect"]);
const onSelect = () => {
  emits("onSelect", {
    items: [selectedItem.value]
  });
};
</script>

<style lang="scss" scoped>
.file-manager--container {
  position: relative;
  display: flex;
  min-width: 320px;
  flex-direction: column;
  box-shadow: 0px 0px 4px 2px #dee6fe;
  overflow: hidden;
  flex-wrap: nowrap;
  --directory-item--width: 128px;
  --directory-item--height: 176px;
  --grid-layout-gap: 10px;
  --grid-column-count: 13;
  --gap-count: calc(var(--grid-column-count) - 1);
  --total-gap-width: calc(var(--gap-count) * var(--grid-layout-gap));
  --grid-item--max-width: calc((100% - var(--total-gap-width)) / var(--grid-column-count));

  &.small-icon {
    --directory-item--width: 96px;
    --directory-item--height: 142px;
  }

  .file-manager--statistics-bar {
    font-size: 12px;
    padding: 4px 6px;
    display: inline-flex;
    justify-content: space-between;
    align-items: center;
  }
  .file-manager--viewbox {
    font-size: 12px;
    overflow: auto;

    .file-manager--listbox {
      display: grid !important;
      grid-template-columns: repeat(auto-fill, minmax(max(var(--directory-item--width), var(--grid-item--max-width)), 1fr));
      grid-template-rows: repeat(auto-fit, var(--directory-item--height));
      grid-gap: var(--grid-layout-gap);
      background-color: rgb(255 255 255);
      height: 100%;
      width: 100%;

      &.empty {
        display: flex !important;
        justify-content: center;
      }

      .file-manager--directory-item {
        padding: 8px 0;
        width: var(--directory-item--width);
        height: var(--directory-item--height);
        border-radius: 6px;
        overflow: hidden;
        margin: 0 auto !important; // Important to be center in cells
        cursor: pointer;

        &:hover {
          background-color: rgb(172, 172, 172, 0.3);
        }

        &.focused {
          background-color: rgb(172, 172, 172, 0.3);
        }

        .directory-thumbnail {
          text-align: center;
          width: var(--directory-item--width);
          height: var(--directory-item--width);
          border-radius: 6px;
          padding: 2px;
          max-height: 200px;

          > img {
            border-radius: 3px;
            width: var(--directory-item--width);
            height: var(--directory-item--width);
            object-fit: contain;
          }
        }

        .directory-name {
          overflow: hidden;
          text-align: center;
          text-overflow: ellipsis;
          word-break: break-all;
          width: 100%;
          height: 36px;
          padding: 0 10px;
          display: -webkit-box;
          -webkit-line-clamp: 2;
          line-clamp: 2;
          -webkit-box-orient: vertical;
        }
      }
    }

    .file-manager--detail-box {
      width: 40%;
      max-width: 256px;
      padding: 8px;
      background-color: rgb(255 255 255);
    }
  }

  ::v-deep(.p-toolbar) {
    .p-toolbar-start {
      flex: 1;
    }
  }
}
</style>
