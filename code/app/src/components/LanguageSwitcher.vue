<script setup lang="ts">
import { computed, watch } from "vue";
import { useI18n } from "vue-i18n";

const { locale } = useI18n();

const languages = [
  { code: "pt-BR", name: "🇧🇷 Português" },
  { code: "es", name: "🇪🇸 Español" },
  { code: "en", name: "🇺🇸 English" },
];

const _currentLanguage = computed(() => {
  return languages.find((lang) => lang.code === locale.value) || languages[0];
});

watch(locale, (newLocale) => {
  localStorage.setItem("locale", newLocale);
});
</script>

<template>
  <div class="relative inline-block">
    <select
      v-model="locale"
      class="appearance-none bg-white/80 border-2 border-soft-gray text-primary-text font-bold px-4 py-2 pr-8 rounded-full cursor-pointer hover:bg-brand-yellow/20 hover:border-brand-yellow transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-brand-teal focus:border-brand-teal"
    >
      <option v-for="lang in languages" :key="lang.code" :value="lang.code">
        {{ lang.name }}
      </option>
    </select>
    <div
      class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-primary-text"
    >
      <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
        <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
      </svg>
    </div>
  </div>
</template>
