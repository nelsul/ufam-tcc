<script setup lang="ts">
import { ref, onMounted, watch, computed } from "vue";
import { useI18n } from "vue-i18n";
import { Users, Search } from "lucide-vue-next";

import { usePagination, useFilters, useApiToast } from "../../composables";

import { PageHeader } from "../../components/layout";
import { LoadingSpinner, Pagination, CardGrid, BaseCard } from "../../components/common";
import ProfessorStudentDetailsModal from "../../components/students/ProfessorStudentDetailsModal.vue";

import { professorStudentsService } from "../../services/professorStudentsService";

import type {
  SubjectOfferingDto,
  StudentEnrollmentDto,
  StudentDto,
  PatientObservationDto,
} from "@/types/api";

const { t } = useI18n();
const { showError } = useApiToast();

const offerings = ref<SubjectOfferingDto[]>([]);
const students = ref<StudentEnrollmentDto[]>([]);
const selectedOfferingId = ref<string | null>(null);
const loadingOfferings = ref(false);
const loadingStudents = ref(false);

const showDetailsModal = ref(false);
const selectedStudent = ref<StudentDto | null>(null);
const loadingDetails = ref(false);
const studentObservations = ref<PatientObservationDto[]>([]);

const selectedOffering = computed(() =>
  offerings.value.find((o) => o.publicId === selectedOfferingId.value)
);

const pagination = usePagination({
  onPageChange: fetchStudents,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchStudents();
  },
});

async function fetchOfferings() {
  loadingOfferings.value = true;
  try {
    const data = await professorStudentsService.getMyActiveOfferings();
    offerings.value = data.items;
    const firstOffering = data.items[0];
    if (firstOffering && !selectedOfferingId.value) {
      selectedOfferingId.value = firstOffering.publicId;
    }
  } catch (error) {
    console.error("Failed to fetch offerings:", error);
    showError(error, "professor_dashboard.students.load_offerings_failed");
  } finally {
    loadingOfferings.value = false;
  }
}

async function fetchStudents() {
  if (!selectedOfferingId.value) {
    students.value = [];
    return;
  }

  loadingStudents.value = true;
  try {
    const data = await professorStudentsService.getStudentsByOffering(selectedOfferingId.value, {
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      search: filters.search.value || undefined,
    });
    students.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch students:", error);
    showError(error, "professor_dashboard.students.load_students_failed");
  } finally {
    loadingStudents.value = false;
  }
}

function selectOffering(offeringId: string) {
  selectedOfferingId.value = offeringId;
  filters.search.value = "";
  pagination.resetPage();
  fetchStudents();
}

async function openStudentDetails(enrollment: StudentEnrollmentDto) {
  showDetailsModal.value = true;
  loadingDetails.value = true;
  selectedStudent.value = null;
  studentObservations.value = [];

  try {
    const [studentData, observations] = await Promise.all([
      professorStudentsService.getStudentDetails(enrollment.studentId),
      professorStudentsService.getStudentObservations(enrollment.studentId),
    ]);
    selectedStudent.value = studentData;
    studentObservations.value = observations;
  } catch (error) {
    console.error("Failed to fetch student details:", error);
    showError(error, "professor_dashboard.students.load_details_failed");
    showDetailsModal.value = false;
  } finally {
    loadingDetails.value = false;
  }
}

function closeDetailsModal() {
  showDetailsModal.value = false;
  selectedStudent.value = null;
  studentObservations.value = [];
}

watch(
  () => filters.search.value,
  () => {
    pagination.resetPage();
    fetchStudents();
  }
);

onMounted(async () => {
  await fetchOfferings();
  if (selectedOfferingId.value) {
    fetchStudents();
  }
});
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <PageHeader
      :icon="Users"
      :title="t('professor_dashboard.students.title')"
      :subtitle="t('professor_dashboard.students.subtitle')"
    />

    <LoadingSpinner v-if="loadingOfferings" />

    <div
      v-else-if="offerings.length === 0"
      class="flex flex-col items-center justify-center py-20 text-primary-text/50"
    >
      <Users class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("professor_dashboard.students.no_offerings") }}</p>
    </div>

    <template v-else>
      <div class="space-y-2">
        <label class="block text-sm font-bold text-primary-text">
          {{ t("professor_dashboard.students.select_offering") }}
        </label>
        <div class="flex flex-wrap gap-2">
          <button
            v-for="offering in offerings"
            :key="offering.publicId"
            @click="selectOffering(offering.publicId)"
            class="px-4 py-2 rounded-xl text-sm font-medium transition-all border-2"
            :class="
              selectedOfferingId === offering.publicId
                ? 'bg-brand-teal text-white border-brand-teal'
                : 'bg-white text-primary-text/70 border-soft-gray hover:border-brand-teal'
            "
          >
            {{ offering.subject.code }} - {{ offering.subject.name }}
          </button>
        </div>
      </div>

      <div
        v-if="selectedOffering"
        class="bg-brand-teal/10 rounded-xl p-4 border-2 border-brand-teal/20"
      >
        <p class="text-sm text-primary-text">
          <span class="font-bold">{{ t("professor_dashboard.students.viewing_students") }}:</span>
          {{ selectedOffering.subject.code }} - {{ selectedOffering.subject.name }} ({{
            selectedOffering.semester.name
          }})
        </p>
      </div>

      <div class="relative">
        <Search class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-primary-text/40" />
        <input
          v-model="filters.search.value"
          type="text"
          :placeholder="t('professor_dashboard.students.search_placeholder')"
          class="w-full pl-12 pr-4 py-3 rounded-xl border-2 border-soft-gray focus:border-brand-teal focus:outline-none transition-colors bg-white"
        />
      </div>

      <LoadingSpinner v-if="loadingStudents" />

      <div
        v-else-if="students.length === 0 && selectedOfferingId"
        class="flex flex-col items-center justify-center py-20 text-primary-text/50"
      >
        <Users class="w-16 h-16 mb-4" />
        <p class="text-lg font-medium">{{ t("professor_dashboard.students.no_students") }}</p>
      </div>

      <CardGrid v-else-if="students.length > 0">
        <BaseCard
          v-for="enrollment in students"
          :key="enrollment.publicId"
          :clickable="true"
          :show-edit-indicator="false"
          @click="openStudentDetails(enrollment)"
        >
          <div class="flex justify-between items-start mb-4">
            <div class="flex-1 pr-2">
              <h3
                class="font-bold text-xl text-primary-text min-h-[2rem] line-clamp-2 overflow-hidden"
              >
                {{ enrollment.studentName }}
              </h3>
            </div>
            <div
              class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
            >
              👤
            </div>
          </div>
          <div class="space-y-1 text-sm text-primary-text/70">
            <p>
              <span class="font-medium">{{ t("common.registration") }}:</span>
              {{ enrollment.studentRegistration || "-" }}
            </p>
            <p>
              <span class="font-medium">{{ t("common.email") }}:</span>
              {{ enrollment.studentEmail || "-" }}
            </p>
          </div>
        </BaseCard>
      </CardGrid>

      <Pagination
        v-if="students.length > 0"
        :page-number="pagination.pageNumber.value"
        :total-pages="pagination.totalPages.value"
        :can-go-prev="pagination.canGoPrev.value"
        :can-go-next="pagination.canGoNext.value"
        @prev="pagination.prevPage"
        @next="pagination.nextPage"
      />
    </template>

    <ProfessorStudentDetailsModal
      :show="showDetailsModal"
      :student="selectedStudent"
      :loading="loadingDetails"
      :observations="studentObservations"
      @close="closeDetailsModal"
    />
  </div>
</template>
