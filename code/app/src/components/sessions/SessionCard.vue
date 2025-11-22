<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { User, Calendar, Clock, FileText, ClipboardList, Loader2 } from "lucide-vue-next";
import type { SessionDto } from "@/services/sessionsService";

interface Props {
  session: SessionDto;
  openingRecord: string | null;
}

defineProps<Props>();

const emit = defineEmits<{
  click: [];
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
  <div
    @click="emit('click')"
    class="bg-white rounded-2xl p-4 border-2 border-stone-200 hover:border-brand-teal/30 transition-all cursor-pointer"
  >
    <div class="flex items-start justify-between mb-3">
      <div class="flex items-center gap-2">
        <User class="w-4 h-4 text-brand-teal" />
        <span class="font-bold text-primary-text">{{ session.studentName }}</span>
      </div>
      <span class="text-xs font-bold px-2 py-1 rounded-lg" :class="getStatusColor(session.status)">
        {{ translateStatus(session.status) }}
      </span>
    </div>

    <div class="flex flex-col gap-1 text-sm text-primary-text/70">
      <span class="flex items-center gap-1">
        <Calendar class="w-3 h-3" />
        {{ formatDate(session.startedAt) }}
      </span>
      <span class="flex items-center gap-1">
        <Clock class="w-3 h-3" />
        {{ formatTime(session.startedAt) }}
        <template v-if="session.endedAt"> - {{ formatTime(session.endedAt) }} </template>
      </span>
      <span v-if="session.endedAt" class="flex items-center gap-1 font-medium text-brand-teal">
        {{ calculateDuration(session.startedAt, session.endedAt) }}
      </span>
    </div>

    <div v-if="session.notes" class="mt-3 pt-3 border-t border-stone-200">
      <div class="flex items-start gap-1 text-sm text-primary-text/60">
        <FileText class="w-3 h-3 mt-0.5 flex-shrink-0" />
        <span class="line-clamp-2">{{ session.notes }}</span>
      </div>
    </div>

    <div class="mt-3 pt-3 border-t border-stone-200">
      <button
        @click.stop="emit('openPatientRecord', session)"
        :disabled="openingRecord === session.id"
        class="w-full py-2 px-3 bg-brand-green/10 text-brand-green rounded-lg font-medium text-sm flex items-center justify-center gap-2 hover:bg-brand-green/20 transition-all disabled:opacity-70"
      >
        <Loader2 v-if="openingRecord === session.id" class="w-4 h-4 animate-spin" />
        <ClipboardList v-else class="w-4 h-4" />
        {{ t("sessions.patient_record") }}
      </button>
    </div>
  </div>
</template>
