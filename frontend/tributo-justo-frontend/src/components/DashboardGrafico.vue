<template>
    <section class="grafico-box">
      <h3>{{ titulo }}</h3>
      <Bar :data="chartData" :options="chartOptions" />
    </section>
  </template>
  
  <script setup>
  import { computed } from 'vue'
  import { Bar } from 'vue-chartjs'
  import {
    Chart,
    Title,
    Tooltip,
    Legend,
    BarElement,
    CategoryScale,
    LinearScale
  } from 'chart.js'
  
  Chart.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
  
  const props = defineProps({
    titulo: String,
    labels: Array,
    valores: Array
  })
  
  const coresDeFundo = [
    'rgba(54, 162, 235, 0.7)',   // azul
    'rgba(255, 99, 132, 0.7)',   // vermelho
    'rgba(255, 206, 86, 0.7)',   // amarelo
    'rgba(75, 192, 192, 0.7)',   // verde
    'rgba(153, 102, 255, 0.7)',  // roxo
    'rgba(255, 159, 64, 0.7)'    // laranja
  ]
  
  const chartData = computed(() => ({
    labels: props.labels,
    datasets: [
      {
        label: props.titulo,
        data: props.valores,
        backgroundColor: coresDeFundo.slice(0, props.valores.length),
        borderRadius: 8,
        borderWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.1)'
      }
    ]
  }))
  
  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: { enabled: true }
    },
    scales: {
      y: { beginAtZero: true }
    }
  }
  </script>
  
  <style scoped>
  .grafico-box {
    background: white;
    padding: 1.8rem 1.5rem;
    border-radius: 12px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
    max-width: 500px;
    margin: 0 auto;
    height: 230px;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    text-align: center;
    transition: transform 0.3s ease;
  }
  
  .grafico-box:hover {
    transform: translateY(-3px);
  }
  
  h3 {
    font-weight: 700;
    color: #2c3e50;
    margin-bottom: 1rem;
    user-select: none;
    font-size: 1.25rem;
  }
  </style>
  