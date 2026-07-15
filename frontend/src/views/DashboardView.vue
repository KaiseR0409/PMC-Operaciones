<script setup>
import { computed, ref, watch } from "vue"
import { toast } from "vue-sonner"
import { shallowRef } from "vue"

import { VueDatePicker } from "@vuepic/vue-datepicker"
import "@vuepic/vue-datepicker/dist/main.css"
import { es } from "date-fns/locale"

import { useDashboard } from "../composables/useDashboard"

import FiltersBar from "../components/FiltersBar.vue"
import PivotTable from "../components/PivotTable.vue"
import SummaryCards from "../components/SummaryCards.vue"
import ClientSummaryCards from "../components/ClientSummaryCards.vue"
import LineChart from "../components/LineChart.vue"
import LoadingOverlay from "../components/ui/LoadingOverlay.vue"

const {
  globalYear,
  periodMonth,
  selectedClient,
  selectedProduct,
  productFilter,
  currentFilters,
  selectedYear,
  periodLabel,
  isLoading,
  isFetching,
  summary,
  clients,
  clientSummary,
  tableData,
  lineChartData,
  truckChartData,
  handleFiltersChanged,
  summaryQuery,
  clientsQuery,
  clientSummaryQuery,
  tableQuery,
  lineChartQuery,
  truckChartQuery
} = useDashboard()

const tableDataRef = shallowRef([])
const lineChartDataRef = shallowRef(null)
const truckChartDataRef = shallowRef(null)

const debounce = (callback, delay = 1000) => {
  let timeoutId
  return (...args) => {
    window.clearTimeout(timeoutId)
    timeoutId = window.setTimeout(() => callback(...args), delay)
  }
}

const debouncedFiltersChanged = debounce(handleFiltersChanged)

const errors = ref([])

function trackError(query, message) {
  const err = query.error.value
  if (err?.code !== "ERR_CANCELED" && err?.name !== "CanceledError") {
    console.error(err)
    errors.value = [message]
    toast.error(message)
  }
}

watch(() => summaryQuery.error.value, (err) => {
  if (err) trackError(summaryQuery, "No se pudo actualizar el resumen general.")
})

watch(() => clientsQuery.error.value, (err) => {
  if (err) trackError(clientsQuery, "No se pudo actualizar la lista de clientes.")
})

watch(() => clientSummaryQuery.error.value, (err) => {
  if (err) trackError(clientSummaryQuery, "No se pudo actualizar el resumen del cliente.")
})

watch(() => tableQuery.error.value, (err) => {
  if (err) trackError(tableQuery, "No se pudo actualizar la tabla.")
})

watch(() => lineChartQuery.error.value, (err) => {
  if (err) trackError(lineChartQuery, "No se pudo actualizar el grafico de tonelaje.")
})

watch(() => truckChartQuery.error.value, (err) => {
  if (err) trackError(truckChartQuery, "No se pudo actualizar el grafico de camiones.")
})

const tableQueryData = tableQuery.data
const lineChartQueryData = lineChartQuery.data
const truckChartQueryData = truckChartQuery.data

if (tableQueryData) {
  tableQueryData.watch((val) => {
    tableDataRef.value = val || []
  }, { immediate: true })
}

if (lineChartQueryData) {
  lineChartQueryData.watch((val) => {
    lineChartDataRef.value = val || null
  }, { immediate: true })
}

if (truckChartQueryData) {
  truckChartQueryData.watch((val) => {
    truckChartDataRef.value = val || null
  }, { immediate: true })
}
</script>

<template>
  <div class="
      min-h-screen
      bg-gray-950
      text-white
      flex
      justify-center
      items-start
      p-10
    ">
    <div class="
        w-full
        max-w-7xl
        bg-gray-900
        rounded-3xl
        border
        border-gray-800
        shadow-2xl
        p-10
      ">
      <h1 class="
          text-4xl
          font-bold
          text-purple-400
          mb-10
          text-center
        ">
        Dashboard de Tonelaje | Reloncavi
      </h1>

      <div class="
          mb-8
          rounded-2xl
          border
          border-purple-500/20
          bg-purple-500/10
          px-5
          py-4
          text-center
          text-sm
          text-purple-100
        ">
        Mostrando datos de {{ periodLabel }}.
        <span v-if="isFetching" class="text-purple-200">
          Actualizando...
        </span>
      </div>

      <div class="
          flex
          justify-center
          items-center
          gap-4
          mb-8
        ">
        <p class="
            text-gray-300
            font-medium
        ">
          Filtrar ano global:
        </p>

        <VueDatePicker
          v-model="globalYear"
          :locale="es"
          year-picker
          dark
          placeholder="Ano"
          :year-range="[2020, 2030]"
          :start-date="new Date()"
          auto-apply
        />
      </div>

      <div class="relative">
        <SummaryCards :summary="summary || {}" />
        <LoadingOverlay
          v-if="summaryQuery.isLoading.value"
          message="Actualizando resumen..."
        />
      </div>

      <Transition name="fade-slide">
        <div v-if="selectedClient" class="relative">
          <ClientSummaryCards
            :summary="clientSummary || {}"
            :client="selectedClient"
          />
          <LoadingOverlay
            v-if="clientSummaryQuery.isLoading.value"
            message="Actualizando cliente..."
          />
        </div>
      </Transition>

      <div class="
          flex
          flex-col
          gap-6
          mb-10
          items-center
          justify-center
        ">
        <FiltersBar
          :enabled="!clientsQuery.isLoading.value"
          :clients="clients || []"
          @filters-changed="debouncedFiltersChanged"
        />
      </div>

      <div class="relative">
        <PivotTable :rows="tableDataRef" />
        <LoadingOverlay
          v-if="tableQuery.isLoading.value"
          message="Actualizando tabla..."
        />
      </div>

      <div class="
          flex
          items-center
          gap-4
          mt-8
        ">
        <p class="
            text-gray-300
            font-medium
        ">
          Seleccionar producto:
        </p>

        <select v-model="productFilter" class="
            bg-gray-800
            border
            border-gray-700
            rounded-xl
            px-4
            py-2
            text-white
            focus:outline-none
            focus:ring-2
            focus:ring-purple-500
          ">
          <option value="">Todos</option>
          <option value="SACOS">SACOS</option>
          <option value="MAXISACOS">MAXISACOS</option>
        </select>
      </div>

      <div v-if="lineChartDataRef || lineChartQuery.isLoading.value" class="relative">
        <LineChart
          v-if="lineChartDataRef"
          :chartData="lineChartDataRef"
          :selectedProduct="selectedProduct"
        />

        <div v-else class="
            bg-white/5
            border
            border-white/10
            rounded-3xl
            p-6
            mt-10
            h-[500px]
          " />

        <LoadingOverlay
          v-if="lineChartQuery.isLoading.value"
          message="Actualizando grafico..."
        />
      </div>

      <div v-if="truckChartDataRef || truckChartQuery.isLoading.value" class="relative">
        <LineChart
          v-if="truckChartDataRef"
          :chartData="truckChartDataRef"
          selectedProduct="Camiones"
        />

        <div v-else class="
            bg-white/5
            border
            border-white/10
            rounded-3xl
            p-6
            mt-10
            h-[500px]
          " />

        <LoadingOverlay
          v-if="truckChartQuery.isLoading.value"
          message="Actualizando camiones..."
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.35s ease;
}

.fade-slide-enter-from,
.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
</style>
