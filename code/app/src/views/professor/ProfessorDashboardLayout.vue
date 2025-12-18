<script setup lang="ts">
import ProfessorDashboardMenu from "../../components/dashboard/ProfessorDashboardMenu.vue";
</script>

<template>
  <div class="min-h-screen w-full nook-bg flex justify-center md:items-center md:p-8">
    <div
      class="w-full min-h-[800px] md:h-[85vh] md:max-w-7xl bg-[#FDFDF5] md:rounded-[40px] md:shadow-2xl overflow-hidden flex flex-col md:flex-row border-0 md:border-8 md:border-white/50 relative"
    >
      <ProfessorDashboardMenu />

      <main class="flex-1 overflow-y-auto pt-16 md:pt-8 px-4 pb-8 md:p-8 scrollbar-hide bg-white/50">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
