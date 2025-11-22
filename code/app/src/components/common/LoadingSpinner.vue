<script setup lang="ts">
import { Loader2 } from "lucide-vue-next";

defineProps<{
  size?: "sm" | "md" | "lg" | "xl";
  color?: string;
  fullHeight?: boolean;
}>();

const sizeClasses = {
  sm: "w-4 h-4",
  md: "w-8 h-8",
  lg: "w-12 h-12",
  xl: "w-16 h-16",
};
</script>

<template>
  <div class="flex justify-center items-center" :class="fullHeight ? 'h-64' : ''">
    <Loader2
      class="animate-spin"
      :class="[sizeClasses[size || 'lg'], color || 'text-brand-teal']"
    />
  </div>
</template>
