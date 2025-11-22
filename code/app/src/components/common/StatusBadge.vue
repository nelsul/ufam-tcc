<script setup lang="ts">
import { useI18n } from "vue-i18n";

const props = defineProps<{
  status: string;
  size?: "sm" | "md" | "lg";
}>();

const { t } = useI18n();

const sizeClasses = {
  sm: "text-[10px] px-2 py-0.5",
  md: "text-xs px-2 py-1",
  lg: "text-sm px-3 py-1.5",
};

const isActive = () => props.status?.toLowerCase() === "active";
</script>

<template>
  <span
    class="font-bold rounded-full uppercase tracking-wider"
    :class="[
      sizeClasses[size || 'md'],
      isActive() ? 'bg-brand-green/20 text-green-600' : 'bg-stone-200 text-stone-500',
    ]"
  >
    {{ isActive() ? t("common.active") : t("common.inactive") }}
    <slot />
  </span>
</template>
