<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { Calendar, Loader2 } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useApiToast } from "../../composables";

import { professionalsService, type AppointmentDto } from "../../services/professionalsService";
import {
  appointmentsService,
  type UpdateAppointmentDto,
  type AppointmentDto as FullAppointmentDto,
} from "../../services/appointmentsService";
import { sessionTypesService } from "../../services/sessionTypesService";
import { studentsService } from "../../services/studentsService";
import { sessionsService } from "../../services/sessionsService";
import type { SessionTypeDto, StudentDto, CreateStudentDto } from "@/types/api";

import {
  AppointmentCard,
  AppointmentFilters,
  AppointmentDetailsModal,
} from "../../components/appointments";

const { showSuccess, showError } = useApiToast();
const { t } = useI18n();
const router = useRouter();

interface FormattedAppointment {
  id: string;
  name: string;
  time: string;
  date: string;
  type: string | null;
  status: string;
}

const appointments = ref<FormattedAppointment[]>([]);
const loading = ref(false);
const sessionTypes = ref<SessionTypeDto[]>([]);

const filters = ref({
  status: "",
  sessionTypeId: "",
  search: "",
});

const showModal = ref(false);
const loadingDetails = ref(false);
const saving = ref(false);
const startingSession = ref(false);
const selectedAppointment = ref<FullAppointmentDto | null>(null);

const studentSearchQuery = ref("");
const searchingStudents = ref(false);
const studentSearchResults = ref<StudentDto[]>([]);
const selectedStudentId = ref<string | null>(null);
const creatingStudent = ref(false);
const linkedStudent = ref<StudentDto | null>(null);

watch(selectedStudentId, (newVal) => {
  if (newVal === null) {
    linkedStudent.value = null;
  }
});

const editForm = ref({
  startDate: "",
  startTime: "",
  endDate: "",
  endTime: "",
  sessionTypeId: "",
});

const endTimeManuallyEdited = ref(false);

const fetchAppointments = async () => {
  loading.value = true;
  try {
    const result = await professionalsService.getMyAppointments({
      status: filters.value.status || undefined,
      sessionTypeId: filters.value.sessionTypeId || undefined,
      search: filters.value.search || undefined,
    });
    if (result && result.items) {
      appointments.value = result.items.map((apt: AppointmentDto) => {
        const date = new Date(apt.startTime);
        return {
          id: apt.id,
          name: apt.studentFullName,
          time: date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
          date: date.toLocaleDateString([], { weekday: "short", month: "short", day: "numeric" }),
          type: apt.sessionTypeName,
          status: apt.status,
        };
      });
    }
  } catch (error) {
    console.error("Failed to fetch appointments:", error);
  } finally {
    loading.value = false;
  }
};

const fetchSessionTypes = async () => {
  try {
    const data = await sessionTypesService.getAll({ pageSize: 100 });
    sessionTypes.value = data.items;
  } catch (error) {
    console.error("Failed to fetch session types:", error);
  }
};

const searchStudents = async () => {
  if (!selectedAppointment.value) return;

  searchingStudents.value = true;
  try {
    const searchTerm =
      studentSearchQuery.value ||
      selectedAppointment.value.studentEmail ||
      selectedAppointment.value.studentRegistration;

    if (!searchTerm) {
      studentSearchResults.value = [];
      return;
    }

    const result = await studentsService.getAll({ search: searchTerm, pageSize: 10 });
    studentSearchResults.value = result.items;
  } catch (error) {
    console.error("Failed to search students:", error);
  } finally {
    searchingStudents.value = false;
  }
};

const clearFilters = () => {
  filters.value = { status: "", sessionTypeId: "", search: "" };
  fetchAppointments();
};

const openAppointmentModal = async (apt: FormattedAppointment) => {
  showModal.value = true;
  loadingDetails.value = true;
  endTimeManuallyEdited.value = false;

  studentSearchQuery.value = "";
  studentSearchResults.value = [];
  selectedStudentId.value = null;
  linkedStudent.value = null;
  selectedStudentId.value = null;

  try {
    selectedAppointment.value = await appointmentsService.getById(apt.id);

    if (selectedAppointment.value.studentId) {
      selectedStudentId.value = selectedAppointment.value.studentId;
      try {
        linkedStudent.value = await studentsService.getById(selectedAppointment.value.studentId);
      } catch (error) {
        console.error("Failed to fetch linked student:", error);
        linkedStudent.value = null;
      }
    }

    const startDate = new Date(selectedAppointment.value.startTime);
    const localStartDate = `${startDate.getFullYear()}-${String(startDate.getMonth() + 1).padStart(2, "0")}-${String(startDate.getDate()).padStart(2, "0")}`;
    const localStartTime = `${String(startDate.getHours()).padStart(2, "0")}:${String(startDate.getMinutes()).padStart(2, "0")}`;

    editForm.value = {
      startDate: localStartDate,
      startTime: localStartTime,
      endDate: "",
      endTime: "",
      sessionTypeId: selectedAppointment.value.sessionTypeId || "",
    };

    if (selectedAppointment.value.endTime) {
      const endDate = new Date(selectedAppointment.value.endTime);
      editForm.value.endDate = `${endDate.getFullYear()}-${String(endDate.getMonth() + 1).padStart(2, "0")}-${String(endDate.getDate()).padStart(2, "0")}`;
      editForm.value.endTime = `${String(endDate.getHours()).padStart(2, "0")}:${String(endDate.getMinutes()).padStart(2, "0")}`;
    }

    if (sessionTypes.value.length === 0) {
      await fetchSessionTypes();
    }

    await searchStudents();
  } catch (error) {
    console.error("Failed to fetch appointment details:", error);
    showError(error, "appointments.load_failed");
    closeModal();
  } finally {
    loadingDetails.value = false;
  }
};

