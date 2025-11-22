<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { Loader2 } from "lucide-vue-next";

defineProps<{
  loading?: boolean;
  submitLabel?: string;
  loadingLabel?: string;
  cancelLabel?: string;
  submitDisabled?: boolean;
}>();

defineEmits<{
  (e: "cancel"): void;
  (e: "submit"): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="pt-4 flex gap-3">
    <button
      type="button"
      @click="$emit('cancel')"
      class="flex-1 py-3 rounded-xl font-bold text-primary-text/70 hover:bg-black/5 transition-colors"
    >
      {{ cancelLabel || t("common.cancel") }}
    </button>
    <button
      type="submit"
      @click="$emit('submit')"
      :disabled="loading || submitDisabled"
      class="flex-1 py-3 rounded-xl bg-brand-teal text-white font-bold hover:scale-[1.02] active:scale-95 transition-all shadow-sm border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed flex justify-center items-center gap-2"
    >
      <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
      {{ loading ? loadingLabel || t("common.saving") : submitLabel || t("common.save_changes") }}
    </button>
  </div>
</template>
