<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { store } from "../../store";

import { usePagination, useFilters, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import { LoadingSpinner, Pagination, CardGrid, BaseModal } from "../../components/common";
import { FormInput, FormButtons } from "../../components/form";
import StudentCard from "../../components/students/StudentCard.vue";
import StudentFormModal from "../../components/students/StudentFormModal.vue";
import StudentDetailsModal from "../../components/students/StudentDetailsModal.vue";
import RecordModal from "../../components/students/RecordModal.vue";
import ObservationModal from "../../components/students/ObservationModal.vue";

import { studentsService } from "../../services/studentsService";
import { patientRecordsService } from "../../services/patientRecordsService";
import { patientObservationsService } from "../../services/patientObservationsService";
import { observationsService } from "../../services/observationsService";
import { subjectOfferingsService } from "../../services/subjectOfferingsService";

import type {
  StudentDto,
  PatientRecordDto,
  PatientObservationDto,
  EnrollmentDto,
  SubjectOfferingDto,
  ObservationDto,
  StudentFormData,
} from "@/types/api";

interface HttpError {
  status?: number;
  message?: string;
}

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();
const router = useRouter();

const students = ref<StudentDto[]>([]);
const loading = ref(false);

const pagination = usePagination({
  onPageChange: fetchStudents,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchStudents();
  },
});

const showModal = ref(false);
const creating = ref(false);
const isEditing = ref(false);
const newStudent = ref({
  name: "",
  fullName: "",
  institutionalEmail: "",
  registration: "",
  password: "",
});

const showPasswordModal = ref(false);
const resettingPassword = ref(false);
const newPassword = ref("");

const showDetailsModal = ref(false);
const selectedStudent = ref<StudentDto | null>(null);
const patientRecord = ref<PatientRecordDto | null>(null);
const patientObservations = ref<PatientObservationDto[]>([]);
const enrollments = ref<EnrollmentDto[]>([]);
const loadingDetails = ref(false);

const searchQuery = ref("");
const availableOfferings = ref<SubjectOfferingDto[]>([]);

const observationSearchQuery = ref("");
const availableObservations = ref<ObservationDto[]>([]);
const showObservationModal = ref(false);
const newObservationNote = ref("");
const selectedObservation = ref<ObservationDto | null>(null);
const isEditingPatientObservation = ref(false);
const editingPatientObservationId = ref<string | null>(null);

const showRecordModal = ref(false);
const recordContent = ref("");
const savingRecord = ref(false);

async function fetchStudents() {
  loading.value = true;
  try {
    const data = await studentsService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    students.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch students:", error);
  } finally {
    loading.value = false;
  }
}

const openModal = () => {
  isEditing.value = false;
  newStudent.value = {
    name: "",
    fullName: "",
    institutionalEmail: "",
    registration: "",
    password: "",
  };
  showModal.value = true;
};

const openEditStudent = () => {
  if (!selectedStudent.value) return;
  isEditing.value = true;
  newStudent.value = {
    ...selectedStudent.value,
    password: "",
  };
  showDetailsModal.value = false;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
};

const closeDetailsModal = () => {
  showDetailsModal.value = false;
  selectedStudent.value = null;
};

const closeRecordModal = () => {
  showRecordModal.value = false;
  recordContent.value = "";
};

const saveStudent = async (studentData: StudentFormData) => {
  creating.value = true;
  try {
    if (isEditing.value) {
      await studentsService.update(selectedStudent.value!.publicId, {
        name: studentData.name,
        fullName: studentData.fullName,
        institutionalEmail: studentData.institutionalEmail,
        registration: studentData.registration,
        status: selectedStudent.value!.status,
      });
    } else {
      await studentsService.create(studentData);
    }

    closeModal();
    fetchStudents();
  } catch (error) {
    console.error(`Error ${isEditing.value ? "updating" : "creating"} student:`, error);
    showError(error, isEditing.value ? "students.error_updating" : "students.error_creating");
  } finally {
    creating.value = false;
  }
};

const fetchStudentDetails = async (studentId: string) => {
  loadingDetails.value = true;
  patientRecord.value = null;
  patientObservations.value = [];
  enrollments.value = [];
  searchQuery.value = "";
  availableOfferings.value = [];

  try {
    try {
      patientRecord.value = await studentsService.getPatientRecord(studentId);
    } catch (error) {
      const httpError = error as HttpError;
      if (httpError.status !== 404) {
        console.error("Failed to fetch patient record", error);
      }
    }

    try {
      patientObservations.value = await studentsService.getPatientObservations(studentId);
    } catch (error) {
      console.error("Failed to fetch patient observations", error);
    }

    await fetchEnrollments(studentId);
  } catch (error) {
    console.error("Error fetching details:", error);
  } finally {
    loadingDetails.value = false;
  }
};

const fetchEnrollments = async (studentId: string) => {
  try {
    enrollments.value = await studentsService.getEnrollments(studentId);
  } catch (error) {
    console.error("Error fetching enrollments:", error);
  }
};

