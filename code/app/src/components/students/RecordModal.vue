<script setup lang="ts">
import { X, FileText, Loader2 } from "lucide-vue-next";

defineProps<{
  show: boolean;
  content: string;
  saving: boolean;
}>();

const _emit = defineEmits<{
  (e: "close"): void;
  (e: "save"): void;
  (e: "update:content", value: string): void;
}>();
</script>

<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4 backdrop-blur-sm"
  >
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-4xl h-[80vh] shadow-2xl border-4 border-white relative flex flex-col"
    >
      <button
        @click="$emit('close')"
        class="absolute top-6 right-6 text-primary-text/50 hover:text-primary-text transition-colors"
      >
        <X class="w-6 h-6" />
      </button>

      <h2 class="text-2xl font-bold text-primary-text mb-6 flex items-center gap-2">
        <span
          class="w-8 h-8 bg-brand-teal rounded-full flex items-center justify-center text-white text-sm"
        >
          <FileText class="w-5 h-5" />
        </span>
        Patient Record
      </h2>

      <div class="flex-1 mb-6">
        <textarea
          :value="content"
          @input="$emit('update:content', $event.target.value)"
          class="w-full h-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal p-6 text-primary-text outline-none transition-colors resize-none leading-relaxed"
          placeholder="Enter patient record details..."
        ></textarea>
      </div>

      <div class="flex justify-end gap-3">
        <button
          @click="$emit('close')"
          class="px-6 py-3 rounded-xl font-bold text-primary-text/70 hover:bg-black/5 transition-colors"
        >
          Close
        </button>
        <button
          @click="$emit('save')"
          :disabled="saving"
          class="px-6 py-3 rounded-xl bg-brand-teal text-white font-bold hover:scale-[1.02] active:scale-95 transition-all shadow-sm border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed flex items-center gap-2"
        >
          <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
          {{ saving ? "Saving..." : "Save Changes" }}
        </button>
      </div>
    </div>
  </div>
</template>
