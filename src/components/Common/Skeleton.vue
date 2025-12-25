<template>
  <div
    class="relative overflow-hidden p-4 w-full max-w-md mx-auto rounded-2xl glass"
  >
    <div class="animate-pulse">
      <div class="h-40 w-full rounded-xl bg-base-300"></div>

      <div class="mt-4 h-5 w-3/4 rounded-lg bg-base-300"></div>

      <div class="mt-2 h-4 w-full rounded-lg bg-base-300"></div>
      <div class="mt-2 h-4 w-5/6 rounded-lg bg-base-300"></div>

      <div class="mt-4 h-10 w-28 rounded-xl bg-base-300"></div>
    </div>

    <div
      class="absolute inset-0 shimmer"></div>
  </div>
</template>

<style scoped>
.shimmer {
  background: linear-gradient(
    110deg,
    transparent 0%,
    rgba(255, 255, 255, 0.25) 45%,
    transparent 100%
  );
  animation: shimmer 1.8s infinite linear;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(150%);
  }
}
</style>
