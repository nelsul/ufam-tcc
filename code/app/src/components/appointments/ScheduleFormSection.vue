<script setup lang="ts">
import { useI18n } from "vue-i18n";
import type { SessionTypeDto } from "@/types/api";

defineProps<{
  sessionTypes: SessionTypeDto[];
}>();

const editForm = defineModel<{
  startDate: string;
  startTime: string;
  endDate: string;
  endTime: string;
  sessionTypeId: string;
}>("editForm", { required: true });

const emit = defineEmits<{
  endTimeEdited: [];
}>();

const { t } = useI18n();

const handleEndTimeInput = () => {
  emit("endTimeEdited");
};
</script>

<template>
  <div class="bg-brand-teal/10 rounded-2xl p-4 flex flex-col gap-2">
    <h3 class="font-bold text-primary-text">{{ t("appointments.schedule_session") }}</h3>

    <div class="grid grid-cols-2 gap-3">
      <div>
        <label class="text-sm font-medium text-primary-text/70 block mb-1">
          {{ t("date") }}
        </label>
        <input
          type="date"
          v-model="editForm.startDate"
          class="w-full px-3 py-2 rounded-xl border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>
      <div>
        <label class="text-sm font-medium text-primary-text/70 block mb-1">
          {{ t("common.start_time") }}
        </label>
        <input
          type="time"
          v-model="editForm.startTime"
          class="w-full px-3 py-2 rounded-xl border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>
    </div>

    <div>
      <label class="text-sm font-medium text-primary-text/70 block mb-1">
        {{ t("session_type") }}
      </label>
      <select
        v-model="editForm.sessionTypeId"
        class="w-full px-3 py-2 rounded-xl border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm bg-white"
      >
        <option value="">{{ t("appointments.select_session_type_placeholder") }}</option>
        <option v-for="st in sessionTypes" :key="st.publicId" :value="st.publicId">
          {{ st.name }} ({{ st.durationMinutes }} min)
        </option>
      </select>
    </div>

    <div class="grid grid-cols-2 gap-3">
      <div>
        <label class="text-sm font-medium text-primary-text/70 block mb-1">
          {{ t("common.end_date") }}
        </label>
        <input
          type="date"
          v-model="editForm.endDate"
          @input="handleEndTimeInput"
          class="w-full px-3 py-2 rounded-xl border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>
      <div>
        <label class="text-sm font-medium text-primary-text/70 block mb-1">
          {{ t("common.end_time") }}
        </label>
        <input
          type="time"
          v-model="editForm.endTime"
          @input="handleEndTimeInput"
          class="w-full px-3 py-2 rounded-xl border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>
    </div>
    <p class="text-xs text-primary-text/50">
      {{ t("appointments.end_time_auto_calculated") }}
    </p>
  </div>
</template>
