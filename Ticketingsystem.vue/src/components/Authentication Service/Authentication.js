import { ref, computed } from 'vue'
import api from '@/components/Authentication Service/AuthAPI'; 

const currentUser = ref(null)

const decodeToken = (token) => {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))

    // Check if token is expired
    const now = Math.floor(Date.now() / 1000)
    if (payload.exp && payload.exp < now) {
      console.warn("Token expired")
      return null
    }

    return {
      username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload['unique_name'] || payload['name'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload['role'],
      id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
    }
  } catch (e) {
    return null
  }
}

export const initAuth = () => {
  const token = localStorage.getItem('token')
  if (token) {
    const user = decodeToken(token)
    if (user) {
      currentUser.value = user
    } else {
      localStorage.removeItem('token') 
    }
  }
}

export const useAuth = () => {
  const login = (token) => {
    localStorage.setItem('token', token)
    const user = decodeToken(token)
    currentUser.value = user
  }

  const logout = async () => {
    try {
      await api.post('/api/Account/logout')
    } catch (err) {
      console.warn('Backend logout failed:', err.message)
    } finally {
      localStorage.removeItem('token')
      currentUser.value = null
    }
  }

  return {
    isAuthenticated: computed(() => !!currentUser.value),
    currentUsername: computed(() => currentUser.value?.username || ''),
    UserRole: computed(() => currentUser.value?.role || ''),
    isAdmin: computed(() => ['Admin', '1', 1].includes(currentUser.value?.role)),
    login,
    logout
  }
}
