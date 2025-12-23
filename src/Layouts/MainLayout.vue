<template>
  <div>
    <Navbar />
    <router-view />
    <Footer />
  </div>
</template>

<script setup>
import Navbar from '@/components/Common/Navbar.vue';
import Footer from '@/components/Common/Footer.vue';
</script>