<script setup>

import {
    Package2,
    Boxes,
    Droplets,
    Users,
    Database
} from "lucide-vue-next"


import CountUp from "vue-countup-v3"

defineProps({
    summary: Object
})


const formatVariation = (value) => {
    const rounded = Math.round(value)

    if (rounded > 0) {
        return `↑ +${rounded}%`
    }

    if (rounded < 0) {
        return `↓ ${rounded}%`
    }

    return "0%"
}

</script>

<template>

    <div class="
    grid
    grid-cols-1
    md:grid-cols-2
    xl:grid-cols-5
    gap-6
    mb-10
  ">

        <!-- SACOS -->

        <div class="
            relative
            overflow-hidden
            bg-gradient-to-br
            from-purple-600/10
            to-purple-900/5
            border
            border-purple-500/20
            rounded-3xl
            p-6
            transition-all
            hover:scale-[1.02]
            hover:border-purple-400/40
            shadow-xl
        ">

            <div class="
                absolute
                top-0
                right-0
                w-32
                h-32
                bg-purple-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-purple-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Package2 class="w-8 h-8 text-purple-400" />
            </div>

            <h3 class="text-gray-400 text-sm mb-2">
                Total Toneladas Sacos
            </h3>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_sacos || 0" :duration="2"
                    :options="{ separator: '.', decimalPlaces: 0 }"
                    class="text-4xl font-bold text-white" />
                <span class="text-xl text-purple-300 mb-1">
                    T
                </span>
            </div>
            <p class="text-sm mt-2 font-medium" :class="summary?.variations?.sacos >= 0
                ? 'text-green-400'
                : 'text-red-400'
                ">
                {{ formatVariation(summary?.variations?.sacos || 0) }}
            </p>


        </div>

        <!-- MAXISACOS -->

        <div class="
                relative
                overflow-hidden
                bg-gradient-to-br
                from-cyan-600/10
                to-cyan-900/5
                border
                border-cyan-500/20
                rounded-3xl
                p-6
                transition-all
                hover:scale-[1.02]
                hover:border-cyan-400/40
                shadow-xl
            ">

            <div class="
                absolute
                top-0
                right-0
                w-32
                h-32
                bg-cyan-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-cyan-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Boxes class="w-7 h-7 text-cyan-400" />
            </div>

            <h3 class="text-gray-400 text-sm mb-2">
                Total Toneladas Maxisacos
            </h3>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_maxisacos || 0" :duration="2"
                    :options="{ separator: '.', decimalPlaces: 0 }"
                    class="text-4xl font-bold text-white" />

                <span class="text-xl text-purple-300 mb-1">
                    T
                </span>
            </div>
            <p class="text-sm mt-2 font-medium" :class="summary?.variations?.maxisacos >= 0
                ? 'text-green-400'
                : 'text-red-400'
                ">
                {{ formatVariation(summary?.variations?.maxisacos || 0) }}
            </p>
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
            transition-all
            hover:scale-[1.02]
            hover:border-emerald-400/40
            shadow-xl
        ">

            <div class="
                absolute
                top-0
                right-0
                w-32
                h-32
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

            <h3 class="text-gray-400 text-sm mb-2">
                Total Toneladas Granel
            </h3>

            <div class="flex items-end gap-2">
                <CountUp :end-val="summary.total_granel || 0" :duration="2"
                    :options="{ separator: '.', decimalPlaces: 0 }"
                    class="text-4xl font-bold text-white" />

                <span class="text-xl text-emerald-300 mb-1">
                    T
                </span>
            </div>
            <p class="text-sm mt-2 font-medium" :class="summary?.variations?.granel >= 0
                ? 'text-green-400'
                : 'text-red-400'
                ">
                {{ formatVariation(summary?.variations?.granel || 0) }}
            </p>
        </div>

        <!-- CLIENTES -->

        <div class="
            relative
            overflow-hidden
            bg-gradient-to-br
            from-green-600/10
            to-green-900/5
            border
            border-green-500/20
            rounded-3xl
            p-6
            transition-all
            hover:scale-[1.02]
            hover:border-green-400/40
            shadow-xl
        ">

            <div class="
                absolute
                top-0
                right-0
                w-32
                h-32
                bg-green-500/10
                blur-3xl
                rounded-full
            " />

            <div class="
                w-14
                h-14
                rounded-2xl
                bg-green-500/20
                flex
                items-center
                justify-center
                mb-4
            ">
                <Users class="w-7 h-7 text-green-400" />
            </div>

            <h3 class="text-gray-400 text-sm mb-2">
                Clientes Activos
            </h3>

            <CountUp :end-val="summary.clientes_activos || 0" :duration="2" class="text-4xl font-bold text-white" />

            <p class="text-sm mt-2 font-medium" :class="summary?.variations?.clientes >= 0
                ? 'text-green-400'
                : 'text-red-400'
                ">
                {{ formatVariation(summary?.variations?.clientes || 0) }}
            </p>

        </div>

        <!-- REGISTROS -->

        <div class="
      relative
      overflow-hidden
      bg-gradient-to-br
      from-yellow-600/10
      to-yellow-900/5
      border
      border-yellow-500/20
      rounded-3xl
      p-6
      transition-all
      hover:scale-[1.02]
      hover:border-yellow-400/40
      shadow-xl
    ">

            <div class="
        absolute
        top-0
        right-0
        w-32
        h-32
        bg-yellow-500/10
        blur-3xl
        rounded-full
      " />

            <div class="
        w-14
        h-14
        rounded-2xl
        bg-yellow-500/20
        flex
        items-center
        justify-center
        mb-4
      ">
                <Database class="w-7 h-7 text-yellow-400" />
            </div>

            <h3 class="text-gray-400 text-sm mb-2">
                Registros Totales
            </h3>

            <CountUp :end-val="summary.registros_totales || 0" :duration="2" class="text-4xl font-bold text-white" />

            <p class="text-sm mt-2 font-medium" :class="summary?.variations?.registros >= 0
                ? 'text-green-400'
                : 'text-red-400'
                ">
                {{ formatVariation(summary?.variations?.registros || 0) }}
            </p>



        </div>

    </div>

</template>