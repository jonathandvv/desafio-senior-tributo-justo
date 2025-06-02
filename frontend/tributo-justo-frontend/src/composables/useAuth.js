import { ref } from 'vue'

const token = ref(localStorage.getItem('token') || null)

export function useAuth() {
  function setToken(novoToken) {
    token.value = novoToken
    localStorage.setItem('token', novoToken)
  }

  function getToken() {
    if (!token.value) {
      token.value = localStorage.getItem('token')
    }
    return token.value
  }

  function limparToken() {
    token.value = null
    localStorage.removeItem('token')
  }

  return {
    setToken,
    getToken,
    limparToken,
  }
}
