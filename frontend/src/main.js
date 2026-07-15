import { createApp } from "vue"
import { VueQueryPlugin } from "@tanstack/vue-query"
import App from "./App.vue"
import "./assets/main.css"

const app = createApp(App)

app.use(VueQueryPlugin, {
  queryClientConfig: {
    defaultOptions: {
      queries: {
        refetchOnWindowFocus: false,
        retry: 1,
        staleTime: 5 * 60 * 1000
      }
    }
  }
})

app.mount("#app")