const closeModal = () => {
  showModal.value = false;
  selectedAppointment.value = null;
};

const saveAppointment = async () => {
  if (!selectedAppointment.value) return;

  saving.value = true;
  try {
    const startDateTime = new Date(`${editForm.value.startDate}T${editForm.value.startTime}:00`);

    const updateData: UpdateAppointmentDto = {
      startTime: startDateTime.toISOString(),
      sessionTypeId: editForm.value.sessionTypeId || null,
      studentId: selectedStudentId.value,
    };

    if (endTimeManuallyEdited.value && editForm.value.endDate && editForm.value.endTime) {
      const endDateTime = new Date(`${editForm.value.endDate}T${editForm.value.endTime}:00`);
      updateData.endTime = endDateTime.toISOString();
    }

    await appointmentsService.update(selectedAppointment.value.id, updateData);
    showSuccess("appointments.updated");
    closeModal();
    fetchAppointments();
  } catch (error) {
    console.error("Failed to update appointment:", error);
    showError(error, "appointments.update_failed");
  } finally {
    saving.value = false;
  }
};

const confirmAppointment = async () => {
  if (!selectedAppointment.value) return;

  saving.value = true;
  try {
    await appointmentsService.update(selectedAppointment.value.id, { status: "Confirmed" });
    showSuccess("appointments.confirmed_success");
    closeModal();
    fetchAppointments();
  } catch (error) {
    console.error("Failed to confirm appointment:", error);
    showError(error, "appointments.confirm_failed");
  } finally {
    saving.value = false;
  }
};

const cancelAppointment = async () => {
  if (!selectedAppointment.value) return;

  saving.value = true;
  try {
    await appointmentsService.update(selectedAppointment.value.id, { status: "Cancelled" });
    showSuccess("appointments.cancelled_success");
    closeModal();
    fetchAppointments();
  } catch (error) {
    console.error("Failed to cancel appointment:", error);
    showError(error, "appointments.cancel_failed");
  } finally {
    saving.value = false;
  }
};

const startSession = async () => {
  if (!selectedAppointment.value) return;

  if (selectedStudentId.value && selectedStudentId.value !== selectedAppointment.value.studentId) {
    await saveAppointment();
  }

  startingSession.value = true;
  try {
    await sessionsService.startSession(selectedAppointment.value.id);
    showSuccess("sessions.started_success");
    closeModal();
    router.push("/dashboard/sessions");
  } catch (error) {
    console.error("Failed to start session:", error);
    showError(error, "sessions.start_failed");
  } finally {
    startingSession.value = false;
  }
};

const createStudent = async (data: {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
}) => {
  creatingStudent.value = true;
  try {
    const createDto: CreateStudentDto = {
      name: data.name,
      fullName: data.fullName,
      institutionalEmail: data.institutionalEmail,
      registration: data.registration,
    };

    const created = await studentsService.create(createDto);
    showSuccess("appointments.student_created");
    selectedStudentId.value = created.publicId;
    linkedStudent.value = created;
    await searchStudents();
  } catch (error) {
    console.error("Failed to create student:", error);
    showError(error, "appointments.student_create_failed");
  } finally {
    creatingStudent.value = false;
  }
};

onMounted(() => {
  fetchAppointments();
  fetchSessionTypes();
});
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <header class="mb-4 md:mb-8">
      <h1 class="text-2xl md:text-4xl font-bold text-primary-text mb-2">
        {{ t("appointments.title") }}
      </h1>
      <p class="text-sm md:text-base text-primary-text/60">{{ t("appointments.subtitle") }}</p>
    </header>

    <AppointmentFilters
      v-model:filters="filters"
      :session-types="sessionTypes"
      @search="fetchAppointments"
      @filter-change="fetchAppointments"
      @clear="clearFilters"
    />

    <div v-if="loading" class="flex items-center justify-center py-20">
      <Loader2 class="w-10 h-10 animate-spin text-brand-teal" />
    </div>

    <div
      v-else-if="appointments.length === 0"
      class="flex flex-col items-center justify-center py-20 text-primary-text/50"
    >
      <Calendar class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("appointments.no_appointments") }}</p>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <AppointmentCard
        v-for="apt in appointments"
        :key="apt.id"
        :appointment="apt"
        @click="openAppointmentModal(apt)"
      />
    </div>

    <AppointmentDetailsModal
      :show="showModal"
      :loading="loadingDetails"
      :saving="saving"
      :starting-session="startingSession"
      :appointment="selectedAppointment"
      :session-types="sessionTypes"
      :searching-students="searchingStudents"
      :student-search-results="studentSearchResults"
      :creating-student="creatingStudent"
      :linked-student="linkedStudent"
      v-model:selected-student-id="selectedStudentId"
      v-model:student-search-query="studentSearchQuery"
      v-model:edit-form="editForm"
      @close="closeModal"
      @save="saveAppointment"
      @confirm="confirmAppointment"
      @cancel="cancelAppointment"
      @start-session="startSession"
      @search-students="searchStudents"
      @create-student="createStudent"
      @end-time-edited="endTimeManuallyEdited = true"
    />
  </div>
</template>
