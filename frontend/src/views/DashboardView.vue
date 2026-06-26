<script setup>
import { computed, onMounted, ref, watch } from "vue"

import api from "../api/analytics"
import { VueDatePicker } from "@vuepic/vue-datepicker"
import "@vuepic/vue-datepicker/dist/main.css"
import { es } from "date-fns/locale"

import FiltersBar from "../components/FiltersBar.vue"
import PivotTable from "../components/PivotTable.vue"
import SummaryCards from "../components/SummaryCards.vue"
import ClientSummaryCards from "../components/ClientSummaryCards.vue"
import LineChart from "../components/LineChart.vue"

const tableData = ref([])
const clients = ref([])
const summary = ref({})
const selectedClientSummary = ref({})
const selectedClient = ref("")
const lineChartData = ref(null)
const selectedProduct = ref("")
const productFilter = ref("")
const today = new Date()
const currentMonth = today.getMonth() + 1
const globalYear = ref(today.getFullYear())
const periodMonth = ref(currentMonth)
const currentFilters = ref({})
const truckChartData = ref(null)
const dashboardError = ref("")

const loadingSummary = ref(false)
const loadingClients = ref(false)
const loadingClientSummary = ref(false)
const loadingTable = ref(false)
const loadingLineChart = ref(false)
const loadingTruckChart = ref(false)

const requestControllers = {}

const loadingStates = {
  summary: loadingSummary,
  clients: loadingClients,
  clientSummary: loadingClientSummary,
  table: loadingTable,
  lineChart: loadingLineChart,
  truckChart: loadingTruckChart
}

const normalizeYear = (value) => {
  if (typeof value === "number") {
    return value
  }

  if (value instanceof Date) {
    return value.getFullYear()
  }

  return today.getFullYear()
}

const selectedYear = computed(() =>
  normalizeYear(globalYear.value)
)

const periodLabel = computed(() => {
  const periodDate = new Date(
    selectedYear.value,
    periodMonth.value - 1,
    1
  )

  return new Intl.DateTimeFormat(
    "es-CL",
    {
      month: "long",
      year: "numeric"
    }
  ).format(periodDate)
})

const isUpdating = computed(() =>
  loadingSummary.value ||
  loadingClients.value ||
  loadingClientSummary.value ||
  loadingTable.value ||
  loadingLineChart.value ||
  loadingTruckChart.value
)

const getRequestPeriod = (filters = {}) => ({
  year: selectedYear.value,
  month: filters.month || periodMonth.value || currentMonth
})

const debounce = (callback, delay = 1000) => {
  let timeoutId

  return (...args) => {
    window.clearTimeout(timeoutId)
    timeoutId = window.setTimeout(
      () => callback(...args),
      delay
    )
  }
}

const beginRequest = (key) => {
  requestControllers[key]?.abort()

  const controller = new AbortController()
  requestControllers[key] = controller
  loadingStates[key].value = true

  return controller
}

const finishRequest = (key, controller) => {
  if (requestControllers[key] !== controller) {
    return
  }

  loadingStates[key].value = false
  delete requestControllers[key]
}

const isCanceledRequest = (error) =>
  error?.code === "ERR_CANCELED" ||
  error?.name === "CanceledError"

const showRequestError = (error, message) => {
  if (isCanceledRequest(error)) {
    return
  }

  dashboardError.value = message
  console.error(error)
}

const clearClientData = () => {
  selectedClientSummary.value = {}
  lineChartData.value = null
  truckChartData.value = null
  tableData.value = []
  selectedProduct.value = ""
}

const fetchClients = async (filters = currentFilters.value) => {
  const controller = beginRequest("clients")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/clients",
      {
        params: {
          year: period.year,
          month: period.month
        },
        signal: controller.signal
      }
    )

    clients.value = response.data
  } catch (error) {
    showRequestError(error, "No se pudo actualizar la lista de clientes.")
  } finally {
    finishRequest("clients", controller)
  }
}

