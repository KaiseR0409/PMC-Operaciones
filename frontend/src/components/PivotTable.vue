<script setup>
import { ref, computed, watch } from "vue"

const props = defineProps({
  rows: Array
})

const currentPage = ref(1)
const rowsPerPage = 10

const totalPages = computed(() => {
  return Math.ceil((props.rows?.length || 0) / rowsPerPage)
})

watch(
  () => props.rows,
  () => {
    currentPage.value = 1
  }
)

const paginatedRows = computed(() => {
  const start = (currentPage.value - 1) * rowsPerPage
  const end = start + rowsPerPage
  return (props.rows || []).slice(start, end)
})
</script>

<template>
  <div class="
      overflow-x-auto
      rounded-2xl
      border
      border-gray-800
    ">
    <table class="
        w-full
        border-collapse
      ">
      <thead class="
          bg-gray-800
          text-purple-300
        ">
        <tr>
          <th class="p-4 text-left">Fecha</th>
          <th class="p-4 text-left">Dia</th>
          <th class="p-4 text-left">Turno</th>
          <th class="p-4 text-left">SACOS</th>
          <th class="p-4 text-left">MAXISACOS</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="(row, index) in paginatedRows" :key="index" class="
            border-t
            border-gray-800
            hover:bg-gray-800
            transition
          ">
          <td class="p-4">{{ row.Fecha }}</td>
          <td class="p-4">{{ row["Dia Semana"] }}</td>
          <td class="p-4">{{ row.Turno }}</td>
          <td class="p-4">{{ row.SACOS || 0 }}</td>
          <td class="p-4">{{ row.MAXISACOS || 0 }}</td>
        </tr>
      </tbody>
    </table>

    <div v-if="totalPages > 1" class="
        flex
        justify-center
        items-center
        gap-4
        mt-6
    ">
      <button @click="currentPage--" :disabled="currentPage === 1" class="
            px-4
            py-2
            rounded-xl
            bg-white/5
            border
            border-white/10
            hover:bg-white/10
            disabled:opacity-40
        ">
        Anterior
      </button>

      <span class="text-gray-300">
        Pagina {{ currentPage }} de {{ totalPages }}
      </span>

      <button @click="currentPage++" :disabled="currentPage === totalPages" class="
            px-4
            py-2
            rounded-xl
            bg-purple-600
            hover:bg-purple-500
            disabled:opacity-40
        ">
        Siguiente
      </button>
    </div>
  </div>
</template>
