import { useAuth } from './useAuth'

export async function apiFetch(url, options = {}) {
  const { getToken } = useAuth()
  const token = getToken()

  const headers = {
    ...options.headers,
    Authorization: `Bearer ${token}`,
    'Content-Type': 'application/json',
  }

  return fetch(url, { ...options, headers })
}
