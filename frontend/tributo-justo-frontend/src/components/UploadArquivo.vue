<template>
  <section class="upload-page">
    <div class="upload-box">
      <h2>Enviar Arquivo</h2>

      <form @submit.prevent="enviarArquivo">
        <label class="custom-file-upload">
          {{ arquivo ? arquivo.name : 'Selecione um arquivo' }}
          <input type="file" @change="selecionarArquivo" :disabled="carregando" />
        </label>

        <button type="submit" :disabled="!arquivo || carregando">
          <template v-if="carregando">
            <span class="spinner"></span> Enviando...
          </template>
          <template v-else>
            Enviar
          </template>
        </button>
      </form>

      <p v-if="mensagem" :class="{ erro: mensagem.includes('Erro') }">{{ mensagem }}</p>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'

const { getToken } = useAuth()
const arquivo = ref(null)
const mensagem = ref('')
const carregando = ref(false)

onMounted(() => {
  validarToken()
})

function validarToken() {
  const token = getToken()
  if (!token) {
    mensagem.value = 'Erro: token de autenticação não encontrado. Faça login novamente.'
  }
}

function selecionarArquivo(event) {
  arquivo.value = event.target.files[0] || null
}

async function enviarArquivo() {
  if (!arquivo.value) {
    mensagem.value = 'Selecione um arquivo antes de enviar.'
    return
  }

  if (!validarTokenParaUpload()) return

  carregando.value = true
  mensagem.value = ''

  const formData = prepararFormData()

  try {
    const resposta = await enviarRequisicaoUpload(formData)
    await tratarRespostaUpload(resposta)
  } catch (erro) {
    mensagem.value = 'Erro no upload: ' + erro.message
  } finally {
    carregando.value = false
  }
}

function validarTokenParaUpload() {
  const token = getToken()
  if (!token) {
    mensagem.value = 'Erro: token de autenticação não encontrado. Faça login novamente.'
    return false
  }
  return true
}

function prepararFormData() {
  const formData = new FormData()
  formData.append('Arquivo', arquivo.value)
  return formData
}

async function enviarRequisicaoUpload(formData) {
  const token = getToken()
  return await fetch('https://localhost:7204/Upload/arquivo', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  })
}

async function tratarRespostaUpload(resposta) {
  const texto = (await resposta.text()).trim()

  if (!resposta.ok) {
    mensagem.value = 'Erro: ' + texto
    return
  }

  mensagem.value = texto
}
</script>



<style scoped>
.upload-page {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 3rem 1rem;
  min-height: 100vh;
  background-color: #f7f9fc;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.upload-box {
  background: white;
  padding: 2.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  width: 100%;
  max-width: 500px;
  text-align: center;
  transition: transform 0.3s ease;
}

.upload-box:hover {
  transform: translateY(-3px);
}

h2 {
  font-size: 1.8rem;
  margin-bottom: 1.5rem;
  color: #2c3e50;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}

.custom-file-upload {
  display: inline-block;
  padding: 0.6rem 1rem;
  border: 1.5px solid #ccc;
  border-radius: 8px;
  background-color: white;
  color: #2c3e50;
  font-size: 1rem;
  cursor: pointer;
  width: 100%;
  text-align: center;
  user-select: none;
  transition: border-color 0.3s;
  max-width: 100%;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}

.custom-file-upload:hover {
  border-color: #1e87f0;
}

.custom-file-upload input[type="file"] {
  display: none;
}

button {
  padding: 0.7rem 2rem;
  background-color: #1e87f0;
  border: none;
  color: white;
  font-weight: 600;
  font-size: 1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.25s ease;
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
}

button:disabled {
  background-color: #a0c4f6;
  cursor: not-allowed;
}

button:hover:not(:disabled) {
  background-color: #166bb3;
}

p {
  margin-top: 1.5rem;
  font-size: 1rem;
  color: #2c3e50 !important;
  font-weight: 500;
}

p.erro {
  color: #e74c3c !important;
}

.spinner {
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top: 3px solid white;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  animation: spin 1s linear infinite;
  display: inline-block;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
