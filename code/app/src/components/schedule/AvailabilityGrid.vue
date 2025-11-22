<script setup lang="ts">
import { Calendar, Loader2, Frown } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import type { AvailabilityDay, AvailabilityTimeRange } from "@/types/api";

const { locale } = useI18n();

defineProps<{
  availabilities: AvailabilityDay[];
  loading: boolean;
  selectedSlot: string | null;
}>();

defineEmits<{
  (e: "select-slot", payload: { day: AvailabilityDay; slot: AvailabilityTimeRange }): void;
}>();

const formatDate = (dateValue: string | Date) => {
  try {
    let date: Date;

    if (typeof dateValue === "string") {
      if (dateValue.includes("T")) {
        date = new Date(dateValue);
      } else {
        const [year, month, day] = dateValue.split("-").map(Number);
        date = new Date(year, month - 1, day);
      }
    } else if (dateValue instanceof Date) {
      date = dateValue;
    } else {
      return "Invalid Date";
    }

    return date.toLocaleDateString(locale.value, {
      weekday: "short",
      month: "short",
      day: "numeric",
    });
  } catch (e) {
    console.error("Error formatting date:", dateValue, e);
    return "Invalid Date";
  }
};
</script>

<template>
  <div class="mt-10 sm:mt-12 pt-8 sm:pt-10 border-t-4 border-dashed border-soft-gray">
    <div
      class="bg-brand-yellow/20 px-6 py-4 sm:px-8 sm:py-5 rounded-3xl border-4 border-brand-yellow mb-8"
    >
      <h3 class="text-xl font-bold text-primary-text flex items-center gap-3">
        <Calendar class="h-6 w-6 text-brand-yellow" />
        {{ $t("available_time_slots") }}
      </h3>
    </div>
    <div v-if="loading" class="flex items-center justify-center py-10 text-primary-text">
      <Loader2 class="animate-spin h-8 w-8 text-brand-teal mr-3" />
      {{ $t("loading_availability") }}
    </div>
    <div
      v-else-if="availabilities.length === 0"
      class="text-center py-10 text-primary-text bg-canvas rounded-3xl border-4 border-dashed border-soft-gray"
    >
      <Frown class="h-12 w-12 mx-auto mb-3 text-soft-gray" />
      {{ $t("no_available_slots") }}
    </div>
    <div v-else class="flex flex-col gap-6">
      <div
        v-for="day in availabilities"
        :key="day.date"
        class="bg-white/40 rounded-[24px] p-4 border-2 border-white"
      >
        <div class="flex items-center justify-between mb-3 px-2">
          <span class="font-bold text-primary-text text-lg">{{ formatDate(day.date) }}</span>
        </div>
        <div class="grid grid-cols-3 gap-2">
          <button
            v-for="slot in day.timeRanges"
            :key="`${day.date}-${slot.start}`"
            type="button"
            class="py-2 px-1 rounded-xl text-xs font-bold transition-all duration-200 border-2 transform hover:scale-105 active:scale-95 text-center"
            :class="[
              selectedSlot === `${day.date}-${slot.start}`
                ? 'bg-brand-orange text-white border-brand-orange shadow-md'
                : 'bg-white text-primary-text border-soft-gray hover:bg-brand-yellow/20 hover:border-brand-yellow',
            ]"
            @click="$emit('select-slot', { day, slot })"
          >
            {{ slot.start }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
