<template>
  <section class="inconsistencias-page">
    <div class="inconsistencias-box">
      <h2>Notas com Inconsistências</h2>
      <p class="subtitulo">Listagem de notas fiscais com possíveis riscos fiscais.</p>

      <ul v-if="notasInconsistentes.length" class="lista-notas">
        <li v-for="nota in notasInconsistentes" :key="nota.notaFiscalId" class="nota-item">
          <div class="nota-header">
            <strong>Nota: {{ nota.numeroNota }}</strong>
            <span class="detalhe">
              CNPJ: {{ nota.cnpjEmpresa }}
              • Total: R$ {{ Number(nota.total ?? 0).toFixed(2) }}
              • Impostos: R$ {{ Number(nota.impostos ?? 0).toFixed(2) }}
            </span>
            <span class="status" :class="nota.riscoFiscal > 0 ? 'risco' : 'sem-risco'">
              {{ nota.riscoFiscal > 0 ? 'Risco Fiscal' : 'Sem Risco' }}
            </span>
          </div>

          <ul class="inconsistencias">
            <li v-for="(inc, i) in nota.inconsistencias" :key="i">{{ inc }}</li>
          </ul>
        </li>
      </ul>

      <p v-else-if="carregando" class="carregando">Carregando notas com inconsistências...</p>
      <p v-else-if="erro" class="erro">{{ erro }}</p>
      <p v-else class="carregando">Nenhuma nota com inconsistência encontrada.</p>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const token = localStorage.getItem('token') || ''
const notasInconsistentes = ref([])
const carregando = ref(false)
const erro = ref(null)

async function carregarNotasInconsistentes() {
  if (!token) {
    erro.value = 'Usuário não autenticado.'
    return
  }

  carregando.value = true
  erro.value = null

  try {
    const response = await fetch('https://localhost:7204/Relatorio/inconsistencias', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (!response.ok) throw new Error('Erro ao buscar dados')

    const data = await response.json()
    notasInconsistentes.value = data
  } catch (error) {
    erro.value = error.message || 'Erro desconhecido'
  } finally {
    carregando.value = false
  }
}

onMounted(() => {
  carregarNotasInconsistentes()
})
</script>


<style scoped>
.inconsistencias-page {
  display: flex;
  justify-content: center;
  padding: 3rem 1rem;
  background-color: #f7f9fc;
  min-height: 100vh;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.inconsistencias-box {
  background: white;
  padding: 2.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  max-width: 700px;
  width: 100%;
}

h2 {
  font-size: 1.8rem;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  text-align: center;
}

.subtitulo {
  font-size: 1.05rem;
  text-align: center;
  margin-bottom: 2rem;
  color: #555;
}

.lista-notas {
  list-style: none;
  padding: 0;
  margin: 0;
}

.nota-item {
  padding: 1.2rem 1rem;
  border-bottom: 1px solid #e0e0e0;
}

.nota-header {
  margin-bottom: 0.75rem;
  font-size: 1rem;
  font-weight: 600;
  color: #2c3e50;
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
}

.detalhe {
  font-size: 0.95rem;
  color: #555;
}

.status {
  display: inline-block;
  margin-top: 0.5rem;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 700;
  color: white;
  width: fit-content;
}

.status.risco {
  background-color: #e74c3c;
}

.status.sem-risco {
  background-color: #27ae60;
}

.inconsistencias {
  padding-left: 1.2rem;
  list-style-type: disc;
  color: #c0392b;
  font-size: 0.95rem;
  line-height: 1.4;
}

.carregando {
  text-align: center;
  font-size: 1.1rem;
  color: #888;
  margin-top: 2rem;
}

@media (max-width: 480px) {
  .inconsistencias-box {
    padding: 2rem 1rem;
  }

  .nota-header {
    font-size: 0.95rem;
  }

  .detalhe {
    font-size: 0.9rem;
  }

  .inconsistencias {
    font-size: 0.9rem;
  }

  .erro {
    color: #c0392b;
    text-align: center;
    font-weight: 600;
    margin-top: 1rem;
  }
}
</style>
