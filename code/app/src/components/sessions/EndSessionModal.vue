<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { CheckCircle, Loader2 } from "lucide-vue-next";
import type { SessionDto } from "@/services/sessionsService";

interface Props {
  session: SessionDto | null;
  notes: string;
  loading: boolean;
}

const props = defineProps<Props>();
void props;

const emit = defineEmits<{
  close: [];
  confirm: [];
  "update:notes": [value: string];
}>();

const { t } = useI18n();
</script>

<template>
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
    <div class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-md shadow-2xl border-4 border-white">
      <h2 class="text-2xl font-bold text-primary-text mb-4">
        {{ t("sessions.end_session_title") }}
      </h2>

      <p class="text-primary-text/70 mb-6">
        {{ t("sessions.end_session_confirm", { name: session?.studentName }) }}
      </p>

      <div class="mb-6">
        <label class="block text-sm font-bold text-primary-text mb-2">
          {{ t("sessions.notes") }}
        </label>
        <textarea
          :value="notes"
          @input="emit('update:notes', ($event.target as HTMLTextAreaElement).value)"
          :placeholder="t('sessions.notes_placeholder')"
          rows="4"
          class="w-full px-4 py-3 rounded-xl border-2 border-stone-200 focus:border-brand-teal focus:outline-none resize-none text-primary-text"
        />
      </div>

      <div class="flex gap-3">
        <button
          @click="emit('close')"
          :disabled="loading"
          class="flex-1 py-3 rounded-xl border-2 border-stone-300 text-primary-text font-bold hover:bg-stone-100 transition-all disabled:opacity-70"
        >
          {{ t("common.cancel") }}
        </button>
        <button
          @click="emit('confirm')"
          :disabled="loading"
          class="flex-1 py-3 rounded-xl bg-brand-teal text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all disabled:opacity-70"
        >
          <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
          <CheckCircle v-else class="w-5 h-5" />
          {{ t("sessions.confirm_end") }}
        </button>
      </div>
    </div>
  </div>
</template>
