<script setup lang="ts">
import { Calendar, Clock } from "lucide-vue-next";
import { useI18n } from "vue-i18n";

interface FormattedAppointment {
  id: string;
  name: string;
  time: string;
  date: string;
  type: string | null;
  status: string;
}

const emit = defineEmits<{
  click: [];
}>();

const { t } = useI18n();

defineProps<{
  appointment: FormattedAppointment;
}>();

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
  <div
    class="bg-white rounded-3xl p-5 border border-soft-gray shadow-sm hover:border-brand-teal hover:shadow-md transition-all group h-full flex flex-col"
  >
    <div class="flex-grow">
      <div class="flex justify-between items-start mb-4">
        <div class="flex-1 min-w-0 mr-2">
          <h3 class="font-bold text-xl text-primary-text line-clamp-2">{{ appointment.name }}</h3>
          <span
            v-if="appointment.type"
            class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-teal/20 text-primary-text/70 mt-1 inline-block"
          >
            {{ appointment.type }}
          </span>
          <span
            v-else
            class="text-xs font-medium px-2 py-1 rounded-lg bg-slate-100 text-slate-600 mt-1 inline-block"
          >
            {{ t("appointments.no_session_type") }}
          </span>
        </div>
        <span
          class="text-xs font-bold px-2 py-1 rounded-lg shrink-0"
          :class="getStatusColor(appointment.status)"
        >
          {{ translateStatus(appointment.status) }}
        </span>
      </div>

      <div class="space-y-2 mb-4">
        <div class="flex items-center gap-2 text-primary-text/80">
          <Calendar class="w-4 h-4 shrink-0" />
          <span class="text-sm font-medium">{{ appointment.date }}</span>
        </div>
        <div class="flex items-center gap-2 text-primary-text/80">
          <Clock class="w-4 h-4 shrink-0" />
          <span class="text-sm font-medium">{{ appointment.time }}</span>
        </div>
      </div>
    </div>

    <div class="mt-auto">
      <button
        @click="emit('click')"
        class="w-full py-3 rounded-xl bg-brand-teal text-white font-bold flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-sm border-2 border-white"
      >
        {{ t("appointments.view_details") }}
      </button>
    </div>
  </div>
</template>
