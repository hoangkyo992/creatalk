<template>
  <div class="file-manager-dialog">
    <div class="file-manager-dialog--mask">
      <slot name="mask" />
    </div>
    <div class="file-manager-dialog--content">
      <slot class="p-card-in-dialog" />
    </div>
  </div>
</template>

<script lang="ts" setup></script>

<style scoped lang="scss">
.file-manager-dialog {
  position: absolute;
  top: 0px;
  left: 0px;
  width: 100%;
  height: 100%;
  z-index: 9999;
  border-radius: 6px;
  overflow: hidden;

  .file-manager-dialog--mask {
    display: block;
    position: absolute;
    z-index: -1;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.3);
  }

  .file-manager-dialog--content {
    display: flex;
    justify-content: center;
    align-items: center;
    text-align: center;
    min-height: 100%;
    z-index: 9999;
  }
}
</style>