const fetchSummary = async (filters = currentFilters.value) => {
  const controller = beginRequest("summary")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/summary",
      {
        params: {
          year: period.year,
          month: period.month
        },
        signal: controller.signal
      }
    )

    summary.value = response.data
  } catch (error) {
    showRequestError(error, "No se pudo actualizar el resumen general.")
  } finally {
    finishRequest("summary", controller)
  }
}

const fetchClientSummary = async (filters = currentFilters.value) => {
  if (!filters.client) {
    selectedClientSummary.value = {}
    return
  }

  const controller = beginRequest("clientSummary")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/summary",
      {
        params: {
          client: filters.client,
          year: period.year,
          month: period.month
        },
        signal: controller.signal
      }
    )

    selectedClientSummary.value = response.data
  } catch (error) {
    showRequestError(error, "No se pudo actualizar el resumen del cliente.")
  } finally {
    finishRequest("clientSummary", controller)
  }
}

const fetchTruckChart = async (filters = currentFilters.value) => {
  if (!filters.client) {
    truckChartData.value = null
    return
  }

  const controller = beginRequest("truckChart")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/truck-chart",
      {
        params: {
          client: filters.client,
          year: period.year,
          month: period.month
        },
        signal: controller.signal
      }
    )

    truckChartData.value = {
      labels: response.data.map(item => item.fecha),
      weeks: response.data.map(item => item.semana),
      showWeeks: !!period.month,
      datasets: [
        {
          label: "Camiones",
          data: response.data.map(item => item.camiones),
          borderColor: "#22c55e",
          backgroundColor: "rgba(34,197,94,0.2)",
          tension: 0.4,
          fill: true
        }
      ]
    }
  } catch (error) {
    showRequestError(error, "No se pudo actualizar el grafico de camiones.")
  } finally {
    finishRequest("truckChart", controller)
  }
}

const fetchLineChart = async (filters = currentFilters.value) => {
  if (!filters.client) {
    lineChartData.value = null
    selectedProduct.value = ""
    return
  }

  const controller = beginRequest("lineChart")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/line-chart",
      {
        params: {
          client: filters.client,
          product: productFilter.value,
          year: period.year,
          month: period.month
        },
        signal: controller.signal
      }
    )

    lineChartData.value = {
      labels: response.data.map(item => item.fecha),
      weeks: response.data.map(item => item.semana),
      showWeeks: !!period.month,
      datasets: [
        {
          label: productFilter.value || "Tonelaje",
          data: response.data.map(item => item.cantidad),
          borderColor: "#a855f7",
          backgroundColor: "rgba(168,85,247,0.2)",
          tension: 0.4,
          fill: true
        }
      ]
    }

    selectedProduct.value = productFilter.value
  } catch (error) {
    showRequestError(error, "No se pudo actualizar el grafico de tonelaje.")
  } finally {
    finishRequest("lineChart", controller)
  }
}

const fetchClientTable = async (filters = currentFilters.value) => {
  if (!filters.client) {
    tableData.value = []
    return
  }

  const controller = beginRequest("table")
  const period = getRequestPeriod(filters)

  try {
    const response = await api.get(
      "/analytics/client-table",
      {
        params: {
          client: filters.client,
          year: period.year,
          month: period.month,
          turno: filters.turno || undefined
        },
        signal: controller.signal
      }
    )

    tableData.value = response.data
  } catch (error) {
    showRequestError(error, "No se pudo actualizar la tabla.")
  } finally {
    finishRequest("table", controller)
  }
}

const refreshClientData = async (filters = currentFilters.value) => {
  selectedClient.value = filters.client || ""

  if (!filters.client) {
    clearClientData()
    return
  }

  await Promise.all([
    fetchClientSummary(filters),
    fetchLineChart(filters),
    fetchTruckChart(filters),
    fetchClientTable(filters)
  ])
}

const refreshPeriodData = async (filters = currentFilters.value) => {
  dashboardError.value = ""

  await Promise.all([
    fetchSummary(filters),
    fetchClients(filters)
  ])

  await refreshClientData(filters)
}

