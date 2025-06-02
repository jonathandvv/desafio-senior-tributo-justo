<template>
  <section class="dashboard-page">
    <div class="dashboard-box">
      <h2>Dashboard Fiscal</h2>

      <form @submit.prevent="buscarDashboard" class="formulario">
        <label for="cnpj">CNPJ da Empresa:</label>
        <input id="cnpj" v-model="cnpj" type="text" placeholder="00.000.000/0000-00" required autocomplete="off"
          :disabled="loading" />
        <button type="submit" :disabled="loading">
          <template v-if="!loading">Buscar</template>
          <template v-else><span class="loader"></span></template>
        </button>
      </form>

      <p v-if="erro" class="erro">{{ erro }}</p>

      <div v-if="resumo" class="graficos">
        <DashboardGrafico titulo="Notas Fiscais (R$)" :labels="['Valor Total', 'Valor Média']"
          :valores="[resumo.valorTotalNotas, resumo.mediaValorNotas]" />

        <DashboardGrafico titulo="Impostos (R$)" :labels="['Valor Total', 'Valor Média']"
          :valores="[resumo.totalImpostos, resumo.mediaValorImpostos]" />

        <DashboardGrafico titulo="Risco Fiscal" :labels="['Risco (Pontuação)', 'Qtde Notas Risco']"
          :valores="[resumo.riscoFiscalTotal, resumo.quantidadeNotasComRisco]" />
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import DashboardGrafico from './DashboardGrafico.vue'

const token = localStorage.getItem('token') || ''
const cnpj = ref('')
const loading = ref(false)
const erro = ref(null)
const resumo = ref(null)

function validarCNPJ(cnpj) {
  return cnpj.replace(/\D/g, '').length === 14
}

function prepararHeaders() {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`,
  }
}

function limparCNPJ(cnpj) {
  return cnpj.replace(/\D/g, '')
}

async function buscarDashboard() {
  resetarEstados()
  
  if (!validarCNPJ(cnpj.value)) {
    erro.value = 'CNPJ inválido! Digite no formato correto.'
    return
  }

  loading.value = true

  try {
    const data = await enviarRequisicaoDashboard(limparCNPJ(cnpj.value))
    atualizarResumo(data.resumoNotasFiscais)
  } catch (e) {
    erro.value = e.message || 'Erro ao buscar dados.'
  } finally {
    loading.value = false
  }
}

async function enviarRequisicaoDashboard(cnpjLimpo) {
  const response = await fetch('https://localhost:7204/Relatorio/dashboard', {
    method: 'POST',
    headers: prepararHeaders(),
    body: JSON.stringify({ cnpj: cnpjLimpo }),
  })

  if (!response.ok) {
    const text = await response.text()
    throw new Error(`Erro ao buscar dados: ${text}`)
  }

  return await response.json()
}

function atualizarResumo(novoResumo) {
  resumo.value = novoResumo
}

function resetarEstados() {
  erro.value = null
  resumo.value = null
}
</script>



<style scoped>
.dashboard-page {
  min-height: 100vh;
  background-color: #f7f9fc;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 3rem 1rem;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.dashboard-box {
  background: white;
  padding: 2.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  width: 100%;
  max-width: 500px;
  text-align: center;
  transition: transform 0.3s ease;
}

.dashboard-box:hover {
  transform: translateY(-3px);
}

h2 {
  font-size: 1.8rem;
  margin-bottom: 2rem;
  color: #2c3e50;
  font-weight: 700;
}

.formulario {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
  text-align: left;
}

label {
  font-weight: 600;
  color: #34495e;
  user-select: none;
}

input[type='text'] {
  padding: 0.6rem;
  font-size: 1rem;
  border: 1.5px solid #ccc;
  border-radius: 8px;
  outline-offset: 2px;
  transition: border-color 0.3s;
  width: 100%;
}

input[type='text']:focus {
  border-color: #1e87f0;
  outline: none;
}

button {
  padding: 0.7rem 0;
  background-color: #1e87f0;
  border: none;
  color: white;
  font-weight: 700;
  font-size: 1.1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.25s ease;
  display: flex;
  justify-content: center;
  align-items: center;
}

button:disabled {
  background-color: #a0c4f6;
  cursor: not-allowed;
}

button:hover:not(:disabled) {
  background-color: #166bb3;
}

p.erro {
  color: #e74c3c;
  font-weight: 700;
  margin-top: -1rem;
  margin-bottom: 1.5rem;
  user-select: none;
}

.graficos {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.loader {
  border: 3px solid #ffffff80;
  border-top: 3px solid #1e87f0;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

@media (max-width: 480px) {
  .dashboard-box {
    padding: 1.8rem 1.5rem;
    max-width: 100%;
  }
}
</style>
