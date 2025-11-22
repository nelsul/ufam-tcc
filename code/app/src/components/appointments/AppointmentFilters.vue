<script setup lang="ts">
import { Search, X } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import type { SessionTypeDto } from "@/types/api";

defineProps<{
  sessionTypes: SessionTypeDto[];
}>();

const filters = defineModel<{
  status: string;
  sessionTypeId: string;
  search: string;
}>("filters", { required: true });

const emit = defineEmits<{
  search: [];
  filterChange: [];
  clear: [];
}>();

const { t } = useI18n();

let searchTimeout: ReturnType<typeof setTimeout> | null = null;

const handleSearchInput = () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    emit("search");
  }, 300);
};

const hasActiveFilters = () => {
  return filters.value.search || filters.value.status || filters.value.sessionTypeId;
};
</script>

<template>
  <div class="mb-8 bg-white rounded-2xl p-4 border border-soft-gray">
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="md:col-span-2">
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("common.search") }}
        </label>
        <div class="relative">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-primary-text/40" />
          <input
            v-model="filters.search"
            type="text"
            :placeholder="t('appointments.search_placeholder')"
            class="w-full pl-10 pr-4 py-2 rounded-xl border border-soft-gray focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
            @input="handleSearchInput"
          />
        </div>
      </div>

      <div>
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("common.status") }}
        </label>
        <select
          v-model="filters.status"
          class="w-full px-3 py-2 rounded-xl border border-soft-gray focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
          @change="emit('filterChange')"
        >
          <option value="">{{ t("appointments.all_statuses") }}</option>
          <option value="Pending">{{ t("appointments.pending") }}</option>
          <option value="Confirmed">{{ t("appointments.confirmed") }}</option>
          <option value="InSession">{{ t("appointments.in_session") }}</option>
          <option value="Completed">{{ t("appointments.completed") }}</option>
          <option value="Cancelled">{{ t("appointments.cancelled") }}</option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium text-primary-text/70 mb-1">
          {{ t("session_type") }}
        </label>
        <select
          v-model="filters.sessionTypeId"
          class="w-full px-3 py-2 rounded-xl border border-soft-gray focus:outline-none focus:ring-2 focus:ring-brand-teal text-sm"
          @change="emit('filterChange')"
        >
          <option value="">{{ t("appointments.all_types") }}</option>
          <option v-for="st in sessionTypes" :key="st.publicId" :value="st.publicId">
            {{ st.name }}
          </option>
        </select>
      </div>
    </div>

    <div v-if="hasActiveFilters()" class="mt-3 flex justify-end">
      <button
        @click="emit('clear')"
        class="text-sm text-brand-teal hover:text-brand-teal/80 font-medium flex items-center gap-1"
      >
        <X class="w-4 h-4" />
        {{ t("common.clear_filters") }}
      </button>
    </div>
  </div>
</template>
