<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { ChevronLeft, ChevronRight, Leaf, X, Loader2, Save, Globe } from "lucide-vue-next";
import { professionalsService, type MyAvailabilityDto } from "../../services/professionalsService";
import { useI18n } from "vue-i18n";

interface SlotState {
  enabled: boolean;
  availabilityId?: string;
}

const loading = ref(false);
const saving = ref(false);
const currentDayIndex = ref(0);

const originalSlots = ref<Record<string, SlotState>>({});
const slots = ref<Record<string, SlotState>>({});
const existingAvailabilities = ref<MyAvailabilityDto[]>([]);

const userTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;

const timeSlots = Array.from({ length: 18 }, (_, i) => {
  const hour = 6 + i;
  return `${hour.toString().padStart(2, "0")}:00`;
});

const days = computed(() => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const result: Date[] = [];
  for (let i = 0; i < 7; i++) {
    const day = new Date(today);
    day.setDate(today.getDate() + i);
    result.push(day);
  }
  return result;
});

const currentDay = computed(() => days.value[currentDayIndex.value]);

const { t, locale } = useI18n();

const formatDate = (date: Date) => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const isToday = date.getTime() === today.getTime();

  if (isToday) {
    return t("availability.today");
  }

  const weekday = date.toLocaleDateString(locale.value, { weekday: "short" }).replace(/\./g, "");
  const month = date.toLocaleDateString(locale.value, { month: "short" }).replace(/\./g, "");
  const day = date.getDate();

  return `${weekday} ${month} ${day}`;
};

const formatDateShort = (date: Date) => {
  const weekday = date.toLocaleDateString(locale.value, { weekday: "short" }).replace(/\./g, "");
  const day = date.getDate();
  return `${weekday} ${day}`;
};

const getSlotKey = (date: Date, slot: string) => {
  const dateStr = date.toISOString().split("T")[0];
  return `${dateStr}-${slot}`;
};

const hasChanges = computed(() => {
  const originalKeys = Object.keys(originalSlots.value);
  const currentKeys = Object.keys(slots.value);

  const allKeys = new Set([...originalKeys, ...currentKeys]);
  for (const key of allKeys) {
    const original = originalSlots.value[key]?.enabled || false;
    const current = slots.value[key]?.enabled || false;
    if (original !== current) {
      return true;
    }
  }
  return false;
});

