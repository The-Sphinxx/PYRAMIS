<template>
  <div class="not-found-container">
    <div class="error-content">
      <div class="error-code">404</div>
      <h1>الصفحة غير موجودة</h1>
      <p>عذراً، الصفحة التي تبحث عنها غير موجودة أو تم نقلها.</p>
      <router-link to="/" class="home-button">
        العودة للرئيسية
      </router-link>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.not-found-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-align: center;
  padding: 2rem;
}

.error-content {
  max-width: 500px;
}

.error-code {
  font-size: 8rem;
  font-weight: bold;
  line-height: 1;
  margin-bottom: 1rem;
  text-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

h1 {
  font-size: 2rem;
  margin-bottom: 1rem;
}

p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.home-button {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: white;
  color: #667eea;
  text-decoration: none;
  border-radius: 8px;
  font-weight: 600;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.home-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.3);
}
</style>
