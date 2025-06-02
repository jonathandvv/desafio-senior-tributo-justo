import { createRouter, createWebHistory } from 'vue-router'

import Login from '@/components/Login.vue'
import PaginaInicial from '@/views/PaginaInicial.vue'
import UploadArquivo from '@/components/UploadArquivo.vue'
import NotasInconsistentes from '@/components/NotasInconsistentes.vue'
import RelatorioIA from '@/components/RelatorioIA.vue'
import Dashboard from '@/components/Dashboard.vue'
import EmpresasNotas from '@/components/EmpresasNotas.vue'

const routes = [
  { path: '/', component: Login },
  {
    path: '/pagina-inicial',
    component: PaginaInicial,
    meta: { requiresAuth: true },
  },
  {
    path: '/upload',
    component: UploadArquivo,
    meta: { requiresAuth: true },
  },
  {
    path: '/notas-inconsistentes',
    component: NotasInconsistentes,
    meta: { requiresAuth: true },
  },
  {
    path: '/relatorio-ia',
    component: RelatorioIA,
    meta: { requiresAuth: true },
  },
  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true },
  },
  {
    path: '/empresas-notas',
    component: EmpresasNotas,
    meta: { requiresAuth: true },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/')
  } else {
    next()
  }
})

export default router
