<template>
  <Breadcrumb v-bind="$attrs">
    <template #item="{ item, props }">
      <router-link v-if="item.to" v-slot="{ href, navigate }" :to="item.to" custom>
        <a :href="href" v-bind="props.action" @click="navigate">
          <span :class="[item.icon, 'text-primary', 'p-menuitem-icon']" />
          <span class="text-primary font-semibold">{{ item.label }}</span>
        </a>
      </router-link>
      <div v-else>
        <span :class="[item.icon, 'text-primary', 'p-menuitem-icon']" />
        <span class="text-primary font-semibold">{{ item.label }}</span>
      </div>
    </template>
  </Breadcrumb>
</template>
<script setup lang="ts"></script>