const handleFiltersChanged = async (filters) => {
  const nextFilters = {
    client: filters.client || "",
    month: filters.month,
    turno: filters.turno || ""
  }

  const previousFilters = currentFilters.value
  const clientChanged = nextFilters.client !== (previousFilters.client || "")
  const monthChanged = nextFilters.month !== previousFilters.month
  const turnoChanged = nextFilters.turno !== (previousFilters.turno || "")

  currentFilters.value = nextFilters
  periodMonth.value = nextFilters.month || currentMonth
  dashboardError.value = ""

  if (monthChanged) {
    await refreshPeriodData(nextFilters)
    return
  }

  if (clientChanged) {
    await refreshClientData(nextFilters)
    return
  }

  if (turnoChanged) {
    await fetchClientTable(nextFilters)
  }
}

const handleFiltersChangedDebounced = debounce(handleFiltersChanged)

const loadInitialData = async () => {
  dashboardError.value = ""

  await Promise.all([
    fetchSummary(),
    fetchClients()
  ])
}

watch(productFilter, () => {
  fetchLineChart(currentFilters.value)
})

watch(globalYear, () => {
  refreshPeriodData(currentFilters.value)
})

onMounted(loadInitialData)
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
        Dashboard de Tonelaje | Reloncaví
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
        <span v-if="isUpdating" class="text-purple-200">
          Actualizando...
        </span>
      </div>

      <p v-if="dashboardError" class="
          mb-6
          text-center
          text-sm
          text-red-300
        ">
        {{ dashboardError }}
      </p>

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
          Filtrar año global:
        </p>

        <VueDatePicker
          v-model="globalYear"
          :locale="es"
          year-picker
          dark
          placeholder="Año"
          :year-range="[2020, 2030]"
          :start-date="new Date()"
          auto-apply
        />
      </div>

      <div class="relative">
        <SummaryCards :summary="summary" />

        <div v-if="loadingSummary" class="
            absolute
            inset-0
            rounded-3xl
            bg-gray-950/30
            backdrop-blur-[1px]
            flex
            items-start
            justify-center
            pt-4
            text-sm
            text-purple-100
          ">
          Actualizando resumen...
        </div>
      </div>

      <Transition name="fade-slide">
        <div v-if="selectedClient" class="relative">
          <ClientSummaryCards
            :summary="selectedClientSummary"
            :client="selectedClient"
          />

          <div v-if="loadingClientSummary" class="
              absolute
              inset-0
              rounded-3xl
              bg-gray-950/30
              backdrop-blur-[1px]
              flex
              items-start
              justify-center
              pt-4
              text-sm
              text-purple-100
            ">
            Actualizando cliente...
          </div>
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
          :enabled="!loadingClients"
          :clients="clients"
          @filters-changed="handleFiltersChangedDebounced"
        />
      </div>

      <div class="relative">
        <PivotTable :rows="tableData" />

        <div v-if="loadingTable" class="
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
          ">
          Actualizando tabla...
        </div>
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
          <option value="">
            Todos
          </option>

          <option value="SACOS">
            SACOS
          </option>

          <option value="MAXISACOS">
            MAXISACOS
          </option>
        </select>
      </div>

      <div v-if="lineChartData || loadingLineChart" class="relative">
        <LineChart
          v-if="lineChartData"
          :chartData="lineChartData"
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

        <div v-if="loadingLineChart" class="
            absolute
            inset-0
            rounded-3xl
            bg-gray-950/40
            backdrop-blur-[1px]
            flex
            items-center
            justify-center
            text-sm
            text-purple-100
          ">
          Actualizando grafico...
        </div>
      </div>

      <div v-if="truckChartData || loadingTruckChart" class="relative">
        <LineChart
          v-if="truckChartData"
          :chartData="truckChartData"
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

        <div v-if="loadingTruckChart" class="
            absolute
            inset-0
            rounded-3xl
            bg-gray-950/40
            backdrop-blur-[1px]
            flex
            items-center
            justify-center
            text-sm
            text-purple-100
          ">
          Actualizando camiones...
        </div>
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
