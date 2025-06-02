<template>
  <nav class="sidebar">
    <h1 class="logo">Tributo Justo</h1>

    <ul class="menu-links">
      <li :class="{ active: isActive('/pagina-inicial') }" @click="navegar('/pagina-inicial')">Inicial</li>
      <li :class="{ active: isActive('/upload') }" @click="navegar('/upload')">Upload</li>
      <li :class="{ active: isActive('/empresas-notas') }" @click="navegar('/empresas-notas')">Empresas & Notas</li>
      <li :class="{ active: isActive('/notas-inconsistentes') }" @click="navegar('/notas-inconsistentes')">Notas
        Inconsistentes</li>
      <li :class="{ active: isActive('/relatorio-ia') }" @click="navegar('/relatorio-ia')">Relatório IA</li>
      <li :class="{ active: isActive('/dashboard') }" @click="navegar('/dashboard')">Dashboard</li>
    </ul>

    <button class="btn-logout" @click="logout">Sair</button>
  </nav>
</template>

<script setup>
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

const emit = defineEmits(['logout'])
const router = useRouter()
const route = useRoute()
const { limparToken } = useAuth()

function navegar(caminho) {
  router.push(caminho)
}

function isActive(caminho) {
  return route.path === caminho
}

function logout() {
  limparToken()
  emit('logout')
  router.push('/')
}
</script>


<style scoped>
.sidebar {
  width: 220px;
  height: 100vh;
  background-color: #ffffff;
  box-shadow: 2px 0 10px rgba(0, 0, 0, 0.06);
  padding: 2rem 1.5rem;
  display: flex;
  flex-direction: column;
  font-family: 'Inter', 'Segoe UI', sans-serif;
  position: fixed;
  left: 0;
  top: 0;
  box-sizing: border-box;
  transition: all 0.3s ease;
}

.logo {
  font-size: 1.6rem;
  font-weight: 700;
  margin-bottom: 2rem;
  color: #1b2a49;
  text-align: center;
  letter-spacing: 1px;
}

.menu-links {
  list-style: none;
  padding: 0;
  margin: 0;
  flex-grow: 1;
}

.menu-links li {
  padding: 0.8rem 1rem;
  margin-bottom: 0.5rem;
  cursor: pointer;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1rem;
  color: #34495e;
  transition: background-color 0.25s, color 0.25s;
}

.menu-links li:hover {
  background-color: #1b2a49;
  color: #ffffff;
}

.menu-links li.active {
  background-color: #16243b;
  color: #ffffff;
  box-shadow: 0 0 8px rgba(22, 36, 59, 0.5);
}

.btn-logout {
  background-color: #e74c3c;
  border: none;
  color: white;
  padding: 0.75rem 1rem;
  font-weight: 600;
  font-size: 1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s, box-shadow 0.3s;
  margin-top: auto;
}

.btn-logout:hover {
  background-color: #c0392b;
  box-shadow: 0 0 12px rgba(192, 57, 43, 0.5);
}

@media (max-width: 768px) {
  .sidebar {
    flex-direction: row;
    height: auto;
    width: 100%;
    padding: 1rem;
    position: relative;
    justify-content: space-between;
    align-items: center;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
  }

  .logo {
    font-size: 1.4rem;
    margin-bottom: 0;
  }

  .menu-links {
    display: flex;
    flex-direction: row;
    gap: 0.8rem;
    flex-grow: 0;
  }

  .menu-links li {
    padding: 0.5rem 0.8rem;
    font-size: 0.95rem;
    margin-bottom: 0;
  }

  .btn-logout {
    font-size: 0.9rem;
    padding: 0.5rem 0.8rem;
    margin-top: 0;
  }
}
</style>
