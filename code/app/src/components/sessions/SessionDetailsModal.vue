<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { X, FileText, User, Calendar, Clock, Play, ClipboardList, Loader2 } from "lucide-vue-next";
import type { SessionDto } from "@/services/sessionsService";

interface Props {
  session: SessionDto;
  openingRecord: string | null;
}

defineProps<Props>();

const emit = defineEmits<{
  close: [];
  openPatientRecord: [session: SessionDto];
}>();

const { t } = useI18n();

function getStatusColor(status: string): string {
  switch (status?.toLowerCase()) {
    case "inprogress":
      return "bg-brand-orange/20 text-brand-orange";
    case "completed":
      return "bg-brand-green/20 text-brand-green";
    case "cancelled":
      return "bg-red-100 text-red-700";
    default:
      return "bg-gray-100 text-gray-700";
  }
}

function translateStatus(status: string): string {
  const statusKey = status?.toLowerCase();
  const statusMap: Record<string, string> = {
    inprogress: "sessions.status_in_progress",
    completed: "sessions.status_completed",
    cancelled: "sessions.status_cancelled",
  };
  return statusMap[statusKey] ? t(statusMap[statusKey]) : status;
}

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

function calculateDuration(startedAt: string, endedAt: string): string {
  const start = new Date(startedAt);
  const end = new Date(endedAt);
  const diffMs = end.getTime() - start.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const hours = Math.floor(diffMins / 60);
  const mins = diffMins % 60;
  return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
}
</script>

<template>
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-lg shadow-2xl border-4 border-white relative"
    >
      <button
        @click="emit('close')"
        class="absolute top-6 right-6 text-primary-text/50 hover:text-primary-text transition-colors"
      >
        <X class="w-6 h-6" />
      </button>

      <h2 class="text-2xl font-bold text-primary-text mb-6 flex items-center gap-3">
        <span class="w-10 h-10 bg-brand-teal/20 rounded-full flex items-center justify-center">
          <FileText class="w-5 h-5 text-brand-teal" />
        </span>
        {{ t("sessions.details_title") }}
      </h2>

      <div class="mb-6">
        <h3 class="text-sm font-bold text-primary-text/60 uppercase tracking-wide mb-2">
          {{ t("sessions.student_info") }}
        </h3>
        <div class="bg-white rounded-xl p-4 border border-stone-200">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 bg-brand-teal/20 rounded-full flex items-center justify-center">
              <User class="w-6 h-6 text-brand-teal" />
            </div>
            <div>
              <p class="font-bold text-primary-text text-lg">{{ session.studentName }}</p>
              <span
                class="text-xs font-bold px-2 py-0.5 rounded-lg"
                :class="getStatusColor(session.status)"
              >
                {{ translateStatus(session.status) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div class="mb-6">
        <h3 class="text-sm font-bold text-primary-text/60 uppercase tracking-wide mb-2">
          {{ t("sessions.date_time") }}
        </h3>
        <div class="bg-white rounded-xl p-4 border border-stone-200 space-y-3">
          <div class="flex items-center gap-3">
            <Calendar class="w-5 h-5 text-brand-orange" />
            <span class="text-primary-text">{{ formatDate(session.startedAt) }}</span>
          </div>
          <div class="flex items-center gap-3">
            <Clock class="w-5 h-5 text-brand-orange" />
            <span class="text-primary-text">
              {{ formatTime(session.startedAt) }}
              <template v-if="session.endedAt"> - {{ formatTime(session.endedAt) }} </template>
            </span>
          </div>
          <div v-if="session.endedAt" class="flex items-center gap-3">
            <Play class="w-5 h-5 text-brand-teal" />
            <span class="text-primary-text font-medium">
              {{ t("sessions.duration") }}:
              {{ calculateDuration(session.startedAt, session.endedAt) }}
            </span>
          </div>
        </div>
      </div>

      <div class="mb-6">
        <h3 class="text-sm font-bold text-primary-text/60 uppercase tracking-wide mb-2">
          {{ t("sessions.notes") }}
        </h3>
        <div class="bg-white rounded-xl p-4 border border-stone-200 min-h-[100px]">
          <p v-if="session.notes" class="text-primary-text whitespace-pre-wrap">
            {{ session.notes }}
          </p>
          <p v-else class="text-primary-text/40 italic">
            {{ t("sessions.no_notes") }}
          </p>
        </div>
      </div>

      <div class="flex gap-3">
        <button
          @click="emit('close')"
          class="flex-1 py-3 rounded-xl border-2 border-stone-300 text-primary-text font-bold hover:bg-stone-100 transition-all"
        >
          {{ t("common.close") }}
        </button>
        <button
          @click="
            emit('close');
            emit('openPatientRecord', session);
          "
          :disabled="openingRecord === session.id"
          class="flex-1 py-3 rounded-xl bg-brand-green text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all disabled:opacity-70"
        >
          <Loader2 v-if="openingRecord === session.id" class="w-5 h-5 animate-spin" />
          <ClipboardList v-else class="w-5 h-5" />
          {{ t("sessions.patient_record") }}
        </button>
      </div>
    </div>
  </div>
</template>
