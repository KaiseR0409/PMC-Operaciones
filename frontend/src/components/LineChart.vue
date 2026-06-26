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
        const weeks =
            props.chartData?.weeks

        const showWeeks =
            props.chartData?.showWeeks

        if (!showWeeks) {
            return
        }

        if (!weeks) {
            return
        }

        const uniqueWeeks = [
            ...new Set(weeks)
        ]

        if (uniqueWeeks.length <= 1) {
            return
        }

        const {
            ctx,
            chartArea,
            scales
        } = chart

        ctx.save()

        let previousWeek = weeks[0]

        for (
            let i = 1;
            i < weeks.length;
            i++
        ) {
            if (
                weeks[i] !== previousWeek
            ) {
                const x =
                    scales.x.getPixelForValue(i)

                ctx.strokeStyle =
                    "rgba(255,255,255,0.2)"

                ctx.beginPath()

                ctx.moveTo(
                    x,
                    chartArea.top
                )

                ctx.lineTo(
                    x,
                    chartArea.bottom
                )

                ctx.stroke()

                ctx.fillStyle =
                    "#c084fc"

                ctx.font =
                    "11px sans-serif"

                ctx.fillText(
                    `S${weeks[i]}`,
                    x + 5,
                    chartArea.top - 25
                )

                previousWeek =
                    weeks[i]
            }
        }

        ctx.restore()
    }
}

const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,

    plugins: {
        tooltip: {
            callbacks: {
                label: (context) => {
                    const value = Math.round(context.parsed.y)

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
