<script setup lang="ts">
import StatusBadge from "./StatusBadge.vue";
import { Pencil } from "lucide-vue-next";

defineProps<{
  clickable?: boolean;
  showEditIndicator?: boolean;
  status?: string;
  tapeColor?: string;
}>();

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <div
    @click="clickable !== false ? $emit('click') : undefined"
    class="bg-white rounded-3xl p-5 border-2 border-dashed border-soft-gray hover:border-brand-orange transition-colors group relative overflow-hidden h-full flex flex-col"
    :class="clickable !== false ? 'cursor-pointer' : ''"
  >
    <div
      class="absolute -top-3 right-8 w-16 h-6 -rotate-2"
      :class="tapeColor || 'bg-brand-teal/50'"
    />

    <div
      v-if="showEditIndicator !== false && clickable !== false"
      class="absolute top-4 right-4 opacity-0 group-hover:opacity-100 transition-opacity"
    >
      <Pencil class="w-4 h-4 text-brand-teal" />
    </div>

    <div class="mt-2 flex-1">
      <slot />
    </div>

    <div
      v-if="status || $slots.footer"
      class="pt-4 border-t border-dashed border-soft-gray flex justify-between items-center mt-auto"
    >
      <StatusBadge v-if="status" :status="status" />
      <slot name="footer" />
    </div>
  </div>
</template>
