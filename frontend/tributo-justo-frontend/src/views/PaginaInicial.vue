<template>
  <section class="pagina-inicial">
    <div class="welcome-box">
      <h1>Bem-vindo!</h1>
      <p>Este é o painel Tributo Justo.</p>
    </div>
  </section>
</template>

<style scoped>
.pagina-inicial {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: #f7f9fc;
  padding: 2rem;
  font-family: 'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #2c3e50;
}

.welcome-box {
  background: white;
  padding: 2.5rem 3rem;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  text-align: center;
  max-width: 420px;
  width: 100%;
  transition: transform 0.3s ease;
}

.welcome-box:hover {
  transform: translateY(-4px);
}

h1 {
  font-size: 2rem;
  margin-bottom: 1rem;
  font-weight: 600;
}

p {
  font-size: 1.1rem;
  margin-bottom: 2rem;
  color: #555;
}

.btn-primary {
  background-color: #1e87f0;
  color: white;
  border: none;
  padding: 0.75rem 1.75rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.25s ease;
}

.btn-primary:hover {
  background-color: #166bb3;
}

@media (max-width: 500px) {
  .welcome-box {
    padding: 2rem 1.5rem;
  }

  h1 {
    font-size: 1.6rem;
  }

  p {
    font-size: 1rem;
  }

  .btn-primary {
    width: 100%;
    font-size: 0.95rem;
  }
}
</style>
