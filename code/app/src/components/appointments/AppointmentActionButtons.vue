<script setup lang="ts">
import { Loader2, Save, CheckCircle, XCircle, Play } from "lucide-vue-next";
import { useI18n } from "vue-i18n";

defineProps<{
  status: string;
  saving: boolean;
  startingSession: boolean;
  hasLinkedStudent: boolean;
}>();

const emit = defineEmits<{
  confirm: [];
  cancel: [];
  save: [];
  startSession: [];
}>();

const { t } = useI18n();

const getStatusColor = (status: string) => {
  switch (status?.toLowerCase()) {
    case "confirmed":
      return "bg-[#D4EAC8] text-[#2F5930]";
    case "pending":
      return "bg-[#FBF3C3] text-[#795C34]";
    case "insession":
      return "bg-purple-100 text-purple-700";
    case "cancelled":
      return "bg-red-100 text-red-700";
    case "completed":
      return "bg-blue-100 text-blue-700";
    default:
      return "bg-gray-100 text-gray-700";
  }
};

const translateStatus = (status: string) => {
  const statusKey = status?.toLowerCase();
  const statusMap: Record<string, string> = {
    pending: "appointments.pending",
    confirmed: "appointments.confirmed",
    insession: "appointments.in_session",
    completed: "appointments.completed",
    cancelled: "appointments.cancelled",
  };
  return statusMap[statusKey] ? t(statusMap[statusKey]) : status;
};
</script>

<template>
  <div class="flex flex-col gap-3">
    <div class="flex items-center justify-between">
      <span class="text-sm text-primary-text/60">{{ t("common.status") }}</span>
      <span class="text-sm font-bold px-3 py-1 rounded-lg" :class="getStatusColor(status)">
        {{ translateStatus(status) }}
      </span>
    </div>

    <div v-if="status?.toLowerCase() === 'pending'" class="grid grid-cols-2 gap-3">
      <button
        @click="emit('confirm')"
        :disabled="saving"
        class="py-3 rounded-xl bg-brand-green text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed"
      >
        <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
        <CheckCircle v-else class="w-5 h-5" />
        {{ t("common.confirm") }}
      </button>
      <button
        @click="emit('cancel')"
        :disabled="saving"
        class="py-3 rounded-xl border-2 border-red-400 text-red-500 bg-transparent font-bold flex items-center justify-center gap-2 hover:bg-red-50 active:scale-95 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
      >
        <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
        <XCircle v-else class="w-5 h-5" />
        {{ t("common.cancel") }}
      </button>
    </div>

    <button
      v-if="status?.toLowerCase() === 'confirmed'"
      @click="emit('cancel')"
      :disabled="saving"
      class="w-full py-3 rounded-xl border-2 border-red-400 text-red-500 bg-transparent font-bold flex items-center justify-center gap-2 hover:bg-red-50 active:scale-95 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
    >
      <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
      <XCircle v-else class="w-5 h-5" />
      {{ t("appointments.cancel_appointment") }}
    </button>

    <button
      v-if="
        (status?.toLowerCase() === 'pending' || status?.toLowerCase() === 'confirmed') &&
        hasLinkedStudent
      "
      @click="emit('startSession')"
      :disabled="startingSession || saving"
      class="w-full py-3 rounded-xl bg-brand-orange text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed"
    >
      <Loader2 v-if="startingSession" class="w-5 h-5 animate-spin" />
      <Play v-else class="w-5 h-5" />
      {{ t("sessions.start_session") }}
    </button>

    <button
      v-if="status?.toLowerCase() === 'cancelled'"
      @click="emit('confirm')"
      :disabled="saving"
      class="w-full py-3 rounded-xl bg-brand-green text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed"
    >
      <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
      <CheckCircle v-else class="w-5 h-5" />
      {{ t("appointments.reactivate_confirm") }}
    </button>

    <button
      @click="emit('save')"
      :disabled="saving"
      class="w-full py-3 rounded-xl bg-brand-teal text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed"
    >
      <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
      <Save v-else class="w-5 h-5" />
      {{ saving ? t("common.saving") : t("common.save_changes") }}
    </button>
  </div>
</template>
