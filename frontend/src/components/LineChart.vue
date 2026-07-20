<script setup>
import {
    Line
} from "vue-chartjs"

import {
    Chart,
    Title,
    Tooltip,
    Legend,
    LineElement,
    CategoryScale,
    LinearScale,
    PointElement
} from "chart.js"

const props = defineProps({
    chartData: Object,
    selectedProduct: String
})

const weekSeparatorPlugin = {
    id: "weekSeparator",

    afterDraw(chart) {
        const weeks = props.chartData?.weeks
        const showWeeks = props.chartData?.showWeeks

        if (!showWeeks || !weeks || weeks.length === 0) {
            return
        }

        const { ctx, chartArea, scales } = chart
        if (!chartArea) return

        const groups = []
        let currentWeek = weeks[0]
        let startIdx = 0

        for (let i = 1; i < weeks.length; i++) {
            if (weeks[i] !== currentWeek) {
                groups.push({ week: currentWeek, start: startIdx, end: i - 1 })
                currentWeek = weeks[i]
                startIdx = i
            }
        }
        groups.push({ week: currentWeek, start: startIdx, end: weeks.length - 1 })

        ctx.save()

        for (let i = 0; i < groups.length; i++) {
            const g = groups[i]
            const centerX = scales.x.getPixelForValue(g.start + (g.end - g.start) / 2)

            ctx.fillStyle = "#c084fc"
            ctx.font = "bold 12px sans-serif"
            ctx.textAlign = "center"
            ctx.fillText(`S${g.week}`, centerX, chartArea.top - 8)

            if (i > 0) {
                const lineX = scales.x.getPixelForValue(g.start)
                ctx.strokeStyle = "rgba(255,255,255,0.15)"
                ctx.lineWidth = 1
                ctx.setLineDash([4, 4])
                ctx.beginPath()
                ctx.moveTo(lineX, chartArea.top)
                ctx.lineTo(lineX, chartArea.bottom)
                ctx.stroke()
                ctx.setLineDash([])
            }
        }

        ctx.restore()
    }
}

const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,

    layout: {
        padding: {
            top: 20
        }
    },

    plugins: {
        tooltip: {
            callbacks: {
                label: (context) => {
                    const value = Math.round(context.parsed.y).toLocaleString("es-CL")

                    return `${context.dataset.label}: ${value}`
                }
            }
        },

        legend: {
            labels: {
                color: "white",
            },
            padding: {
                top: 30
            }
        }
    },

    scales: {
        x: {
            ticks: {
                color: "white",
                maxRotation: 45,
                minRotation: 45,
                maxTicksLimit: 20
            },

            grid: {
                color: "rgba(255,255,255,0.05)"
            }
        },

        y: {
            ticks: {
                color: "white"
            },
            grid: {
                color: "rgba(255,255,255,0.05)"
            }
        }
    }
}

Chart.register(
    Title,
    Tooltip,
    Legend,
    LineElement,
    CategoryScale,
    LinearScale,
    PointElement,
    weekSeparatorPlugin
)
</script>

<template>
    <div class="
      bg-white/5
      border
      border-white/10
      rounded-3xl
      p-6
      mt-10
      h-[500px]
  ">
        <h2 class="
        text-2xl
        font-bold
        text-white
        mb-6
    ">
            Gráfica de
            {{ selectedProduct || "Despachos" }}
        </h2>

        <Line :data="chartData" :options="chartOptions" />
    </div>
</template>
