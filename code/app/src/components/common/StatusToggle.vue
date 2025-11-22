<script setup lang="ts">
import { useI18n } from "vue-i18n";

const props = defineProps<{
  status: string;
  disabled?: boolean;
}>();

const emit = defineEmits<{
  (e: "toggle"): void;
}>();

const { t } = useI18n();

const isActive = () => props.status === "Active";
</script>

<template>
  <button
    type="button"
    @click="emit('toggle')"
    :disabled="disabled"
    class="w-full flex items-center justify-between bg-white rounded-2xl border-2 border-soft-gray px-4 py-3 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    :class="isActive() ? 'border-green-300' : 'border-red-300'"
  >
    <span class="font-medium" :class="isActive() ? 'text-green-700' : 'text-red-700'">
      {{ isActive() ? t("common.active") : t("common.inactive") }}
    </span>
    <span
      class="text-xs px-2 py-1 rounded-lg"
      :class="isActive() ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'"
    >
      {{ isActive() ? t("common.click_to_deactivate") : t("common.click_to_activate") }}
    </span>
  </button>
</template>
