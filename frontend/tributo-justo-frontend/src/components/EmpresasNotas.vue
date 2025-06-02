<template>
  <section class="page">
    <div class="box">
      <h2>Relação de Empresas, Notas Fiscais e Itens Correspondentes</h2>

      <p v-if="carregandoEmpresas" class="info">Carregando empresas...</p>
      <p v-if="mensagem" :class="{ erro: mensagem.includes('Erro') }">{{ mensagem }}</p>

      <ul v-if="!carregandoEmpresas && empresas.length" class="list">
        <li v-for="empresa in empresas" :key="empresa.id" class="list-item">
          <button class="toggle-btn" @click="toggleEmpresa(empresa.id)">
            <span>{{ empresa.razaoSocial }}</span>
            <small>CNPJ: {{ empresa.cnpj }}</small>
            <span>{{ isEmpresaExpanded(empresa.id) ? '−' : '+' }}</span>
          </button>

          <div v-if="isEmpresaExpanded(empresa.id)" class="nested">
            <p v-if="carregandoNotas[empresa.id]" class="info">Carregando notas...</p>
            <p v-if="erroNotas[empresa.id]" class="erro">{{ erroNotas[empresa.id] }}</p>

            <ul v-if="notasPorEmpresa[empresa.id]?.length" class="list nested-list">
              <li v-for="nota in notasPorEmpresa[empresa.id]" :key="nota.id" class="list-item">
                <button class="toggle-btn" @click="toggleNota(nota.id)">
                  <span>Nº {{ nota.numero }} — {{ formatarData(nota.dataEmissao) }}</span>
                  <small>
                    Total: R$ {{ nota.total.toFixed(2) }} • Impostos: R$ {{ nota.impostos.toFixed(2) }} • Risco Fiscal: {{ nota.riscoFiscal ? 'Sim' : 'Não' }}
                  </small>
                  <span>{{ isNotaExpanded(nota.id) ? '−' : '+' }}</span>
                </button>

                <div v-if="isNotaExpanded(nota.id)" class="nested">
                  <p v-if="carregandoDetalhesNota[nota.id]" class="info">Carregando detalhes da nota...</p>
                  <p v-if="erroDetalhesNota[nota.id]" class="erro">{{ erroDetalhesNota[nota.id] }}</p>

                  <ul v-if="detalhesNota[nota.id]?.itens?.length" class="list nested-list">
                    <li v-for="item in detalhesNota[nota.id].itens" :key="item.id" class="list-item">
                      {{ item.descricao }} — Qtde: {{ item.quantidade }} — R$ {{ item.valorUnitario.toFixed(2) }}
                    </li>
                  </ul>

                  <p v-else class="info">Nenhum detalhe encontrado.</p>
                </div>
              </li>
            </ul>

            <p v-else class="info">Nenhuma nota encontrada para essa empresa.</p>
          </div>
        </li>
      </ul>

      <p v-if="!carregandoEmpresas && empresas.length === 0" class="info">Nenhuma empresa encontrada.</p>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'

const empresas = ref([])
const carregandoEmpresas = ref(false)
const mensagem = ref('')

const notasPorEmpresa = reactive({})
const carregandoNotas = reactive({})
const erroNotas = reactive({})
const empresasExpandida = ref([])

const detalhesNota = reactive({})
const carregandoDetalhesNota = reactive({})
const erroDetalhesNota = reactive({})
const notasExpandida = ref([])

const { getToken } = useAuth()

function obterToken() {
  const token = getToken()
  if (!token) throw new Error('Token de autenticação não encontrado.')
  return token
}

async function carregarEmpresas() {
  limparEstadosEmpresas()
  carregandoEmpresas.value = true

  try {
    const token = obterToken()
    empresas.value = await buscarEmpresas(token)
  } catch (e) {
    mensagem.value = `Erro ao carregar empresas: ${e.message}`
  } finally {
    carregandoEmpresas.value = false
  }
}

async function carregarNotas(idEmpresa) {
  limparEstadosNotas(idEmpresa)
  carregandoNotas[idEmpresa] = true

  try {
    const token = obterToken()
    notasPorEmpresa[idEmpresa] = await buscarNotas(token, idEmpresa)
  } catch (e) {
    erroNotas[idEmpresa] = e.message
  } finally {
    carregandoNotas[idEmpresa] = false
  }
}

