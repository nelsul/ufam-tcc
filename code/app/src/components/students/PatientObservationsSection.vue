<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { Eye, Search, Loader2, Trash2 } from "lucide-vue-next";
import type { PatientObservationWithId, ObservationWithId } from "@/types/api";

const { t, locale } = useI18n();

defineProps<{
  observations: PatientObservationWithId[];
  loading: boolean;
  searchQuery: string;
  availableObservations: ObservationWithId[];
}>();

const _emit = defineEmits<{
  (e: "update:searchQuery", value: string): void;
  (e: "search"): void;
  (e: "add-observation", obs: ObservationWithId): void;
  (e: "edit-observation", obs: PatientObservationWithId): void;
  (e: "delete-observation", id: string): void;
}>();
</script>

<template>
  <div class="flex flex-col gap-2">
    <div class="flex items-center gap-3 mb-4">
      <div class="flex items-center gap-2">
        <Eye class="w-5 h-5 text-brand-orange" />
        <h3 class="text-xl font-bold text-[#5E5340]">{{ t("patient_observations.title") }}</h3>
      </div>
      <div class="h-px flex-1 border-t-2 border-dashed border-[#E9E7D8]"></div>
    </div>

    <div class="mb-4 relative">
      <div class="flex gap-2">
        <div class="relative flex-1">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#5E5340]/40" />
          <input
            :value="searchQuery"
            @input="
              $emit('update:searchQuery', ($event.target as HTMLInputElement).value);
              $emit('search');
            "
            type="text"
            :placeholder="t('patient_observations.search_placeholder')"
            class="w-full bg-[#FDFDF5] rounded-full border-none shadow-inner pl-10 pr-4 py-2 text-sm text-[#5E5340] outline-none transition-colors placeholder-[#5E5340]/30"
          />
        </div>
      </div>

      <div
        v-if="availableObservations && availableObservations.length > 0"
        class="absolute top-full left-0 right-0 mt-2 bg-white rounded-xl shadow-xl border-2 border-[#E9E7D8] z-20 overflow-hidden"
      >
        <div
          v-for="obs in availableObservations"
          :key="obs.id"
          class="p-3 hover:bg-[#FDFDF5] border-b border-[#E9E7D8] last:border-0 flex justify-between items-center"
        >
          <div>
            <p class="font-bold text-sm text-[#5E5340]">
              {{ obs.name }}
            </p>
            <p class="text-xs text-[#5E5340]/60">
              {{ obs.description }}
            </p>
          </div>
          <button
            @click="$emit('add-observation', obs)"
            class="text-xs bg-brand-orange text-white px-3 py-1.5 rounded-lg font-bold hover:bg-brand-orange/90 transition-colors"
          >
            {{ t("patient_observations.add_note") }}
          </button>
        </div>
      </div>
    </div>

    <div class="bg-[#FDFDF5] rounded-2xl border-2 border-dashed border-[#E9E7D8] p-6 min-h-[100px]">
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
          class="w-full flex justify-between items-center bg-white rounded-xl p-3 shadow-sm border border-stone-100 hover:border-brand-orange/50 transition-colors cursor-pointer group"
          @click="$emit('edit-observation', obs)"
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
            <p class="text-xs text-[#5E5340]/70 line-clamp-2">
              {{ obs.notes }}
            </p>
          </div>

          <button
            @click.stop="$emit('delete-observation', obs.id)"
            class="ml-3 p-2 text-stone-300 hover:text-red-500 hover:bg-red-50 rounded-full transition-all hover:animate-pulse"
            :title="t('patient_observations.delete_tooltip')"
          >
            <Trash2 class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
