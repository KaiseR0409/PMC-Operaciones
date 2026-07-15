<script setup>

import {
    Package2,
    Boxes,
    Droplets
} from "lucide-vue-next"

import CountUp from "vue-countup-v3"

defineProps({
    summary: Object,
    client: String,
})

</script>

<template>

    <div v-if="client" class="
            grid
            grid-cols-1
            md:grid-cols-3
            gap-6
            mb-8
        ">

        <!-- SACOS -->

        <div class="
            relative
            overflow-hidden
            bg-gradient-to-br
            from-pink-600/10
            to-pink-900/5
            border
            border-pink-500/20
            rounded-3xl
            p-6
            shadow-xl
        ">
            <div class="
                absolute
                top-0
                right-0
                w-28
                h-28
                bg-pink-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-pink-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Package2 class="w-7 h-7 text-pink-400" />
            </div>

            <p class="text-gray-400 text-sm mb-2">
                Total Toneladas Sacos de {{ client }}
            </p>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_sacos || 0" :duration="2"
                    :options="{ decimal: ',', separator: '.', decimalPlaces: 1 }"
                    class="text-4xl font-bold text-white" />

                <span class="text-lg text-pink-300 mb-1">
                    T
                </span>
            </div>
        </div>

        <!-- MAXISACOS -->

        <div class="
            relative
            overflow-hidden
            bg-gradient-to-br
            from-sky-600/10
            to-sky-900/5
            border
            border-sky-500/20
            rounded-3xl
            p-6
            shadow-xl
        ">
            <div class="
                absolute
                top-0
                right-0
                w-28
                h-28
                bg-sky-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-sky-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Boxes class="w-7 h-7 text-sky-400" />
            </div>

            <p class="text-gray-400 text-sm mb-2">
                Total Toneladas Maxisacos {{ client }}
            </p>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_maxisacos || 0" :duration="2"
                    :options="{ decimal: ',', separator: '.', decimalPlaces: 1 }"
                    class="text-4xl font-bold text-white" />

                <span class="text-lg text-sky-300 mb-1">
                    T
                </span>
            </div>
        </div>

        <!-- GRANEL -->

        <div class="
            relative
            overflow-hidden
            bg-gradient-to-br
            from-emerald-600/10
            to-emerald-900/5
            border
            border-emerald-500/20
            rounded-3xl
            p-6
            shadow-xl
        ">
            <div class="
                absolute
                top-0
                right-0
                w-28
                h-28
                bg-emerald-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-emerald-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Droplets class="w-7 h-7 text-emerald-400" />
            </div>

            <p class="text-gray-400 text-sm mb-2">
                Total Toneladas Granel {{ client }}
            </p>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_granel || 0" :duration="2"
                    :options="{ decimal: ',', separator: '.', decimalPlaces: 1 }"
                    class="text-4xl font-bold text-white" />

                <span class="text-lg text-emerald-300 mb-1">
                    T
                </span>
            </div>
        </div>

    </div>

</template>
