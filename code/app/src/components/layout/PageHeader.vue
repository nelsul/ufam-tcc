<script setup lang="ts">
import { Plus } from "lucide-vue-next";
import type { Component } from "vue";

defineProps<{
  title: string;
  subtitle?: string;
  icon?: Component;
  buttonLabel?: string;
  buttonIcon?: Component;
  buttonColor?: string;
  showButton?: boolean;
}>();

defineEmits<{
  (e: "action"): void;
}>();
</script>

<template>
  <header class="mb-6 md:mb-8 flex flex-col md:flex-row md:justify-between md:items-end gap-4">
    <div>
      <div class="flex items-center gap-3 mb-2">
        <component v-if="icon" :is="icon" class="w-7 h-7 text-brand-teal" />
        <h1 class="text-2xl md:text-4xl font-bold text-primary-text">
          {{ title }}
        </h1>
      </div>
      <p v-if="subtitle" class="text-primary-text/60">{{ subtitle }}</p>
    </div>
    <slot name="actions">
      <button
        v-if="showButton !== false && buttonLabel"
        @click="$emit('action')"
        class="w-full md:w-auto px-4 py-2 rounded-xl font-bold flex items-center justify-center gap-2 hover:scale-105 transition-transform shadow-sm border-2 border-white"
        :class="buttonColor || 'bg-brand-teal text-white'"
      >
        <component :is="buttonIcon || Plus" class="w-5 h-5" />
        {{ buttonLabel }}
      </button>
    </slot>
  </header>
</template>
