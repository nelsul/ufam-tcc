<script setup lang="ts">
defineProps<{
  cols?: 1 | 2 | 3 | 4;
  gap?: "sm" | "md" | "lg";
}>();

const colClasses = {
  1: "grid-cols-1",
  2: "grid-cols-1 md:grid-cols-2",
  3: "grid-cols-1 md:grid-cols-2 lg:grid-cols-3",
  4: "grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4",
};

const gapClasses = {
  sm: "gap-2 md:gap-3",
  md: "gap-4 md:gap-6",
  lg: "gap-6 md:gap-8",
};
</script>

<template>
  <div class="grid" :class="[colClasses[cols || 3], gapClasses[gap || 'md']]">
    <slot />
  </div>
</template>
