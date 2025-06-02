<template>
  <div class="login-container">
    <h2>{{ modoRegistro ? 'Cadastro' : 'Login' }}</h2>

    <form @submit.prevent="modoRegistro ? registrar() : tentarLogin()">
      <input v-model="usuario" placeholder="Usuário" required :disabled="carregando" />
      <input
        type="password"
        v-model="senha"
        placeholder="Senha"
        required
        autocomplete="current-password"
        :disabled="carregando"
      />
      <button type="submit" :disabled="carregando">
        <span v-if="!carregando">{{ modoRegistro ? 'Cadastrar' : 'Entrar' }}</span>
        <span v-else class="loader"></span>
      </button>
    </form>

    <p v-if="erro" class="erro">{{ erro }}</p>

    <p class="alternar-modo">
      <span>{{ modoRegistro ? 'Já tem uma conta?' : 'Não tem uma conta?' }}</span>
      <a
        @click="!carregando && (modoRegistro = !modoRegistro)"
        :class="{ disabled: carregando }"
      >
        {{ modoRegistro ? 'Entrar' : 'Cadastrar' }}
      </a>
    </p>
  </div>
</template>

<script setup>
import { ref, watch, defineEmits } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useRouter } from 'vue-router'

const emit = defineEmits(['login-sucesso'])

const usuario = ref('')
const senha = ref('')
const erro = ref('')
const modoRegistro = ref(false)
const carregando = ref(false)

const { setToken } = useAuth()
const router = useRouter()

watch(modoRegistro, limparErro)
watch([usuario, senha], limparErro)

function limparErro() {
  erro.value = ''
}

async function tentarLogin() {
  await processarAutenticacao('entrar')
}

async function registrar() {
  await processarAutenticacao('registrar')
}

async function processarAutenticacao(rota) {
  resetarEstados()
  try {
    const resposta = await enviarRequisicaoAutenticacao(rota)
    await tratarRespostaAutenticacao(resposta)
  } catch (e) {
    erro.value = `Erro no ${rota === 'entrar' ? 'login' : 'cadastro'}: ${e.message}`
  } finally {
    carregando.value = false
  }
}

function resetarEstados() {
  erro.value = ''
  carregando.value = true
}

async function enviarRequisicaoAutenticacao(rota) {
  return await fetch(`https://localhost:7204/Autenticacao/${rota}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ usuario: usuario.value, senha: senha.value }),
  })
}

async function tratarRespostaAutenticacao(resposta) {
  if (!resposta.ok) {
    erro.value = (await resposta.text()) || 'Erro na autenticação.'
    return
  }
  const dados = await resposta.json()
  if (dados.token) {
    setToken(dados.token)
    emit('login-sucesso', dados.token)
    await router.push('/pagina-inicial')
  } else {
    erro.value = 'Erro inesperado: token não recebido.'
  }
}
</script>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 4rem auto;
  padding: 2.5rem;
  background-color: #ffffff;
  border-radius: 12px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.06);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #2c3e50;
  text-align: center;
}

h2 {
  font-size: 2rem;
  color: #1b2a49;
  margin-bottom: 2rem;
  font-weight: 700;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

input {
  padding: 0.75rem 1rem;
  font-size: 1rem;
  border: 1.8px solid #ccc;
  border-radius: 8px;
  color: #2c3e50;
  transition: border-color 0.3s ease, box-shadow 0.2s ease;
}

input:focus {
  border-color: #1b2a49;
  box-shadow: 0 0 8px #1b2a4980;
  outline: none;
}

button {
  padding: 0.8rem 1.2rem;
  font-size: 1.1rem;
  font-weight: 700;
  background-color: #1b2a49;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.25s ease, box-shadow 0.25s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

button:hover:not(:disabled) {
  background-color: #16243b;
  box-shadow: 0 4px 12px rgba(27, 42, 73, 0.4);
}

button:disabled {
  background-color: #4a5c7a;
  cursor: not-allowed;
}

.erro {
  margin-top: 1rem;
  color: #e74c3c;
  font-weight: 600;
}

.alternar-modo {
  margin-top: 1.5rem;
  font-size: 0.95rem;
  color: #34495e;
}

.alternar-modo a {
  margin-left: 0.3rem;
  color: #1b2a49;
  text-decoration: underline;
  font-weight: 600;
  cursor: pointer;
  transition: color 0.2s ease;
}

.alternar-modo a:hover:not(.disabled) {
  color: #16243b;
}

.alternar-modo a.disabled {
  pointer-events: none;
  opacity: 0.5;
  cursor: default;
}

.loader {
  border: 3px solid #fff;
  border-top: 3px solid #1b2a49;
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
  .login-container {
    padding: 2rem 1.5rem;
    margin: 2rem auto;
    border-radius: 10px;
  }

  h2 {
    font-size: 1.6rem;
  }

  button {
    font-size: 1rem;
  }

  .alternar-modo {
    font-size: 0.9rem;
  }
}
</style>
