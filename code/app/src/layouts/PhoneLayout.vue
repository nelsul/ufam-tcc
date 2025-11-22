<script setup lang="ts">
import LanguageSwitcher from "../components/LanguageSwitcher.vue";
</script>

<template>
  <div class="min-h-screen w-full nook-bg flex justify-center items-center p-0 sm:p-4">
    <main
      class="w-full min-h-screen sm:min-h-0 sm:w-full sm:max-w-md bg-canvas sm:rounded-[40px] sm:shadow-xl overflow-hidden flex flex-col border-0 sm:border-8 sm:border-white/50 relative"
    >
      <div
        class="hidden sm:flex justify-between items-center px-6 py-3 bg-brand-teal/10 border-b-2 border-brand-teal/20"
      >
        <div class="flex gap-1">
          <div class="w-2 h-2 rounded-full bg-brand-teal"></div>
          <div class="w-2 h-2 rounded-full bg-brand-teal"></div>
          <div class="w-2 h-2 rounded-full bg-brand-teal"></div>
        </div>
        <LanguageSwitcher />
        <div class="w-4 h-4 rounded-full bg-brand-teal"></div>
      </div>

      <div class="sm:hidden flex justify-end p-4 bg-brand-teal/10">
        <LanguageSwitcher />
      </div>

      <div class="flex-1 overflow-y-auto p-4 sm:p-6 scrollbar-hide">
        <router-view />
      </div>

      <div class="hidden sm:flex justify-center py-4 bg-canvas border-t-2 border-stone-100">
        <div class="w-32 h-1.5 rounded-full bg-stone-300"></div>
      </div>
    </main>
  </div>
</template>

<style>
/* Hide scrollbar for Chrome, Safari and Opera */
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}
</style>
