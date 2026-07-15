<script setup>
defineProps({
  message: {
    type: String,
    default: "Cargando..."
  }
})
</script>

<template>
  <div class="
    absolute
    inset-0
    rounded-2xl
    bg-gray-950/40
    backdrop-blur-[1px]
    flex
    items-center
    justify-center
    text-sm
    text-purple-100
    z-10
  ">
    {{ message }}
  </div>
</template>