const searchOfferings = async () => {
  if (!searchQuery.value || searchQuery.value.length < 2) {
    availableOfferings.value = [];
    return;
  }

  try {
    const data = await subjectOfferingsService.getAll({
      search: searchQuery.value,
      pageSize: 5,
    });
    const enrolledIds = new Set(enrollments.value.map((e) => e.subjectOfferingId));
    availableOfferings.value = data.items.filter((o) => !enrolledIds.has(o.publicId));
  } catch (error) {
    console.error("Error searching offerings:", error);
  }
};

const enrollStudent = async (offering: SubjectOfferingDto) => {
  if (!selectedStudent.value) return;
  try {
    await studentsService.enroll(selectedStudent.value.publicId, {
      subjectOfferingId: offering.publicId,
    });

    await fetchEnrollments(selectedStudent.value.publicId);
    searchQuery.value = "";
    availableOfferings.value = [];
  } catch (error) {
    console.error("Error enrolling student:", error);
    showError(error, "students.error_enrolling");
  }
};

const removeEnrollment = async (id: string) => {
  const enrollment = enrollments.value.find((e) => e.publicId === id);
  if (!enrollment) return;
  if (!confirm(`Are you sure you want to remove enrollment for ${enrollment.subjectName}?`)) return;
  if (!selectedStudent.value) return;

  try {
    await studentsService.removeEnrollment(selectedStudent.value.publicId, id);
    await fetchEnrollments(selectedStudent.value.publicId);
  } catch (error) {
    console.error("Error removing enrollment:", error);
  }
};

const searchObservations = async () => {
  if (!observationSearchQuery.value || observationSearchQuery.value.length < 2) {
    availableObservations.value = [];
    return;
  }

  try {
    const data = await observationsService.getAll({
      search: observationSearchQuery.value,
      pageSize: 5,
    });
    availableObservations.value = data.items;
  } catch (error) {
    console.error("Error searching observations:", error);
  }
};

const openObservationModal = (observation: ObservationDto) => {
  isEditingPatientObservation.value = false;
  selectedObservation.value = observation;
  newObservationNote.value = "";
  showObservationModal.value = true;
  availableObservations.value = [];
  observationSearchQuery.value = "";
};

const openEditObservationModal = (patientObs: PatientObservationDto) => {
  isEditingPatientObservation.value = true;
  editingPatientObservationId.value = patientObs.id;
  selectedObservation.value = {
    id: patientObs.observationId,
    name: patientObs.observationName,
    description: "Edit note for this observation",
    status: "Active",
    createdAt: patientObs.createdAt,
    updatedAt: patientObs.updatedAt,
  };
  newObservationNote.value = patientObs.notes;
  showObservationModal.value = true;
};

const deletePatientObservation = async (id: string) => {
  const patientObs = patientObservations.value.find((p) => p.id === id);
  if (!patientObs) return;
  if (!confirm(`Are you sure you want to delete this observation: ${patientObs.observationName}?`))
    return;
  if (!selectedStudent.value) return;

  try {
    await patientObservationsService.delete(id);
    patientObservations.value = await studentsService.getPatientObservations(
      selectedStudent.value.publicId
    );
  } catch (error) {
    console.error("Error deleting observation:", error);
  }
};

const closeObservationModal = () => {
  showObservationModal.value = false;
  selectedObservation.value = null;
  newObservationNote.value = "";
  isEditingPatientObservation.value = false;
  editingPatientObservationId.value = null;
};

const savePatientObservation = async () => {
  if (!selectedObservation.value || !selectedStudent.value || !store.user) return;

  try {
    if (isEditingPatientObservation.value && editingPatientObservationId.value) {
      await patientObservationsService.update(editingPatientObservationId.value, {
        notes: newObservationNote.value,
      });
    } else {
      const payload = {
        studentId: selectedStudent.value.publicId,
        observationId: selectedObservation.value.id,
        notes: newObservationNote.value,
      };
      await patientObservationsService.create(payload);
    }

    patientObservations.value = await studentsService.getPatientObservations(
      selectedStudent.value.publicId
    );
    closeObservationModal();
  } catch (error) {
    console.error("Error saving observation:", error);
  }
};

const openDetails = async (student: StudentDto) => {
  selectedStudent.value = student;
  showDetailsModal.value = true;
  await fetchStudentDetails(student.publicId);
};

const handleOpenRecord = async () => {
  if (!selectedStudent.value) return;
  savingRecord.value = true;
  try {
    if (!patientRecord.value) {
      patientRecord.value = await patientRecordsService.create({
        studentId: selectedStudent.value.publicId,
        content: "",
      });
    }

    router.push({
      name: "dashboard-patient-record",
      params: { recordId: patientRecord.value.id },
      query: { studentId: selectedStudent.value.publicId },
    });
  } catch (error) {
    console.error("Error opening record:", error);
    showError(error, "students.error_opening_record");
  } finally {
    savingRecord.value = false;
  }
};

const saveRecord = async () => {
  if (!patientRecord.value) return;

  savingRecord.value = true;
  try {
    await patientRecordsService.update(patientRecord.value.id, {
      content: recordContent.value,
    });

    patientRecord.value.content = recordContent.value;
    closeRecordModal();
  } catch (error) {
    console.error("Error saving record:", error);
    showError(error, "students.error_saving_record");
  } finally {
    savingRecord.value = false;
  }
};

