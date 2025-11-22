<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { QuillEditor } from "@vueup/vue-quill";
import "@vueup/vue-quill/dist/vue-quill.snow.css";
import { ArrowLeft, Save, Loader2 } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import { patientRecordsService } from "../../services/patientRecordsService";
import { studentsService } from "../../services/studentsService";
import { patientObservationsService } from "../../services/patientObservationsService";
import { observationsService } from "../../services/observationsService";
import { subjectOfferingsService } from "../../services/subjectOfferingsService";
import { useApiToast } from "../../composables";
import PatientObservationsSection from "../../components/students/PatientObservationsSection.vue";
import SubjectEnrollmentsSection from "../../components/students/SubjectEnrollmentsSection.vue";
import ObservationModal from "../../components/students/ObservationModal.vue";
import type {
  PatientObservationDto,
  EnrollmentDto,
  ObservationDto,
  SubjectOfferingDto,
} from "@/types/api";

const { showSuccess, showError } = useApiToast();
const { t } = useI18n();

const route = useRoute();
const router = useRouter();

const recordId = ref(route.params.recordId as string);
const studentId = ref(route.query.studentId as string);
const studentName = ref("");
const content = ref("");
const loading = ref(true);
const saving = ref(false);

const patientObservations = ref<PatientObservationDto[]>([]);
const observationSearchQuery = ref("");
const availableObservations = ref<ObservationDto[]>([]);
const showObservationModal = ref(false);
const newObservationNote = ref("");
const selectedObservation = ref<ObservationDto | null>(null);
const isEditingPatientObservation = ref(false);
const editingPatientObservationId = ref<string | null>(null);

const enrollments = ref<EnrollmentDto[]>([]);
const enrollmentSearchQuery = ref("");
const availableOfferings = ref<SubjectOfferingDto[]>([]);

const editorOptions = {
  modules: {
    toolbar: [
      [{ header: [1, 2, 3, 4, 5, 6, false] }],
      ["bold", "italic", "underline", "strike"],
      [{ list: "ordered" }, { list: "bullet" }],
      [{ indent: "-1" }, { indent: "+1" }],
      [{ color: [] }, { background: [] }],
      [{ align: [] }],
      ["link"],
      ["clean"],
    ],
  },
  placeholder: "Start writing the patient record...",
  theme: "snow",
};

onMounted(async () => {
  try {
    const record = await patientRecordsService.getById(recordId.value);
    content.value = record.content || "";

    if (studentId.value) {
      const student = await studentsService.getById(studentId.value);
      studentName.value = student.fullName;

      patientObservations.value = await studentsService.getPatientObservations(studentId.value);
      enrollments.value = await studentsService.getEnrollments(studentId.value);
    }
  } catch (error) {
    console.error("Error loading record:", error);
    showError(error, "patient_record.load_error");
    router.back();
  } finally {
    loading.value = false;
  }
});

const saveRecord = async () => {
  saving.value = true;
  try {
    await patientRecordsService.update(recordId.value, {
      content: content.value,
    });
    showSuccess("patient_record.saved");
  } catch (error) {
    console.error("Error saving record:", error);
    showError(error, "patient_record.save_error");
  } finally {
    saving.value = false;
  }
};

