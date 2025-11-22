<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { ChevronLeft, ChevronRight } from "lucide-vue-next";

defineProps<{
  pageNumber: number;
  totalPages: number;
  canGoPrev?: boolean;
  canGoNext?: boolean;
}>();

defineEmits<{
  (e: "prev"): void;
  (e: "next"): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="mt-8 flex justify-center items-center gap-4">
    <button
      @click="$emit('prev')"
      :disabled="canGoPrev === false || pageNumber === 1"
      class="p-2 rounded-full bg-white border-2 border-soft-gray hover:border-brand-teal disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
    >
      <ChevronLeft class="w-6 h-6 text-primary-text" />
    </button>

    <span class="font-bold text-primary-text">
      {{ t("common.page_of", { current: pageNumber, total: totalPages }) }}
    </span>

    <button
      @click="$emit('next')"
      :disabled="canGoNext === false || pageNumber === totalPages"
      class="p-2 rounded-full bg-white border-2 border-soft-gray hover:border-brand-teal disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
    >
      <ChevronRight class="w-6 h-6 text-primary-text" />
    </button>
  </div>
</template>
