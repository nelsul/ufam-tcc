<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { User, Clock, Play, ClipboardList, Square, Loader2, Calendar } from "lucide-vue-next";
import type { SessionDto } from "@/services/sessionsService";

interface Props {
  session: SessionDto;
  openingRecord: string | null;
}

defineProps<Props>();

const emit = defineEmits<{
  openPatientRecord: [session: SessionDto];
  openEndModal: [session: SessionDto];
}>();

const { t } = useI18n();

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString([], {
    weekday: "short",
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

function formatTime(dateStr: string): string {
  return new Date(dateStr).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

function calculateDuration(startedAt: string): string {
  const start = new Date(startedAt);
  const now = new Date();
  const diffMs = now.getTime() - start.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const hours = Math.floor(diffMins / 60);
  const mins = diffMins % 60;
  return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
}
</script>

<template>
  <div class="mb-6">
    <h2 class="text-lg font-bold text-primary-text mb-4 flex items-center gap-2">
      <Play class="w-5 h-5 text-brand-orange" />
      {{ t("sessions.current_session") }}
    </h2>
    <div class="bg-brand-orange/10 rounded-3xl p-6 border-2 border-brand-orange/30">
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div class="flex-1">
          <div class="flex items-center gap-3 mb-2">
            <User class="w-5 h-5 text-brand-orange" />
            <span class="font-bold text-primary-text text-lg">{{ session.studentName }}</span>
          </div>
          <div class="flex flex-wrap items-center gap-4 text-sm text-primary-text/70">
            <span class="flex items-center gap-1">
              <Calendar class="w-4 h-4" />
              {{ formatDate(session.startedAt) }}
            </span>
            <span class="flex items-center gap-1">
              <Clock class="w-4 h-4" />
              {{ t("sessions.started_at") }}: {{ formatTime(session.startedAt) }}
            </span>
            <span class="flex items-center gap-1 font-medium text-brand-orange">
              <Play class="w-4 h-4" />
              {{ calculateDuration(session.startedAt) }}
            </span>
          </div>
        </div>
        <div class="flex flex-col sm:flex-row gap-2">
          <button
            @click="emit('openPatientRecord', session)"
            :disabled="openingRecord === session.id"
            class="px-6 py-3 bg-brand-green text-white rounded-xl font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md disabled:opacity-70"
          >
            <Loader2 v-if="openingRecord === session.id" class="w-5 h-5 animate-spin" />
            <ClipboardList v-else class="w-5 h-5" />
            {{ t("sessions.patient_record") }}
          </button>
          <button
            @click="emit('openEndModal', session)"
            class="px-6 py-3 bg-brand-teal text-white rounded-xl font-bold flex items-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md"
          >
            <Square class="w-5 h-5" />
            {{ t("sessions.end_session") }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
