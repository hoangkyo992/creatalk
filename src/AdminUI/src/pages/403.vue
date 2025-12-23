<route lang="yaml">
meta:
  layout: empty
</route>

<template>
  <div class="surface-ground flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden">
    <div class="flex flex-column align-items-center justify-content-center">
      <img src="@/assets/vikiworld.svg" alt="Vikiworld logo" class="mb-5 w-5rem flex-shrink-0" />
      <div style="border-radius: 56px; padding: 0.3rem; background: linear-gradient(180deg, rgba(33, 150, 243, 0.4) 10%, rgba(33, 150, 243, 0) 30%)">
        <div class="w-full h-full surface-card py-8 px-5 sm:px-8 flex flex-column align-items-center" style="border-radius: 53px">
          <span class="text-blue-600 font-bold text-5xl">403</span>
          <h1 class="text-900 font-bold text-3xl mb-4">
            {{ $t("PageUnauthorized.Title.Header") }}
          </h1>
          <div class="text-600 mb-5">
            {{ $t("PageUnauthorized.Description.Header") }}
          </div>
          <router-link to="/" class="w-full flex align-items-center py-5 px-8 border-300 border-bottom-1 h-5rem">
            <span class="flex justify-content-center align-items-center bg-cyan-400 border-round" style="height: 3.5rem; width: 3.5rem">
              <i class="text-50 pi pi-fw pi-home text-2xl" />
            </span>
            <span class="ml-8 flex flex-column">
              <span class="text-900 font-medium mb-0 block">{{ $t("PageUnauthorized.Title.HomePage") }}</span>
              <span class="text-600">{{ $t("PageUnauthorized.Description.HomePage") }}</span>
            </span>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup></script>
