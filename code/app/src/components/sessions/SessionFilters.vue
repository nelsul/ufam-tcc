<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { Search, X } from "lucide-vue-next";

interface Props {
  search: string;
  dateFrom: string;
  dateTo: string;
  maxDateTo?: string;
  minDateFrom?: string;
  hasActiveFilters?: boolean;
}

defineProps<Props>();

const emit = defineEmits<{
  "update:search": [value: string];
  "update:dateFrom": [value: string];
  "update:dateTo": [value: string];
  clearFilters: [];
}>();

const { t } = useI18n();
</script>

<template>
  <div class="bg-white rounded-2xl p-4 border-2 border-stone-200 mb-4">
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="md:col-span-2">
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("sessions.search_label") }}
        </label>
        <div class="relative">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-primary-text/40" />
          <input
            :value="search"
            @input="emit('update:search', ($event.target as HTMLInputElement).value)"
            type="text"
            :placeholder="t('sessions.search_placeholder')"
            class="w-full pl-10 pr-4 py-2 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
          />
        </div>
      </div>

      <div>
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("sessions.date_from") }}
        </label>
        <input
          :value="dateFrom"
          @input="emit('update:dateFrom', ($event.target as HTMLInputElement).value)"
          type="date"
          :max="dateTo || undefined"
          :min="minDateFrom || undefined"
          class="w-full px-3 py-2 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("sessions.date_to") }}
        </label>
        <input
          :value="dateTo"
          @input="emit('update:dateTo', ($event.target as HTMLInputElement).value)"
          type="date"
          :min="dateFrom || undefined"
          :max="maxDateTo || undefined"
          class="w-full px-3 py-2 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
        />
      </div>
    </div>

    <div v-if="hasActiveFilters" class="mt-3 flex justify-end">
      <button
        @click="emit('clearFilters')"
        class="text-sm text-primary-text/60 hover:text-primary-text flex items-center gap-1 transition-colors"
      >
        <X class="w-4 h-4" />
        {{ t("common.clear_filters") }}
      </button>
    </div>
  </div>
</template>