async function carregarDetalhesNota(idNota) {
  limparEstadosDetalhes(idNota)
  carregandoDetalhesNota[idNota] = true

  try {
    const token = obterToken()
    detalhesNota[idNota] = await buscarDetalhesNota(token, idNota)
  } catch (e) {
    erroDetalhesNota[idNota] = e.message
  } finally {
    carregandoDetalhesNota[idNota] = false
  }
}

async function buscarEmpresas(token) {
  const resposta = await fetch('https://localhost:7204/Empresas', {
    headers: { Authorization: `Bearer ${token}` }
  })
  if (!resposta.ok) throw new Error(await resposta.text())
  return await resposta.json()
}

async function buscarNotas(token, idEmpresa) {
  const resposta = await fetch(`https://localhost:7204/NotasFiscais/empresa/${idEmpresa}`, {
    headers: { Authorization: `Bearer ${token}` }
  })
  if (!resposta.ok) throw new Error(await resposta.text())
  return await resposta.json()
}

async function buscarDetalhesNota(token, idNota) {
  const resposta = await fetch(`https://localhost:7204/NotasFiscais/${idNota}`, {
    headers: { Authorization: `Bearer ${token}` }
  })
  if (!resposta.ok) throw new Error(await resposta.text())
  return await resposta.json()
}

function toggleEmpresa(id) {
  if (isEmpresaExpanded(id)) {
    empresasExpandida.value = empresasExpandida.value.filter(x => x !== id)
  } else {
    empresasExpandida.value.push(id)
    if (!notasPorEmpresa[id]) carregarNotas(id)
  }
}

function isEmpresaExpanded(id) {
  return empresasExpandida.value.includes(id)
}

function toggleNota(id) {
  if (isNotaExpanded(id)) {
    notasExpandida.value = notasExpandida.value.filter(x => x !== id)
  } else {
    notasExpandida.value.push(id)
    if (!detalhesNota[id]) carregarDetalhesNota(id)
  }
}

function isNotaExpanded(id) {
  return notasExpandida.value.includes(id)
}

function limparEstadosEmpresas() {
  mensagem.value = ''
  empresas.value = []
}

function limparEstadosNotas(idEmpresa) {
  erroNotas[idEmpresa] = ''
  notasPorEmpresa[idEmpresa] = []
}

function limparEstadosDetalhes(idNota) {
  erroDetalhesNota[idNota] = ''
  detalhesNota[idNota] = null
}

function formatarData(data) {
  return new Date(data).toLocaleDateString('pt-BR')
}

onMounted(() => {
  carregarEmpresas()
})
</script>


<style scoped>
.page {
  display: flex;
  justify-content: center;
  padding: 3rem 1rem;
  min-height: 100vh;
  background-color: #f7f9fc;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.box {
  background: white;
  padding: 2.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  width: 100%;
  max-width: 700px;
  text-align: center;
  transition: transform 0.3s ease;
}

.box:hover {
  transform: translateY(-3px);
}

h2 {
  font-size: 1.8rem;
  margin-bottom: 1.5rem;
  color: #2c3e50;
}

.list {
  list-style: none;
  padding: 0;
  margin: 0;
  text-align: left;
}

.list-item {
  border-bottom: 1px solid #ddd;
  padding: 0.7rem 0;
}

.toggle-btn {
  width: 100%;
  background: none;
  border: none;
  font-size: 1rem;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0;
  color: #2c3e50;
  font-weight: 600;
}

.toggle-btn small {
  color: #666;
  margin-left: 0.5rem;
  font-weight: normal;
  font-size: 0.85rem;
}

.nested, 
.nested * {
  color: #2c3e50 !important;
}


.nested-list {
  margin-top: 0.3rem;
}

.info {
  color: #555;
  font-size: 0.9rem;
  margin: 0.4rem 0;
  font-weight: 500;
}

.erro {
  color: #e74c3c;
  font-size: 0.9rem;
  margin: 0.4rem 0;
  font-weight: 500;
}
</style>
