<template>
  <div id="app">
    <div v-if="!logado" class="tela-login">
      <Login @login-sucesso="logar" />
    </div>
    <div v-else class="app-container">
      <MenuLateral
        :telaAtual="tela"
        @trocar-tela="trocarTela"
        @logout="deslogar"
      />
      <main class="conteudo-principal">
        <component :is="componenteAtual" />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'

import Login from './components/Login.vue'
import PaginaInicial from './views/PaginaInicial.vue'
import UploadArquivo from './components/UploadArquivo.vue'
import NotasInconsistentes from './components/NotasInconsistentes.vue'
import RelatorioIA from './components/RelatorioIA.vue'
import Dashboard from './components/Dashboard.vue'
import EmpresasNotas from './components/EmpresasNotas.vue'
import MenuLateral from './components/MenuLateral.vue'
import { useAuth } from './composables/useAuth'

const { setToken, limparToken } = useAuth()
const router = useRouter()

const logado = ref(false)
const tela = ref('pagina-inicial')

function logar(tokenRecebido) {
  setToken(tokenRecebido)
  logado.value = true
  tela.value = 'pagina-inicial'
  router.push('/pagina-inicial')
}

function deslogar() {
  limparToken()
  logado.value = false
  tela.value = 'pagina-inicial'
  router.push('/')
}

function trocarTela(nomeTela) {
  tela.value = nomeTela
  router.push(`/${nomeTela}`)
}

const componenteAtual = computed(() => {
  switch (tela.value) {
    case 'upload':
      return UploadArquivo
    case 'notas-inconsistentes':
      return NotasInconsistentes
    case 'relatorio-ia':
      return RelatorioIA
    case 'dashboard':
      return Dashboard
    case 'empresas-notas':
      return EmpresasNotas
    case 'pagina-inicial':
    default:
      return PaginaInicial
  }
})

watch(
  () => router.currentRoute.value.path,
  (path) => {
    const p = path.replace('/', '')
    if (
      ['pagina-inicial', 'upload', 'notas-inconsistentes', 'relatorio-ia', 'dashboard', 'empresas-notas'].includes(p)
    ) {
      tela.value = p
    } else if (path === '/' || path === '') {
      tela.value = 'pagina-inicial'
    }
  },
  { immediate: true }
)
</script>