const openPasswordReset = () => {
  if (!selectedStudent.value) return;
  newPassword.value = "";
  showPasswordModal.value = true;
};

const closePasswordModal = () => {
  showPasswordModal.value = false;
  newPassword.value = "";
};

const handleResetPassword = async () => {
  if (!selectedStudent.value || !newPassword.value) return;
  resettingPassword.value = true;
  try {
    await studentsService.resetPassword(selectedStudent.value.publicId, {
      newPassword: newPassword.value,
    });
    showSuccess("students.password_reset_success");
    closePasswordModal();
  } catch (error) {
    console.error("Error resetting password:", error);
    showError(error, "students.error_resetting_password");
  } finally {
    resettingPassword.value = false;
  }
};

const toggleStudentStatus = async () => {
  if (!selectedStudent.value) return;
  const newStatus = selectedStudent.value.status === "Active" ? "Inactive" : "Active";
  try {
    await studentsService.update(selectedStudent.value.publicId, {
      name: selectedStudent.value.name,
      fullName: selectedStudent.value.fullName,
      institutionalEmail: selectedStudent.value.institutionalEmail,
      registration: selectedStudent.value.registration,
      status: newStatus,
    });
    selectedStudent.value.status = newStatus;
    showSuccess(newStatus === "Active" ? "students.activated" : "students.deactivated");
    fetchStudents();
  } catch (error) {
    console.error("Error toggling status:", error);
    showError(error, "students.error_changing_status");
  }
};

onMounted(fetchStudents);
</script>

<template>
  <div class="h-full flex flex-col relative gap-6 pb-6">
    <PageHeader
      :title="t('students.title')"
      :subtitle="t('students.subtitle')"
      :button-label="t('students.new_student')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="openModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('students.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <CardGrid v-else>
      <StudentCard
        v-for="student in students"
        :key="student.publicId"
        :student="student"
        @click="openDetails(student)"
      />
    </CardGrid>

    <Pagination
      v-if="!loading"
      :page-number="pagination.pageNumber.value"
      :total-pages="pagination.totalPages.value"
      :can-go-prev="pagination.canGoPrev.value"
      :can-go-next="pagination.canGoNext.value"
      @prev="pagination.prevPage"
      @next="pagination.nextPage"
    />

    <StudentFormModal
      :show="showModal"
      :is-editing="isEditing"
      :loading="creating"
      :initial-data="newStudent"
      @close="closeModal"
      @save="saveStudent"
    />

    <StudentDetailsModal
      :show="showDetailsModal"
      :student="selectedStudent"
      :loading-details="loadingDetails"
      :patient-record="patientRecord"
      :patient-observations="patientObservations"
      :enrollments="enrollments"
      :observation-search-query="observationSearchQuery"
      :available-observations="availableObservations"
      :enrollment-search-query="searchQuery"
      :available-offerings="availableOfferings"
      @close="closeDetailsModal"
      @edit-student="openEditStudent"
      @reset-password="openPasswordReset"
      @toggle-status="toggleStudentStatus"
      @open-record="handleOpenRecord"
      @update:observation-search-query="observationSearchQuery = $event"
      @search-observations="searchObservations"
      @add-observation="openObservationModal"
      @edit-observation="openEditObservationModal"
      @delete-observation="deletePatientObservation"
      @update:enrollment-search-query="searchQuery = $event"
      @search-offerings="searchOfferings"
      @enroll="enrollStudent"
      @remove-enrollment="removeEnrollment"
    />

    <RecordModal
      :show="showRecordModal"
      :content="recordContent"
      :saving="savingRecord"
      @close="closeRecordModal"
      @save="saveRecord"
      @update:content="recordContent = $event"
    />

    <ObservationModal
      :show="showObservationModal"
      :observation="selectedObservation"
      :note="newObservationNote"
      :is-editing="isEditingPatientObservation"
      :search-query="observationSearchQuery"
      :available-observations="availableObservations"
      @close="closeObservationModal"
      @save="savePatientObservation"
      @update:note="newObservationNote = $event"
      @update:search-query="observationSearchQuery = $event"
      @search="searchObservations"
      @select-observation="openObservationModal"
    />

    <BaseModal
      :show="showPasswordModal"
      :title="t('students.reset_password')"
      max-width="max-w-md"
      @close="closePasswordModal"
    >
      <p class="text-primary-text/60 mb-4">
        {{ t("students.enter_new_password") }} <strong>{{ selectedStudent?.name }}</strong>
      </p>
      <FormInput v-model="newPassword" type="password" :placeholder="t('students.new_password')" />
      <FormButtons
        class="mt-6"
        :loading="resettingPassword"
        :submit-label="t('students.reset_password')"
        :loading-label="t('students.resetting')"
        :submit-disabled="!newPassword"
        @cancel="closePasswordModal"
        @submit="handleResetPassword"
      />
    </BaseModal>
  </div>
</template>
