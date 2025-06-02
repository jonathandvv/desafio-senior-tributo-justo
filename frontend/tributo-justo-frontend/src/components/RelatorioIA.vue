<template>
  <section class="relatorio-page">
    <div class="relatorio-box">
      <h2>Relatório Fiscal com IA</h2>

      <form @submit.prevent="buscarRelatorio">
        <label>
          CNPJ da Empresa
          <input
            id="cnpj"
            v-model="formulario.cnpj"
            type="text"
            placeholder="00.000.000/0000-00"
            required
            autocomplete="off"
            :disabled="carregando"
            @input="formulario.cnpj = $event.target.value.replace(/\D/g, '')"
            maxlength="14"
          />
        </label>

        <label>
          Número da Nota Fiscal
          <input
            v-model="formulario.numeroNota"
            type="text"
            required
            placeholder="Ex: 403378"
            :disabled="carregando"
          />
        </label>

        <button type="submit" :disabled="carregando">
          <template v-if="!carregando">Gerar Relatório</template>
          <template v-else><span class="loader"></span></template>
        </button>
      </form>

      <p v-if="erro" class="erro">Erro: {{ erro }}</p>

      <div v-if="relatorio" class="resultado">
        <h3>Relatório Gerado</h3>
        <pre>{{ relatorio.respostaGerada }}</pre>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref } from 'vue'

const token = localStorage.getItem('token') || ''

const formulario = reactive({
  cnpj: '',
  numeroNota: '',
})

const relatorio = ref(null)
const erro = ref(null)
const carregando = ref(false)

function validarCNPJ(cnpj) {
  return /^\d{14}$/.test(cnpj)
}

function validarFormulario() {
  erro.value = null
  if (!validarCNPJ(formulario.cnpj)) {
    erro.value = 'CNPJ inválido: deve conter exatamente 14 números.'
    return false
  }
  if (!formulario.numeroNota.trim()) {
    erro.value = 'Número da Nota Fiscal é obrigatório.'
    return false
  }
  return true
}

async function buscarRelatorio() {
  if (!validarFormulario()) return

  carregando.value = true
  relatorio.value = null

  try {
    const resposta = await fetch('https://localhost:7204/Relatorio/interpretar', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(formulario),
    })

    if (!resposta.ok) {
      const texto = await resposta.text()
      throw new Error(`Erro: ${texto}`)
    }

    const dados = await resposta.json()
    relatorio.value = dados
  } catch (e) {
    erro.value = e.message
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.relatorio-page {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding: 3rem 1rem;
  background-color: #f7f9fc;
  min-height: 100vh;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.relatorio-box {
  background: white;
  padding: 2.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  max-width: 550px;
  width: 100%;
}

h2 {
  font-size: 1.8rem;
  margin-bottom: 1.5rem;
  color: #2c3e50;
  text-align: center;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

label {
  font-weight: 600;
  color: #34495e;
  font-size: 1rem;
  display: flex;
  flex-direction: column;
}

input {
  margin-top: 0.25rem;
  padding: 0.55rem 0.75rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.25s ease;
}

input:focus {
  border-color: #1e87f0;
  outline: none;
}

button {
  background-color: #1e87f0;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  font-size: 1rem;
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

.erro {
  color: #e74c3c;
  font-weight: 600;
  margin-top: 1rem;
  text-align: center;
}

.resultado {
  margin-top: 2rem;
}

.resultado h3 {
  font-size: 1.4rem;
  margin-bottom: 0.75rem;
  color: #2c3e50;
  text-align: center;
}

.resultado pre {
  background: #ecf0f1;
  padding: 1rem;
  border-radius: 8px;
  font-size: 0.95rem;
  line-height: 1.5;
  white-space: pre-wrap;
  color: #2c3e50;
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
</style>
