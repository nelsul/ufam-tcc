<script setup lang="ts">
import { X, Loader2, FileText } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import type { SessionTypeDto, StudentDto } from "@/types/api";
import type { AppointmentDto } from "../../services/appointmentsService";
import PatientInfoSection from "./PatientInfoSection.vue";
import LinkStudentSection from "./LinkStudentSection.vue";
import ScheduleFormSection from "./ScheduleFormSection.vue";
import AppointmentActionButtons from "./AppointmentActionButtons.vue";

defineProps<{
  show: boolean;
  loading: boolean;
  saving: boolean;
  startingSession: boolean;
  appointment: AppointmentDto | null;
  sessionTypes: SessionTypeDto[];
  searchingStudents: boolean;
  studentSearchResults: StudentDto[];
  creatingStudent: boolean;
  linkedStudent?: StudentDto | null;
}>();

const selectedStudentId = defineModel<string | null>("selectedStudentId", { required: true });
const studentSearchQuery = defineModel<string>("studentSearchQuery", { required: true });
const editForm = defineModel<{
  startDate: string;
  startTime: string;
  endDate: string;
  endTime: string;
  sessionTypeId: string;
}>("editForm", { required: true });

const emit = defineEmits<{
  close: [];
  save: [];
  confirm: [];
  cancel: [];
  startSession: [];
  searchStudents: [];
  createStudent: [
    data: { name: string; fullName: string; institutionalEmail: string; registration: string },
  ];
  endTimeEdited: [];
}>();

const { t } = useI18n();
</script>

<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm"
  >
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-lg shadow-2xl border-4 border-white relative max-h-[90vh] overflow-y-auto scrollbar-hide"
    >
      <button
        @click="emit('close')"
        class="absolute top-6 right-6 text-[#5E5340]/50 hover:text-[#5E5340] transition-colors"
      >
        <X class="w-6 h-6" />
      </button>

      <h2 class="text-2xl font-bold text-primary-text mb-6">
        {{ t("appointments.details_title") }}
      </h2>

      <div v-if="loading" class="flex items-center justify-center py-10">
        <Loader2 class="w-10 h-10 animate-spin text-brand-teal" />
      </div>

      <div v-else-if="appointment" class="flex flex-col gap-3">
        <PatientInfoSection
          :full-name="appointment.studentFullName"
          :registration="appointment.studentRegistration"
          :email="appointment.studentEmail"
        />

        <LinkStudentSection
          v-model:selected-student-id="selectedStudentId"
          v-model:search-query="studentSearchQuery"
          :student-email="appointment.studentEmail"
          :student-registration="appointment.studentRegistration"
          :student-full-name="appointment.studentFullName"
          :searching="searchingStudents"
          :search-results="studentSearchResults"
          :creating="creatingStudent"
          :linked-student="linkedStudent"
          @search="emit('searchStudents')"
          @create-student="emit('createStudent', $event)"
        />

        <div class="bg-white rounded-2xl p-4">
          <h3 class="font-bold text-primary-text flex items-center gap-2 mb-2">
            <FileText class="w-5 h-5 text-brand-orange" />
            {{ t("reason") }}
          </h3>
          <p class="text-sm text-primary-text/80">
            {{ appointment.reasonForVisit || t("common.not_specified") }}
          </p>
        </div>

        <ScheduleFormSection
          v-model:edit-form="editForm"
          :session-types="sessionTypes"
          @end-time-edited="emit('endTimeEdited')"
        />

        <AppointmentActionButtons
          :status="appointment.status"
          :saving="saving"
          :starting-session="startingSession"
          :has-linked-student="!!linkedStudent || !!selectedStudentId"
          @confirm="emit('confirm')"
          @cancel="emit('cancel')"
          @save="emit('save')"
          @start-session="emit('startSession')"
        />
      </div>
    </div>
  </div>
</template>
