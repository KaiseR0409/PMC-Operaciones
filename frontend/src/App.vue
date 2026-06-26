<script setup>
import DashboardView from "./views/DashboardView.vue"
</script>

<template>
  <Toaster
    theme="dark"
    position="top-center"
    :expand="false"
    richColors
    :toastOptions="{
      style: {
        background: '#111827'
      }
    }"
  />

  <DashboardView />
</template>

<style scoped></style>
