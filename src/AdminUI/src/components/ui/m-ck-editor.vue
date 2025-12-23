<template>
  <div class="m-ckeditor-wrapper">
    <ckeditor v-model="model" :editor="editor" v-bind="$attrs" :config="editorConfig"></ckeditor>
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import "ckeditor5-custom-build/build/ckeditor";

interface Props {
  feeds?: string[];
}
const props = defineProps<Props>();
const model = defineModel<string>();

const editorConfig = computed(() => {
  return {
    removePlugins: ["MediaEmbedToolbar", "Title"],
    toolbar: [
      "undo",
      "redo",
      "|",
      "insertTable",
      "specialCharacters",
      "blockQuote",
      "codeBlock",
      "heading",
      "|",
      "bold",
      "italic",
      "underline",
      "strikethrough",
      "alignment",
      "bulletedList",
      "numberedList",
      "todoList",
      "horizontalLine",
      "|",
      "outdent",
      "indent"
    ],
    heading: {
      options: [
        { model: "paragraph", title: "Paragraph", class: "ck-heading_paragraph" },
        { model: "heading1", view: "h1", title: "Heading 1", class: "ck-heading_heading1" },
        { model: "heading2", view: "h2", title: "Heading 2", class: "ck-heading_heading2" },
        { model: "heading3", view: "h3", title: "Heading 3", class: "ck-heading_heading3" },
        { model: "heading4", view: "h4", title: "Heading 4", class: "ck-heading_heading4" },
        { model: "heading5", view: "h5", title: "Heading 5", class: "ck-heading_heading5" }
      ]
    },
    mention: { feeds: [{ marker: "@", feed: props.feeds?.map((x) => `@${x}`), minimumCharacters: 1 }] }
  } as any;
});

const editor = (window as any).ClassicEditor;
</script>
<style lang="scss" scoped>
.m-ckeditor-wrapper {
  &.sm {
    ::v-deep(.ck-editor) {
      .ck-content {
        min-height: 120px;
      }
    }
  }
  &.md {
    ::v-deep(.ck-editor) {
      .ck-content {
        min-height: 240px;
      }
    }
  }
  ::v-deep(.ck-editor) {
    .ck-content {
      min-height: 180px;
    }
  }
}
</style>
