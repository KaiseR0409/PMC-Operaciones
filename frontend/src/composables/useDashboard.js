import { useQuery } from "@tanstack/vue-query"
import { computed, ref, watch } from "vue"
import api from "../api/analytics"

export function useDashboard() {
  const today = new Date()
  const currentMonth = today.getMonth() + 1

  const globalYear = ref(today.getFullYear())
  const periodMonth = ref(currentMonth)
  const selectedClient = ref("")
  const selectedProduct = ref("")
  const productFilter = ref("")
  const currentFilters = ref({})

  const selectedYear = computed(() => {
    const value = globalYear.value
    if (typeof value === "number") return value
    if (value instanceof Date) return value.getFullYear()
    return today.getFullYear()
  })

  const periodLabel = computed(() => {
    const periodDate = new Date(selectedYear.value, periodMonth.value - 1, 1)
    return new Intl.DateTimeFormat("es-CL", {
      month: "long",
      year: "numeric"
    }).format(periodDate)
  })

  const queryKey = computed(() => ({
    year: selectedYear.value,
    month: periodMonth.value
  }))

  const summaryQuery = useQuery({
    queryKey: computed(() => ["summary", queryKey.value]),
    queryFn: async ({ signal }) => {
      const { data } = await api.get("/analytics/summary", {
        params: { year: queryKey.value.year, month: queryKey.value.month },
        signal
      })
      return data
    },
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const clientsQuery = useQuery({
    queryKey: computed(() => ["clients", queryKey.value]),
    queryFn: async ({ signal }) => {
      const { data } = await api.get("/analytics/clients", {
        params: { year: queryKey.value.year, month: queryKey.value.month },
        signal
      })
      return data
    },
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const clientSummaryQuery = useQuery({
    queryKey: computed(() => [
      "clientSummary",
      queryKey.value,
      selectedClient.value
    ]),
    queryFn: async ({ signal }) => {
      if (!selectedClient.value) return null
      const { data } = await api.get("/analytics/summary", {
        params: {
          client: selectedClient.value,
          year: queryKey.value.year,
          month: queryKey.value.month
        },
        signal
      })
      return data
    },
    enabled: computed(() => !!selectedClient.value),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const tableQuery = useQuery({
    queryKey: computed(() => [
      "clientTable",
      queryKey.value,
      selectedClient.value,
      currentFilters.value.turno || ""
    ]),
    queryFn: async ({ signal }) => {
      if (!selectedClient.value) return []
      const { data } = await api.get("/analytics/client-table", {
        params: {
          client: selectedClient.value,
          year: queryKey.value.year,
          month: queryKey.value.month,
          turno: currentFilters.value.turno || undefined
        },
        signal
      })
      return data
    },
    enabled: computed(() => !!selectedClient.value),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const lineChartQuery = useQuery({
    queryKey: computed(() => [
      "lineChart",
      queryKey.value,
      selectedClient.value,
      productFilter.value
    ]),
    queryFn: async ({ signal }) => {
      if (!selectedClient.value) return null
      const { data } = await api.get("/analytics/line-chart", {
        params: {
          client: selectedClient.value,
          product: productFilter.value || undefined,
          year: queryKey.value.year,
          month: queryKey.value.month
        },
        signal
      })

      if (!data || data.length === 0) return null

      selectedProduct.value = productFilter.value

      return {
        labels: data.map((item) => item.fecha),
        weeks: data.map((item) => item.semana),
        showWeeks: !!periodMonth.value,
        datasets: [
          {
            label: productFilter.value || "Tonelaje",
            data: data.map((item) => item.cantidad),
            borderColor: "#a855f7",
            backgroundColor: "rgba(168,85,247,0.2)",
            tension: 0.4,
            fill: true
          }
        ]
      }
    },
    enabled: computed(() => !!selectedClient.value),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const truckChartQuery = useQuery({
    queryKey: computed(() => [
      "truckChart",
      queryKey.value,
      selectedClient.value
    ]),
    queryFn: async ({ signal }) => {
      if (!selectedClient.value) return null
      const { data } = await api.get("/analytics/truck-chart", {
        params: {
          client: selectedClient.value,
          year: queryKey.value.year,
          month: queryKey.value.month
        },
        signal
      })

      if (!data || data.length === 0) return null

      return {
        labels: data.map((item) => item.fecha),
        weeks: data.map((item) => item.semana),
        showWeeks: !!periodMonth.value,
        datasets: [
          {
            label: "Camiones",
            data: data.map((item) => item.camiones),
            borderColor: "#22c55e",
            backgroundColor: "rgba(34,197,94,0.2)",
            tension: 0.4,
            fill: true
          }
        ]
      }
    },
    enabled: computed(() => !!selectedClient.value),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000
  })

  const isLoading = computed(
    () =>
      summaryQuery.isLoading.value ||
      clientsQuery.isLoading.value ||
      clientSummaryQuery.isLoading.value ||
      tableQuery.isLoading.value ||
      lineChartQuery.isLoading.value ||
      truckChartQuery.isLoading.value
  )

  const isFetching = computed(
    () =>
      summaryQuery.isFetching.value ||
      clientsQuery.isFetching.value ||
      clientSummaryQuery.isFetching.value ||
      tableQuery.isFetching.value ||
      lineChartQuery.isFetching.value ||
      truckChartQuery.isFetching.value
  )

  function handleFiltersChanged(filters) {
    const nextFilters = {
      client: filters.client || "",
      month: filters.month,
      turno: filters.turno || ""
    }

    const previousFilters = currentFilters.value

    currentFilters.value = nextFilters
    periodMonth.value = nextFilters.month || currentMonth

    if (nextFilters.client !== (previousFilters.client || "")) {
      selectedClient.value = nextFilters.client
    }
  }

  watch(globalYear, () => {
    summaryQuery.refetch()
    clientsQuery.refetch()
    if (selectedClient.value) {
      clientSummaryQuery.refetch()
      tableQuery.refetch()
      lineChartQuery.refetch()
      truckChartQuery.refetch()
    }
  })

  return {
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
    summary: summaryQuery.data,
    clients: clientsQuery.data,
    clientSummary: clientSummaryQuery.data,
    tableData: tableQuery.data,
    lineChartData: lineChartQuery.data,
    truckChartData: truckChartQuery.data,
    handleFiltersChanged,
    summaryQuery,
    clientsQuery,
    clientSummaryQuery,
    tableQuery,
    lineChartQuery,
    truckChartQuery
  }
}
