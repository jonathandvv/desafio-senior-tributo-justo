# Desafio Técnico – Desenvolvedor(a) Sênior (IA + Banco de Dados)

## 🎯 Objetivo Geral

Desenvolver uma aplicação completa que realize a ingestão massiva de dados fiscais, detecte inconsistências tributárias, gere relatórios e insights com o apoio de IA e exponha esses resultados via API e interface web.

---

## 🧩 Cenário

A Tributo Justo atua na consultoria tributária de grandes empresas. Diariamente, recebemos milhares de notas fiscais para análise. Seu desafio é criar uma solução capaz de processar essas informações e oferecer insights inteligentes sobre o comportamento fiscal dos clientes.

---

## 🧱 Escopo da Solução

### 1. Importação e Estruturação de Dados

- Receber arquivos `.csv` ou `.json` com milhares de notas fiscais.
- Armazenar os dados em um banco relacional com modelagem que envolva:
  - Empresas (CNPJ, razão social, regime tributário)
  - Notas fiscais (número, data, total, impostos)
  - Itens (NCM, descrição, quantidade, valor unitário, alíquota)
  - Natureza da operação (CFOP, tipo de produto, etc.)
- Considerar aspectos de performance: índices, consultas otimizadas, estratégia de carga.

### 2. Análise e Detecção de Inconsistências

- Desenvolver regras que identifiquem situações como:
  - Recolhimento fora da média para o CNPJ e período.
  - Alíquotas incompatíveis com o NCM.
  - Itens com valor total elevado, mas imposto reduzido.
- Implementar score de risco fiscal para cada nota.

### 3. Integração com IA

- Criar um endpoint `/relatorio/interpretar` que envie dados resumidos para uma LLM (ex.: OpenAI ou HuggingFace) e gere:
  - Resumo do comportamento fiscal
  - Ações recomendadas (revisar classificação, reemitir nota, etc.)

**Exemplo de prompt:**

> "Analise as notas da empresa X no mês de abril e identifique padrões anormais no recolhimento de ICMS. Sugira hipóteses."

### 4. API REST

- Endpoints protegidos com JWT
- Expor:
  - Upload de arquivos
  - Listagem de notas e empresas
  - Relatórios de inconsistência
  - Detalhamento por nota
  - Endpoint de insights com IA

### 5. Front-end Web (React ou equivalente)

- Tela de login/registro
- Upload de arquivos
- Listagem de notas com indicadores de inconsistência
- Tela de relatório com resumo e insights da IA
- Dashboard com KPIs (imposto médio, risco por empresa, etc.)

---

## 🛠️ Tecnologias sugeridas

- **Back-end:** Python (FastAPI), .NET, Node.js
- **Front-end:** React
- **Banco de dados:** PostgreSQL (preferido)
- **Outros:** Docker, Redis (cache), Celery/Hangfire (tarefas assíncronas), GitHub Actions (CI)

---

## ✅ Critérios de Avaliação

| Critério                           | Peso |
|-----------------------------------|------|
| Clareza e organização da solução  | Alto |
| Capacidade de lidar com ambiguidade | Alto |
| Criatividade e uso relevante da IA | Alto |
| Modelagem e performance do banco  | Alto |
| Automação e testes                | Médio|
| Qualidade da API e documentação   | Médio|
| Interface funcional e navegável  | Médio|
| Argumentação e defesa técnica     | Alto |

---

## 📦 Entrega

- Repositório Git com pastas organizadas:
  - `/backend`
  - `/frontend`
  - `/docs`
- README com:
  - Instruções de execução (local ou Docker)
  - Descrição da modelagem e arquitetura adotada
  - Detalhes da integração com a IA
- Enviar o link por e-mail com o assunto: `Entrega Desafio Técnico – Sênior – [Seu Nome]`
- Prazo sugerido: **até 5 dias úteis** após recebimento

---

Este é um desafio de **nível sênior**, e esperamos observar não apenas a solução funcional, mas também o seu raciocínio técnico, decisões de arquitetura, clareza de organização e criatividade. Boa sorte!