const fetchAvailabilities = async () => {
  loading.value = true;
  try {
    const data = await professionalsService.getMyAvailabilities();
    existingAvailabilities.value = data;

    const newSlots: Record<string, SlotState> = {};

    data.forEach((av) => {
      const start = new Date(av.startTime);
      const end = new Date(av.endTime);

      days.value.forEach((day) => {
        timeSlots.forEach((slot) => {
          const parts = slot.split(":").map(Number);
          const h = parts[0] ?? 0;
          const m = parts[1] ?? 0;
          const slotTime = new Date(day);
          slotTime.setHours(h, m, 0, 0);

          const slotEnd = new Date(slotTime);
          slotEnd.setHours(slotEnd.getHours() + 1);

          if (slotTime >= start && slotTime < end) {
            const key = getSlotKey(day, slot);
            newSlots[key] = { enabled: true, availabilityId: av.publicId };
          }
        });
      });
    });

    slots.value = { ...newSlots };
    originalSlots.value = { ...newSlots };
  } catch (err) {
    console.error("Failed to fetch availabilities:", err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchAvailabilities();
});

const toggleSlot = (day: Date, slot: string) => {
  const key = getSlotKey(day, slot);
  const current = slots.value[key];
  if (current?.enabled) {
    slots.value[key] = { enabled: false };
  } else {
    slots.value[key] = { enabled: true };
  }
};

const isSlotEnabled = (day: Date, slot: string) => {
  const key = getSlotKey(day, slot);
  return slots.value[key]?.enabled || false;
};

const saveChanges = async () => {
  saving.value = true;
  try {
    const toCreate: { date: Date; startSlot: string; endSlot: string }[] = [];
    const toDelete = new Set<string>();

    for (const key of Object.keys(originalSlots.value)) {
      const original = originalSlots.value[key];
      const current = slots.value[key];
      if (original?.enabled && !current?.enabled && original.availabilityId) {
        toDelete.add(original.availabilityId);
      }
    }

    for (const day of days.value) {
      const enabledSlotsForDay: string[] = [];

      for (const slot of timeSlots) {
        const key = getSlotKey(day, slot);
        if (slots.value[key]?.enabled) {
          enabledSlotsForDay.push(slot);
        }
      }

      let rangeStart: string | null = null;
      let lastSlotIndex = -2;

      for (let i = 0; i < enabledSlotsForDay.length; i++) {
        const slot = enabledSlotsForDay[i];
        if (!slot) continue;
        const slotIndex = timeSlots.indexOf(slot);

        if (rangeStart === null) {
          rangeStart = slot;
          lastSlotIndex = slotIndex;
        } else if (slotIndex === lastSlotIndex + 1) {
          lastSlotIndex = slotIndex;
        } else {
          const endSlot = timeSlots[lastSlotIndex] ?? slot;
          toCreate.push({ date: day, startSlot: rangeStart, endSlot });
          rangeStart = slot;
          lastSlotIndex = slotIndex;
        }
      }

      if (rangeStart !== null && lastSlotIndex >= 0) {
        const endSlot = timeSlots[lastSlotIndex] ?? rangeStart;
        toCreate.push({ date: day, startSlot: rangeStart, endSlot });
      }
    }

    for (const av of existingAvailabilities.value) {
      await professionalsService.deleteMyAvailability(av.publicId);
    }

    for (const range of toCreate) {
      const startParts = range.startSlot.split(":").map(Number);
      const endParts = range.endSlot.split(":").map(Number);
      const startHour = startParts[0] ?? 0;
      const endHour = endParts[0] ?? 0;

      const startTime = new Date(range.date);
      startTime.setHours(startHour, 0, 0, 0);

      const endTime = new Date(range.date);
      endTime.setHours(endHour + 1, 0, 0, 0);

      await professionalsService.createMyAvailability({
        startTime: startTime.toISOString(),
        endTime: endTime.toISOString(),
      });
    }

    await fetchAvailabilities();
  } catch (err) {
    console.error("Failed to save availabilities:", err);
  } finally {
    saving.value = false;
  }
};

const nextDay = () => {
  if (currentDayIndex.value < days.value.length - 1) currentDayIndex.value++;
};

const prevDay = () => {
  if (currentDayIndex.value > 0) currentDayIndex.value--;
};
</script>

<template>
  <div class="h-full flex flex-col">
    <header class="mb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl md:text-4xl font-bold text-primary-text mb-2">
            {{ t("availability.title") }}
          </h1>
          <p class="text-primary-text/60">{{ t("availability.subtitle") }}</p>
        </div>
        <button
          @click="saveChanges"
          :disabled="!hasChanges || saving"
          class="flex items-center gap-2 px-4 py-2 rounded-xl font-semibold transition-all duration-200"
          :class="[
            hasChanges && !saving
              ? 'bg-brand-green text-white hover:bg-brand-green/90 shadow-md'
              : 'bg-soft-gray text-primary-text/30 cursor-not-allowed',
          ]"
        >
          <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
          <Save v-else class="w-5 h-5" />
          <span class="hidden sm:inline">{{ t("common.save_changes") }}</span>
        </button>
      </div>
      <div class="flex items-center gap-2 mt-3 text-sm text-primary-text/50">
        <Globe class="w-4 h-4" />
        <span>{{ t("availability.timezone") }} {{ userTimezone }}</span>
      </div>
    </header>

    <div class="md:hidden flex items-center justify-between mb-4 bg-white/50 p-2 rounded-2xl">
      <button
        @click="prevDay"
        :disabled="currentDayIndex === 0"
        class="p-2 rounded-xl hover:bg-white disabled:opacity-30"
      >
        <ChevronLeft class="w-5 h-5 text-primary-text" />
      </button>
      <span v-if="currentDay" class="text-lg font-bold text-primary-text">{{
        formatDate(currentDay)
      }}</span>
      <button
        @click="nextDay"
        :disabled="currentDayIndex === days.length - 1"
        class="p-2 rounded-xl hover:bg-white disabled:opacity-30"
      >
        <ChevronRight class="w-5 h-5 text-primary-text" />
      </button>
    </div>

    <div class="flex-1 overflow-y-auto relative">
      <div
        v-if="loading"
        class="absolute inset-0 flex items-center justify-center bg-white/50 z-10"
      >
        <Loader2 class="w-10 h-10 animate-spin text-brand-teal" />
      </div>

      <div class="hidden md:grid grid-cols-8 gap-1">
        <div class="flex flex-col gap-0.5 pt-8">
          <div
            v-for="slot in timeSlots"
            :key="slot"
            class="h-7 flex items-center justify-end pr-1 text-xs font-bold text-primary-text/50"
          >
            {{ slot }}
          </div>
        </div>

        <div v-for="day in days" :key="day.toISOString()" class="flex flex-col gap-0.5">
          <div
            class="text-center font-bold text-xs text-primary-text mb-0.5 h-7 flex items-center justify-center"
          >
            {{ formatDateShort(day) }}
          </div>
          <button
            v-for="slot in timeSlots"
            :key="getSlotKey(day, slot)"
            @click="toggleSlot(day, slot)"
            class="h-7 rounded border transition-all duration-200 flex items-center justify-center relative group"
            :class="[
              isSlotEnabled(day, slot)
                ? 'bg-brand-green border-brand-green text-white shadow-sm'
                : 'bg-soft-gray border-transparent text-primary-text/30 hover:bg-white',
            ]"
          >
            <Leaf v-if="isSlotEnabled(day, slot)" class="w-3 h-3" />
            <X v-else class="w-3 h-3" />
          </button>
        </div>
      </div>

      <div v-if="currentDay" class="md:hidden grid grid-cols-3 gap-2">
        <button
          v-for="slot in timeSlots"
          :key="getSlotKey(currentDay, slot)"
          @click="toggleSlot(currentDay, slot)"
          class="h-10 rounded-xl border-2 transition-all duration-200 flex items-center justify-between px-3"
          :class="[
            isSlotEnabled(currentDay, slot)
              ? 'bg-brand-green border-brand-green text-white shadow-md'
              : 'bg-soft-gray border-transparent text-primary-text/30',
          ]"
        >
          <span class="font-bold text-sm">{{ slot }}</span>
          <Leaf v-if="isSlotEnabled(currentDay, slot)" class="w-4 h-4" />
          <X v-else class="w-4 h-4" />
        </button>
      </div>
    </div>
  </div>
</template>
