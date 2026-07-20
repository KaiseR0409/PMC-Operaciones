import axios from "axios"

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "",
    headers: {
        "X-Analytics-Token": import.meta.env.VITE_ANALYTICS_TOKEN
    }
})

export default api
