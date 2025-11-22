<script setup lang="ts">
import { Search, X } from "lucide-vue-next";

defineProps<{
  modelValue: string;
  placeholder?: string;
  label?: string;
  showClearButton?: boolean;
  inlineLabel?: boolean;
}>();

defineEmits<{
  (e: "update:modelValue", value: string): void;
  (e: "search"): void;
  (e: "clear"): void;
}>();
</script>

<template>
  <div :class="inlineLabel ? 'flex items-center gap-3' : ''">
    <label
      v-if="label"
      class="text-sm font-bold text-primary-text/70 whitespace-nowrap"
      :class="inlineLabel ? '' : 'block mb-1'"
    >
      {{ label }}
    </label>
    <div class="relative flex-1">
      <Search class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-primary-text/40" />
      <input
        :value="modelValue"
        @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
        @keyup.enter="$emit('search')"
        type="text"
        :placeholder="placeholder"
        class="w-full pl-12 pr-10 py-3 rounded-2xl bg-soft-gray/30 border-2 border-transparent focus:border-brand-teal focus:bg-white focus:outline-none text-sm font-medium placeholder:text-primary-text/40 transition-all"
      />
      <button
        v-if="modelValue && showClearButton !== false"
        @click="
          $emit('update:modelValue', '');
          $emit('clear');
        "
        type="button"
        class="absolute right-4 top-1/2 -translate-y-1/2 text-primary-text/40 hover:text-primary-text transition-colors"
      >
        <X class="w-4 h-4" />
      </button>
    </div>
  </div>
</template>
