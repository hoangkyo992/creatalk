<script setup lang="ts">
import { ref, onBeforeMount, watch } from "vue";
import { useRoute } from "vue-router";
import { useLayout } from "@/composables/appLayout";
import profileStore from "@/stores/profileStore";

const store = profileStore();
const route = useRoute();
const { layoutConfig, layoutState, setActiveMenuItem, onMenuToggle } = useLayout();

const props = defineProps({
  item: {
    type: Object,
    default: () => ({})
  },
  index: {
    type: Number,
    default: 0
  },
  root: {
    type: Boolean,
    default: true
  },
  parentItemKey: {
    type: String,
    default: null
  }
});

const isActiveMenu = ref(false);
const itemKey = ref();

const checkActiveItem = (item) => {
  if (route.name === item.to?.name) return true;
  return item.items && item.items.filter((x) => checkActiveItem(x)).length > 0;
};

onBeforeMount(() => {
  itemKey.value = props.parentItemKey ? props.parentItemKey + "-" + props.index : String(props.index);

  if (!layoutConfig.activeMenuItem.value && props.parentItemKey && props.item && props.item.items) {
    if (checkActiveItem(props.item)) {
      setActiveMenuItem(itemKey.value);
    }
  }

  const activeItem = layoutConfig.activeMenuItem.value;
  isActiveMenu.value = activeItem === itemKey.value || activeItem ? activeItem.startsWith(itemKey.value + "-") : false;
});

watch(
  () => layoutConfig.activeMenuItem.value,
  (newVal: any) => {
    isActiveMenu.value = newVal === itemKey.value || newVal.startsWith(itemKey.value + "-");
  }
);
const itemClick = (event, item) => {
  if (item.disabled) {
    event.preventDefault();
    return;
  }

  const { overlayMenuActive, staticMenuMobileActive } = layoutState;
  if ((item.to || item.url) && (staticMenuMobileActive.value || overlayMenuActive.value)) {
    onMenuToggle();
  }
  if (item.command) {
    item.command({ originalEvent: event, item: item });
  }

  const foundItemKey = item.items ? (isActiveMenu.value ? props.parentItemKey : itemKey) : itemKey.value;
  setActiveMenuItem(foundItemKey);
};

const checkActiveRoute = (item) => {
  return route.name === item.to.name;
};
</script>

<template>
  <li :class="{ 'layout-root-menuitem': root, 'active-menuitem': isActiveMenu }">
    <div v-if="root && item.visible !== false && store.hasAccess(item.features)" class="layout-menuitem-root-text">{{ $t(item.label) }}</div>
    <a
      v-if="(!item.to || item.items) && item.visible !== false && store.hasAccess(item.features)"
      :href="item.url"
      :class="item.class"
      :target="item.target"
      tabindex="0"
      @click="itemClick($event, item)"
    >
      <i :class="item.icon" class="layout-menuitem-icon"></i>
      <span class="layout-menuitem-text">{{ $t(item.label) }}</span>
      <i v-if="item.items" class="pi pi-fw pi-angle-down layout-submenu-toggler text-base"></i>
    </a>
    <router-link
      v-if="item.to && !item.items && item.visible !== false && store.hasAccess(item.features)"
      :class="[item.class, { 'active-route': checkActiveRoute(item) }]"
      tabindex="0"
      :to="item.to"
      @click="itemClick($event, item)"
    >
      <i :class="item.icon" class="layout-menuitem-icon"></i>
      <span class="layout-menuitem-text">{{ $t(item.label) }}</span>
      <Badge v-if="item.badge > 0" severity="danger" rounded :value="item.badge" class="ml-6"></Badge>
      <i v-if="item.items" class="pi pi-fw pi-angle-down layout-submenu-toggler"></i>
    </router-link>
    <Transition v-if="item.items && item.visible !== false" name="layout-submenu">
      <ul v-show="root ? true : isActiveMenu" class="layout-submenu">
        <app-menu-item v-for="(child, i) in item.items" :key="child" :index="i" :item="child" :parent-item-key="itemKey" :root="false"></app-menu-item>
      </ul>
    </Transition>
  </li>
</template>
