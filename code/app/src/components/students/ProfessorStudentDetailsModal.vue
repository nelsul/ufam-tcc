<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { X, Eye, Loader2 } from "lucide-vue-next";
import type { StudentDto, PatientObservationDto } from "@/types/api";

const { t, locale } = useI18n();

defineProps<{
  show: boolean;
  student: StudentDto | null;
  loading: boolean;
  observations: PatientObservationDto[];
}>();

const _emit = defineEmits<{
  (e: "close"): void;
}>();
</script>

<template>
  <div
    v-if="show && student"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm"
  >
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-2xl shadow-2xl border-4 border-white relative max-h-[90vh] overflow-y-auto flex flex-col gap-8"
    >
      <button
        @click="$emit('close')"
        class="absolute top-6 right-6 text-[#5E5340]/50 hover:text-[#5E5340] transition-colors z-10"
      >
        <X class="w-6 h-6" />
      </button>

      <div class="pr-12">
        <h2 class="text-3xl font-bold text-[#5E5340] mb-1">
          {{ student.name }}
        </h2>
        <div class="flex items-center gap-2 text-[#5E5340]/60">
          <p>{{ student.fullName }}</p>
          <span class="w-1 h-1 rounded-full bg-[#5E5340]/30"></span>
          <span
            class="text-xs font-bold px-2 py-0.5 rounded-full"
            :class="
              student.status === 'Active'
                ? 'bg-[#78D879]/20 text-[#78D879]'
                : 'bg-stone-200 text-stone-500'
            "
          >
            {{ student.status === "Active" ? t("common.active") : t("common.inactive") }}
          </span>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="md:col-span-2 bg-[#FDFDF5] rounded-2xl p-3 shadow-sm border border-stone-200">
          <p class="text-[#F6A95B] text-xs uppercase tracking-wide font-bold mb-1">
            {{ t("common.email") }}
          </p>
          <p class="text-[#5E5340] text-lg font-bold truncate">
            {{ student.institutionalEmail }}
          </p>
        </div>
        <div class="bg-[#FDFDF5] rounded-2xl p-3 shadow-sm border border-stone-200">
          <p class="text-[#F6A95B] text-xs uppercase tracking-wide font-bold mb-1">
            {{ t("common.registration") }}
          </p>
          <p class="text-[#5E5340] text-lg font-bold">
            {{ student.registration }}
          </p>
        </div>
      </div>

      <div class="flex flex-col gap-2">
        <div class="flex items-center gap-3 mb-4">
          <div class="flex items-center gap-2">
            <Eye class="w-5 h-5 text-brand-orange" />
            <h3 class="text-xl font-bold text-[#5E5340]">{{ t("patient_observations.title") }}</h3>
          </div>
          <div class="h-px flex-1 border-t-2 border-dashed border-[#E9E7D8]"></div>
        </div>

        <div
          class="bg-[#FDFDF5] rounded-2xl border-2 border-dashed border-[#E9E7D8] p-6 min-h-[100px]"
        >
          <div v-if="loading" class="flex justify-center py-4">
            <Loader2 class="w-6 h-6 animate-spin text-brand-orange" />
          </div>
          <div
            v-else-if="!observations || observations.length === 0"
            class="flex flex-col items-center justify-center h-full py-4 text-[#5E5340]/40"
          >
            <p class="text-sm font-medium">{{ t("patient_observations.no_observations") }}</p>
          </div>
          <div v-else class="flex flex-col gap-2">
            <div
              v-for="obs in observations"
              :key="obs.id"
              class="w-full flex justify-between items-start bg-white rounded-xl p-3 shadow-sm border border-stone-100"
            >
              <div class="flex-1">
                <div class="flex justify-between items-start mb-1">
                  <span class="font-bold text-[#5E5340] text-sm">{{ obs.observationName }}</span>
                  <span
                    class="text-[10px] font-bold text-[#5E5340]/40 bg-stone-100 px-2 py-0.5 rounded-full"
                  >
                    {{ new Date(obs.createdAt).toLocaleDateString(locale) }}
                  </span>
                </div>
                <p class="text-xs text-[#5E5340]/70">
                  {{ obs.notes }}
                </p>
                <p class="text-[10px] text-[#5E5340]/40 mt-2">
                  {{ t("patient_observations.by_professional", { name: obs.professionalName }) }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
