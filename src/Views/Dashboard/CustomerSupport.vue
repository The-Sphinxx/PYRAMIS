<template>
  <div class="h-[calc(100vh-64px)] flex flex-col lg:flex-row bg-base-200 overflow-hidden rounded-2xl shadow-sm">
    <!-- Sidebar: Ticket List -->
    <div class="w-full lg:w-80 xl:w-96 flex-none bg-base-100 border-b lg:border-b-0 lg:border-r border-base-300 overflow-y-auto max-h-[40vh] lg:max-h-full">
      <TicketList />
    </div>

    <!-- Main Content: Chat -->
    <div class="flex-1 bg-base-100 min-w-0 overflow-hidden">
      <TicketChat />
    </div>
  </div>
</template>

<script setup>
import TicketList from '@/components/Support/TicketList.vue';
import TicketChat from '@/components/Support/TicketChat.vue';
</script>