<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { FileText, Loader2 } from "lucide-vue-next";
import type { PatientRecordDto } from "@/types/api";

const { t, locale } = useI18n();

defineProps<{
  record: PatientRecordDto | null;
  loading: boolean;
}>();

defineEmits<{
  (e: "open-record"): void;
}>();
</script>

<template>
  <div>
    <div class="flex items-center gap-3 mb-4">
      <div class="flex items-center gap-2">
        <FileText class="w-5 h-5 text-brand-teal" />
        <h3 class="text-xl font-bold text-[#5E5340]">{{ t("patient_record.title") }}</h3>
      </div>
      <div class="h-px flex-1 border-t-2 border-dashed border-[#E9E7D8]"></div>
    </div>

    <div class="bg-[#FDFDF5] p-6 rounded-2xl border-2 border-dashed border-[#E9E7D8]">
      <div v-if="loading" class="flex justify-center py-4">
        <Loader2 class="w-6 h-6 animate-spin text-brand-teal" />
      </div>
      <div v-else class="flex items-center justify-between">
        <div>
          <p class="font-bold text-[#5E5340] mb-1">
            {{ record ? t("patient_record.record_exists") : t("patient_record.no_record") }}
          </p>
          <p class="text-sm text-[#5E5340]/60">
            {{
              record
                ? t("patient_record.last_updated") +
                  " " +
                  new Date(record.updatedAt).toLocaleDateString(locale)
                : t("patient_record.create_record")
            }}
          </p>
        </div>
        <button
          @click="$emit('open-record')"
          class="bg-brand-teal text-white px-4 py-2 rounded-xl font-bold hover:scale-105 transition-transform shadow-sm text-sm"
        >
          {{ record ? t("patient_record.open_record") : t("patient_record.create") }}
        </button>
      </div>
    </div>
  </div>
</template>
