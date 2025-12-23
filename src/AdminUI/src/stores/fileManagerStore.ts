import { defineStore } from "pinia";
import type { FolderDetailItemDto, FolderItemItemDto } from "@/contracts/FileAndFolders";
import FolderService from "@/services/FolderService";
import FileService from "@/services/FileService";
import { FileType, FolderStatus } from "@/contracts/Enums";

interface State {
  currentFolder?: FolderDetailItemDto;
  homeFolderId: string;
  currentFolderId: string;
  selectedFolderId?: string;
  selectedFileIds: string[];
  isUploading: boolean;
  isLoading: boolean;
  folderService: FolderService;
  fileService: FileService;
  viewMode: "Home" | "Trash";
  selectedItemName: string;
  sortBy: "Name" | "Date" | "Size";
  sortDirection: "ASC" | "DESC";
  keyword?: string;
  fileTypeId?: FileType;
  itemToMove?: FolderItemItemDto;
}

const useFileManagerStore = defineStore("FILE_MANAGER_STORE", {
  state: (): State => {
    return {
      homeFolderId: "",
      currentFolderId: "",
      isLoading: false,
      isUploading: false,
      fileService: new FileService(),
      folderService: new FolderService(),
      selectedFileIds: [],
      selectedFolderId: "",
      viewMode: "Home",
      currentFolder: undefined,
      selectedItemName: "",
      sortBy: "Name",
      sortDirection: "ASC",
      keyword: undefined,
      fileTypeId: undefined,
      itemToMove: undefined
    };
  },
  getters: {
    paths: (state) => {
      const paths = [
        {
          id: state.homeFolderId,
          icon: "pi pi-home",
          label: state.viewMode == "Home" ? "Home" : "Recycle Bin"
        }
      ];
      if (state.viewMode == "Home" && state.currentFolderId != state.homeFolderId) {
        if (state.currentFolder?.parentId != state.homeFolderId) {
          paths.push({
            id: "",
            icon: "",
            label: "..."
          });
        }
        paths.push({
          id: state.currentFolder?.id ?? "",
          icon: "",
          label: state.currentFolder?.name ?? ""
        });
      }
      return paths;
    },
    items: (state) => {
      if (state.currentFolder?.items?.length) {
        const applySort = (a: FolderItemItemDto, b: FolderItemItemDto, sortBy: string, sortDirection: string) => {
          switch (sortBy) {
            case "Name":
              return sortDirection == "ASC" ? a.name.localeCompare(b.name) : -a.name.localeCompare(b.name);
            case "Size":
              return sortDirection == "ASC" ? a.size - b.size : b.size - a.size;
            case "Date":
              return sortDirection == "ASC"
                ? new Date(a.createdTime).getTime() - new Date(b.createdTime).getTime()
                : new Date(b.createdTime).getTime() - new Date(a.createdTime).getTime();
            default:
              return 0;
          }
        };

        const folders =
          state.currentFolder?.items
            .filter((x) => x.isDirectory)
            .sort((a, b) => {
              return applySort(a, b, state.sortBy, state.sortDirection);
            }) ?? [];
        const files =
          (!state.fileTypeId ? state.currentFolder.items : state.currentFolder.items.filter((x) => x.fileTypeId == state.fileTypeId))
            .filter((x) => !x.isDirectory)
            .sort((a, b) => {
              return applySort(a, b, state.sortBy, state.sortDirection);
            }) ?? [];

        const setKeys = new Set(state.keyword);
        return state.keyword?.length
          ? [...folders, ...files].filter((x) => {
              return [...new Set(x.name)].filter((x) => setKeys.has(x)).length == state.keyword?.length;
            })
          : [...folders, ...files];
      }
      return [];
    },
    canGoBack: (state) => state.viewMode == "Trash" || state.currentFolder?.parentId
  },
  actions: {
    setSortField(sortBy: "Name" | "Date" | "Size") {
      this.sortBy = sortBy;
    },
    setSortDirection(sortDirection: "ASC" | "DESC") {
      this.sortDirection = sortDirection;
    },
    setItemToMove(itemToMove?: FolderItemItemDto) {
      this.itemToMove = itemToMove;
    },
    setFileType(typeId?: FileType) {
      this.fileTypeId = typeId;
    },
    async viewTrash() {
      this.isLoading = true;
      try {
        const apiResponse = await this.folderService.getInTrash();
        this.viewMode = "Trash";
        this.currentFolder = apiResponse.result;
      } finally {
        this.isLoading = false;
      }
    },
    async viewHome() {
      if (!this.homeFolderId) {
        const apiResponse = await this.folderService.getList();
        if (apiResponse.isSuccess && apiResponse.result.items.length > 0) {
          const root = apiResponse.result.items.find((x) => x.parentId == undefined);
          if (root) this.homeFolderId = root.id;
          else this.homeFolderId = apiResponse.result.items.find((x) => x.statusId == FolderStatus.Active)?.id ?? "";
        }
      }
      return await this.getItems(this.homeFolderId);
    },
    async goBack() {
      if (this.currentFolder?.parentId) return await this.getItems(this.currentFolder?.parentId);
      return await this.getItems(this.homeFolderId);
    },
    async operate(id: string, data: any, operation: string) {
      switch (operation) {
        case "RENAME_FILE":
          await this._renameFile(id, data);
          break;
        case "RENAME_FOLDER":
          await this._renameFolder(id, data);
          break;
        case "CREATE_FOLDER":
          await this._createFolder(data);
          break;
        case "DELETE_FILE":
          await this._deleteFile(id);
          break;
        case "DELETE_FOLDER":
          await this._deleteFolder(id);
          break;
        case "RESTORE_FOLDER":
          await this._restoreFolder(id);
          break;
        case "RESTORE_FILE":
          await this._restoreFile(id);
          break;
        case "MOVE_ITEM":
          await this._moveItem();
          break;
      }
    },
    async _createFolder(data: any) {
      const apiResponse = await this.folderService.create({
        name: data.name,
        parentId: this.currentFolderId
      });
      this.currentFolder?.items.push(apiResponse.result);
      return apiResponse.result;
    },
    async _moveItem() {
      if (this.itemToMove) {
        const apiResponse = this.itemToMove.isDirectory
          ? await this.folderService.move(this.itemToMove.id, {
              parentId: this.currentFolderId
            })
          : await this.fileService.move(this.itemToMove.id, {
              folderId: this.currentFolderId
            });
        if (this.currentFolder) {
          this.currentFolder.items.push(this.itemToMove);
        }
        this.itemToMove = undefined;
        return apiResponse.result;
      }
    },
    async _deleteFolder(id: string) {
      const apiResponse = await this.folderService.moveToTrash(id);
      if (this.currentFolder) {
        this.currentFolder.items = this.currentFolder.items.filter((x) => x.id !== id);
      }
      return apiResponse.result;
    },
    async _restoreFolder(id: string) {
      const apiResponse = await this.folderService.restoreFromTrash(id);
      if (this.currentFolder) {
        this.currentFolder.items = this.currentFolder.items.filter((x) => x.id !== id);
      }
      return apiResponse.result;
    },
    async _deleteFile(id: string) {
      const apiResponse = await this.fileService.moveToTrash(id);
      if (this.currentFolder) {
        this.currentFolder.items = this.currentFolder.items.filter((x) => x.id !== id);
      }
      return apiResponse.result;
    },
    async _restoreFile(id: string) {
      const apiResponse = await this.fileService.restoreFromTrash(id);
      if (this.currentFolder) {
        this.currentFolder.items = this.currentFolder.items.filter((x) => x.id !== id);
      }
      return apiResponse.result;
    },
    async _renameFile(id: string, data: any) {
      const apiResponse = await this.fileService.rename(id, data);
      const item = this.currentFolder?.items.find((x) => x.id === id);
      if (item) {
        item.name = data.name;
      }
      return apiResponse.result;
    },
    async _renameFolder(id: string, data: any) {
      const apiResponse = await this.folderService.rename(id, data);
      const item = this.currentFolder?.items.find((x) => x.id === id);
      if (item) {
        item.name = data.name;
      }
      return apiResponse.result;
    },
    async upload(files: []) {
      if (files.length == 0) return;
      this.isUploading = true;
      try {
        const apiResponse = await this.fileService.upload(this.currentFolderId, files);
        await this.getItems(this.currentFolderId);
        return apiResponse.result;
      } finally {
        this.isUploading = false;
      }
    },
    async getItems(folderId: string) {
      if (folderId == this.selectedFolderId) return;
      this.isLoading = true;
      try {
        const apiResponse = await this.folderService.get(folderId);
        this.viewMode = "Home";
        this.currentFolder = apiResponse.result;
        this.selectedFileIds = [];
        this.selectedFolderId = undefined;
        this.currentFolderId = folderId;
      } finally {
        this.isLoading = false;
      }
    }
  }
});

export default useFileManagerStore;
