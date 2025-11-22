<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { X } from "lucide-vue-next";
import SearchInput from "../form/SearchInput.vue";

defineProps<{
  search: string;
  includeInactive: boolean;
  searchPlaceholder?: string;
  showInactiveFilter?: boolean;
}>();

defineEmits<{
  (e: "update:search", value: string): void;
  (e: "update:includeInactive", value: boolean): void;
  (e: "clear"): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="mb-6 bg-white rounded-2xl p-4 border border-soft-gray">
    <div class="flex flex-col md:flex-row md:items-center gap-4">
      <div class="flex-1">
        <SearchInput
          :model-value="search"
          @update:model-value="$emit('update:search', $event)"
          :placeholder="searchPlaceholder"
          :show-clear-button="true"
          @clear="$emit('update:search', '')"
        />
      </div>

      <div v-if="showInactiveFilter !== false" class="flex items-center gap-3">
        <button
          type="button"
          @click="$emit('update:includeInactive', !includeInactive)"
          class="flex items-center gap-2 px-3 py-2 rounded-xl transition-all text-sm font-medium"
          :class="
            includeInactive
              ? 'bg-brand-teal/10 text-brand-teal'
              : 'text-primary-text/50 hover:text-primary-text/70 hover:bg-soft-gray/30'
          "
        >
          <span
            class="w-8 h-5 rounded-full relative transition-colors"
            :class="includeInactive ? 'bg-brand-teal' : 'bg-soft-gray'"
          >
            <span
              class="absolute top-0.5 w-4 h-4 rounded-full bg-white shadow-sm transition-all"
              :class="includeInactive ? 'left-3.5' : 'left-0.5'"
            />
          </span>
          <span>{{ t("common.show_inactive") }}</span>
        </button>

        <button
          v-if="search || includeInactive"
          @click="$emit('clear')"
          class="text-sm text-primary-text/40 hover:text-red-500 font-medium flex items-center gap-1 transition-colors"
        >
          <X class="w-4 h-4" />
        </button>
      </div>
    </div>
  </div>
</template>
