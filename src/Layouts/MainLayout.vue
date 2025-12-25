<template>
  <div>
    <Navbar :force-solid="!isHeroPage" />
    <div :class="{ 'pt-14': !isHeroPage }">
      <router-view />
    </div>
    <Footer />
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import Navbar from '@/components/Common/Navbar.vue';
import Footer from '@/components/Common/Footer.vue';

const route = useRoute();
const heroRoutes = ['Home', 'AttractionsList', 'HotelsList', 'CarsList', 'TripsList'];
const isHeroPage = computed(() => heroRoutes.includes(route.name));
</script>