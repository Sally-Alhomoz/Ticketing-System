import axios from 'axios'

const API_BASE_URL = 'https://localhost:7100';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 12000
})


api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(error)
})

export default api