const goBack = () => {
  if (content.value && !confirm(t("patient_record.unsaved_changes"))) {
    return;
  }
  router.back();
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
  if (!confirm(t("patient_observations.confirm_delete", { name: patientObs.observationName })))
    return;

  try {
    await patientObservationsService.delete(id);
    patientObservations.value = await studentsService.getPatientObservations(studentId.value);
    showSuccess("patient_observations.deleted");
  } catch (error) {
    console.error("Error deleting observation:", error);
    showError(error, "patient_observations.delete_error");
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
  if (!selectedObservation.value || !studentId.value) return;

  try {
    if (isEditingPatientObservation.value && editingPatientObservationId.value) {
      await patientObservationsService.update(editingPatientObservationId.value, {
        notes: newObservationNote.value,
      });
      showSuccess("patient_observations.updated");
    } else {
      const payload = {
        studentId: studentId.value,
        observationId: selectedObservation.value.id,
        notes: newObservationNote.value,
      };
      await patientObservationsService.create(payload);
      showSuccess("patient_observations.created");
    }

    patientObservations.value = await studentsService.getPatientObservations(studentId.value);
    closeObservationModal();
  } catch (error) {
    console.error("Error saving observation:", error);
    showError(error, "patient_observations.save_error");
  }
};

const searchOfferings = async () => {
  if (!enrollmentSearchQuery.value || enrollmentSearchQuery.value.length < 2) {
    availableOfferings.value = [];
    return;
  }

  try {
    const data = await subjectOfferingsService.getAll({
      search: enrollmentSearchQuery.value,
      pageSize: 5,
    });
    availableOfferings.value = data.items;
  } catch (error) {
    console.error("Error searching offerings:", error);
  }
};

const enrollStudent = async (offering: SubjectOfferingDto) => {
  if (!studentId.value) return;

  try {
    await studentsService.enroll(studentId.value, {
      subjectOfferingId: offering.publicId,
    });
    enrollments.value = await studentsService.getEnrollments(studentId.value);
    enrollmentSearchQuery.value = "";
    availableOfferings.value = [];
    showSuccess("enrollments.enrolled");
  } catch (error) {
    console.error("Error enrolling student:", error);
    showError(error, "enrollments.enroll_error");
  }
};

const removeEnrollment = async (enrollmentId: string) => {
  const enrollment = enrollments.value.find((e) => e.publicId === enrollmentId);
  if (!enrollment) return;
  if (!confirm(t("enrollments.confirm_remove", { name: enrollment.subjectName }))) return;
  if (!studentId.value) return;

  try {
    await studentsService.removeEnrollment(studentId.value, enrollmentId);
    enrollments.value = await studentsService.getEnrollments(studentId.value);
    showSuccess("enrollments.removed");
  } catch (error) {
    console.error("Error removing enrollment:", error);
    showError(error, "enrollments.remove_error");
  }
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br">
    <div
      class="w-full max-w-5xl h-[90vh] flex flex-col bg-[#FDFDF5] rounded-[40px] border-8 border-[#81F2DD] shadow-2xl overflow-hidden"
    >
      <header
        class="bg-[#FDFDF5] px-8 py-6 flex items-center justify-between border-b-2 border-[#E9E7D8]"
      >
        <div class="flex items-center gap-4">
          <button
            @click="goBack"
            class="p-2 rounded-full hover:bg-[#78D879]/20 transition-all hover:scale-110"
            :title="t('common.go_back')"
          >
            <ArrowLeft class="w-6 h-6 text-[#5E5340]" />
          </button>
          <div>
            <h1 class="text-3xl font-bold text-[#5E5340] flex items-center gap-3">
              <span class="text-4xl">📋</span>
              {{ t("patient_record.title") }}
            </h1>
            <p class="text-sm text-[#5E5340]/70 mt-1 font-medium" v-if="studentName">
              {{ studentName }}
            </p>
          </div>
        </div>
        <button
          @click="saveRecord"
          :disabled="saving || loading"
          class="px-8 py-3 rounded-full bg-[#78D879] text-white font-bold hover:scale-[1.05] hover:rotate-2 active:scale-95 transition-all shadow-lg border-4 border-white disabled:opacity-70 disabled:cursor-not-allowed flex items-center gap-3"
        >
          <Loader2 v-if="saving" class="w-5 h-5 animate-spin" />
          <Save v-else class="w-5 h-5" />
          {{ saving ? t("common.saving") : t("patient_record.save") }}
        </button>
      </header>

      <div v-if="loading" class="flex-1 flex items-center justify-center">
        <Loader2 class="w-12 h-12 animate-spin text-[#78D879]" />
      </div>
      <div v-else class="flex-1 overflow-y-auto">
        <div class="px-8 py-6 border-b-2 border-[#E9E7D8] bg-[#FDFDF5]/50">
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <PatientObservationsSection
              :observations="patientObservations"
              :loading="false"
              :search-query="observationSearchQuery"
              :available-observations="availableObservations"
              @update:search-query="observationSearchQuery = $event"
              @search="searchObservations"
              @add-observation="openObservationModal"
              @edit-observation="openEditObservationModal"
              @delete-observation="deletePatientObservation"
            />

            <SubjectEnrollmentsSection
              :enrollments="enrollments"
              :loading="false"
              :search-query="enrollmentSearchQuery"
              :available-offerings="availableOfferings"
              @update:search-query="enrollmentSearchQuery = $event"
              @search="searchOfferings"
              @enroll="enrollStudent"
              @remove="removeEnrollment"
            />
          </div>
        </div>

        <div class="h-[500px] relative">
          <QuillEditor
            v-model:content="content"
            :options="editorOptions"
            content-type="html"
            class="h-full notebook-editor"
          />
        </div>
      </div>
    </div>

    <ObservationModal
      :show="showObservationModal"
      :observation="selectedObservation"
      :note="newObservationNote"
      :is-editing="isEditingPatientObservation"
      :search-query="observationSearchQuery"
      :available-observations="availableObservations"
      @close="closeObservationModal"
      @update:note="newObservationNote = $event"
      @update:search-query="observationSearchQuery = $event"
      @search="searchObservations"
      @select-observation="openObservationModal"
      @save="savePatientObservation"
    />
  </div>
</template>

<style>
/* === The Notebook Editor === */
.notebook-editor {
  height: 100%;
  font-family:
    -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
}

/* === Quill Container === */
.notebook-editor .ql-container {
  font-size: 17px;
  height: calc(100% - 50px);
  border: none !important;
  background: #fdfdf5;
}

/* === The "Paper" (Editor Area) === */
.notebook-editor .ql-editor {
  padding: 50px 60px;
  line-height: 2;
  color: #5e5340;
  background: #fdfdf5;
  /* Notebook Lines Effect */
  background-image: repeating-linear-gradient(
    transparent,
    transparent 31px,
    #e3f5e1 31px,
    #e3f5e1 32px
  );
  background-attachment: local;
  min-height: 100%;
}

.notebook-editor .ql-editor.ql-blank::before {
  color: #5e5340;
  opacity: 0.4;
  font-style: italic;
  left: 60px;
}

/* === Custom Toolbar Styling === */
.notebook-editor .ql-toolbar {
  border: none !important;
  border-bottom: 3px solid #e9e7d8 !important;
  background: #f0faef;
  padding: 12px 20px;
  display: flex;
  gap: 8px;
  align-items: center;
}

/* Remove default separators */
.notebook-editor .ql-toolbar .ql-formats {
  margin-right: 0 !important;
}

/* Style all toolbar buttons */
.notebook-editor .ql-toolbar button,
.notebook-editor .ql-toolbar .ql-picker-label {
  color: #5e5340 !important;
  padding: 6px 8px;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.notebook-editor .ql-toolbar button:hover,
.notebook-editor .ql-toolbar .ql-picker-label:hover {
  background: rgba(120, 216, 121, 0.2) !important;
}

/* Active state for buttons */
.notebook-editor .ql-toolbar button.ql-active {
  background: #78d879 !important;
  color: white !important;
  border-radius: 50%;
  padding: 8px;
}

/* Icon colors */
.notebook-editor .ql-snow .ql-stroke {
  stroke: #5e5340 !important;
}

.notebook-editor .ql-snow .ql-fill {
  fill: #5e5340 !important;
}

.notebook-editor .ql-toolbar button.ql-active .ql-stroke {
  stroke: white !important;
}

.notebook-editor .ql-toolbar button.ql-active .ql-fill {
  fill: white !important;
}

/* Picker styling */
.notebook-editor .ql-snow .ql-picker-label {
  color: #5e5340 !important;
  border: 1px solid #e9e7d8 !important;
  border-radius: 8px;
}

.notebook-editor .ql-snow .ql-picker-label:hover {
  border-color: #78d879 !important;
}

.notebook-editor .ql-snow .ql-picker.ql-expanded .ql-picker-label {
  border-color: #78d879 !important;
  background: #f0faef;
}

/* Dropdown menus */
.notebook-editor .ql-snow .ql-picker-options {
  background: white;
  border: 2px solid #e9e7d8 !important;
  border-radius: 12px;
  padding: 8px;
  box-shadow: 0 10px 25px rgba(94, 83, 64, 0.15);
}

.notebook-editor .ql-snow .ql-picker-options .ql-picker-item:hover {
  background: #f0faef;
  color: #5e5340 !important;
}

/* Remove focus outlines */
.notebook-editor .ql-editor:focus,
.notebook-editor .ql-container:focus,
.notebook-editor .ql-toolbar button:focus {
  outline: none !important;
}

/* Custom scrollbar */
.notebook-editor .ql-editor::-webkit-scrollbar {
  width: 10px;
}

.notebook-editor .ql-editor::-webkit-scrollbar-track {
  background: #f0faef;
  border-radius: 10px;
}

.notebook-editor .ql-editor::-webkit-scrollbar-thumb {
  background: #81f2dd;
  border-radius: 10px;
}

.notebook-editor .ql-editor::-webkit-scrollbar-thumb:hover {
  background: #78d879;
}

/* Typography enhancements */
.notebook-editor .ql-editor h1,
.notebook-editor .ql-editor h2,
.notebook-editor .ql-editor h3 {
  color: #5e5340;
  font-weight: 700;
  margin-top: 1.5em;
  margin-bottom: 0.5em;
}

.notebook-editor .ql-editor h1 {
  font-size: 2em;
  border-bottom: 3px solid #78d879;
  padding-bottom: 0.3em;
}

.notebook-editor .ql-editor h2 {
  font-size: 1.6em;
}

.notebook-editor .ql-editor h3 {
  font-size: 1.3em;
}

.notebook-editor .ql-editor strong {
  color: #5e5340;
  font-weight: 700;
}

.notebook-editor .ql-editor em {
  color: #5e5340;
}

.notebook-editor .ql-editor ul,
.notebook-editor .ql-editor ol {
  padding-left: 2em;
}

.notebook-editor .ql-editor li {
  margin-bottom: 0.5em;
}

.notebook-editor .ql-editor a {
  color: #81f2dd;
  text-decoration: underline;
}

.notebook-editor .ql-editor a:hover {
  color: #78d879;
}
</style>
