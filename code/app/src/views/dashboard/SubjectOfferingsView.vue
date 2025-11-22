<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { Layers, AlertCircle, Search } from "lucide-vue-next";

import { usePagination, useFilters, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import {
  LoadingSpinner,
  Pagination,
  CardGrid,
  BaseModal,
  BaseCard,
  StatusToggle,
} from "../../components/common";
import { FormInput, FormButtons } from "../../components/form";

import { subjectOfferingsService } from "../../services/subjectOfferingsService";
import { subjectsService } from "../../services/subjectsService";
import { professorsService } from "../../services/professorsService";
import { semestersService } from "../../services/semestersService";

import type {
  SubjectOfferingDto,
  SubjectDto,
  ProfessorDto,
  SemesterDto,
  PaginationParams,
} from "@/types/api";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

const offerings = ref<SubjectOfferingDto[]>([]);
const loading = ref(false);

const pagination = usePagination({
  onPageChange: fetchOfferings,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchOfferings();
  },
});

const showModal = ref(false);
const creating = ref(false);
const isEditing = ref(false);
const selectedOffering = ref<SubjectOfferingDto | null>(null);
const offeringStatus = ref("Active");
const newOffering = ref({
  semesterId: "",
  subjectId: "",
  professorId: "",
});

const activeSubjects = ref<SubjectDto[]>([]);
const activeProfessors = ref<ProfessorDto[]>([]);
const currentSemester = ref<SemesterDto | null>(null);
const semesterError = ref("");

const subjectSearch = ref("");
const professorSearch = ref("");
const showSubjectDropdown = ref(false);
const showProfessorDropdown = ref(false);

let subjectSearchTimeout: ReturnType<typeof setTimeout>;
watch(subjectSearch, (newVal) => {
  if (!showSubjectDropdown.value) return;
  clearTimeout(subjectSearchTimeout);
  subjectSearchTimeout = setTimeout(() => {
    fetchActiveSubjects(newVal);
  }, 300);
});

let professorSearchTimeout: ReturnType<typeof setTimeout>;
watch(professorSearch, (newVal) => {
  if (!showProfessorDropdown.value) return;
  clearTimeout(professorSearchTimeout);
  professorSearchTimeout = setTimeout(() => {
    fetchActiveProfessors(newVal);
  }, 300);
});

async function fetchOfferings() {
  loading.value = true;
  try {
    const data = await subjectOfferingsService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    offerings.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch offerings:", error);
  } finally {
    loading.value = false;
  }
}

const fetchActiveSubjects = async (search = "") => {
  try {
    const params: PaginationParams = {};
    if (search) params.search = search;
    const data = await subjectsService.getActive(params);
    activeSubjects.value = data ?? [];
  } catch (error) {
    console.error("Error fetching subjects:", error);
    activeSubjects.value = [];
  }
};

const fetchActiveProfessors = async (search = "") => {
  try {
    const params: PaginationParams = {};
    if (search) params.search = search;
    const data = await professorsService.getAll(params);
    activeProfessors.value = data?.items ?? [];
  } catch (error) {
    console.error("Error fetching professors:", error);
    activeProfessors.value = [];
  }
};

const fetchCurrentSemester = async () => {
  try {
    currentSemester.value = await semestersService.getCurrent();
  } catch (error) {
    console.error("Error fetching current semester:", error);
    currentSemester.value = null;
  }
};

const selectSubject = (subject: SubjectDto) => {
  newOffering.value.subjectId = subject.publicId;
  subjectSearch.value = `${subject.name} (${subject.code})`;
  showSubjectDropdown.value = false;
};

const selectProfessor = (professor: ProfessorDto) => {
  newOffering.value.professorId = professor.publicId;
  professorSearch.value = professor.fullName;
  showProfessorDropdown.value = false;
};

const openModal = async () => {
  isEditing.value = false;
  selectedOffering.value = null;
  newOffering.value = { semesterId: "", subjectId: "", professorId: "" };
  subjectSearch.value = "";
  professorSearch.value = "";
  semesterError.value = "";
  showModal.value = true;

  await Promise.all([fetchActiveSubjects(), fetchActiveProfessors(), fetchCurrentSemester()]);

  if (currentSemester.value) {
    newOffering.value.semesterId = currentSemester.value.publicId;
  } else {
    semesterError.value = t("offerings.no_active_semester");
  }
};

const openEditModal = async (offering: SubjectOfferingDto) => {
  isEditing.value = true;
  selectedOffering.value = offering;
  newOffering.value = {
    semesterId: offering.semester.publicId,
    subjectId: offering.subject.publicId,
    professorId: offering.professor.publicId,
  };
  offeringStatus.value = offering.status;
  subjectSearch.value = `${offering.subject.name} (${offering.subject.code})`;
  professorSearch.value = offering.professor.fullName;
  semesterError.value = "";
  showModal.value = true;

  await Promise.all([fetchActiveSubjects(), fetchActiveProfessors()]);
  currentSemester.value = {
    publicId: offering.semester.publicId,
    name: offering.semester.name,
  } as SemesterDto;
};

const closeModal = () => {
  showModal.value = false;
};

const createOffering = async () => {
  if (!newOffering.value.semesterId) {
    semesterError.value = t("offerings.cannot_create_without_semester");
    return;
  }

  creating.value = true;
  try {
    if (isEditing.value && selectedOffering.value) {
      await subjectOfferingsService.update(selectedOffering.value.publicId, {
        ...newOffering.value,
        status: offeringStatus.value,
      });
      showSuccess("offerings.updated_success");
    } else {
      await subjectOfferingsService.create(newOffering.value);
      showSuccess("offerings.created_success");
    }
    closeModal();
    fetchOfferings();
  } catch (error) {
    console.error("Error saving offering:", error);
    showError(error, isEditing.value ? "offerings.error_updating" : "offerings.error_creating");
  } finally {
    creating.value = false;
  }
};

const toggleOfferingStatus = () => {
  offeringStatus.value = offeringStatus.value === "Active" ? "Inactive" : "Active";
};

onMounted(fetchOfferings);
</script>

<template>
  <div class="h-full flex flex-col relative gap-6 pb-6">
    <PageHeader
      :title="t('offerings.title')"
      :subtitle="t('offerings.subtitle')"
      :button-label="t('offerings.new_offering')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="openModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('offerings.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <div
      v-else-if="offerings.length === 0"
      class="flex-1 flex flex-col justify-center items-center text-primary-text/40"
    >
      <Layers class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("offerings.no_offerings") }}</p>
    </div>

    <CardGrid v-else>
      <BaseCard
        v-for="offering in offerings"
        :key="offering.publicId"
        :status="offering.status"
        :clickable="true"
        @click="openEditModal(offering)"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="flex-1 pr-2">
            <h3
              class="font-bold text-xl text-primary-text min-h-[3.5rem] line-clamp-2 overflow-hidden"
            >
              {{ offering.subject?.name || t("common.unknown") }}
            </h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ offering.semester?.name || t("common.unknown") }}
            </span>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
          >
            📚
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed line-clamp-2">
          {{ offering.professor?.fullName || t("common.unknown") }}
        </p>
      </BaseCard>
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

    <BaseModal
      :show="showModal"
      :title="isEditing ? t('offerings.edit_offering') : t('offerings.new_offering')"
      @close="closeModal"
    >
      <div
        v-if="semesterError"
        class="mb-6 p-4 bg-red-50 text-red-600 rounded-2xl flex items-start gap-3"
      >
        <AlertCircle class="w-5 h-5 shrink-0 mt-0.5" />
        <p class="text-sm font-medium">{{ semesterError }}</p>
      </div>

      <form @submit.prevent="createOffering" class="space-y-6">
        <FormInput
          :model-value="
            currentSemester ? currentSemester.name : t('offerings.no_active_semester_short')
          "
          :label="t('offerings.semester')"
          disabled
        />

        <div>
          <label class="block text-sm font-bold text-primary-text/60 mb-2">
            {{ t("offerings.subject") }}
          </label>
          <div class="relative">
            <div class="relative">
              <input
                type="text"
                v-model="subjectSearch"
                @focus="showSubjectDropdown = true"
                :placeholder="t('offerings.search_subject')"
                :disabled="!currentSemester"
                class="w-full px-4 py-3 pl-10 rounded-xl bg-gray-50 border-2 border-transparent focus:border-brand-teal focus:bg-white outline-none transition-all font-medium"
              />
              <Search
                class="w-5 h-5 text-primary-text/40 absolute left-3 top-1/2 -translate-y-1/2"
              />
            </div>

            <div
              v-if="showSubjectDropdown && activeSubjects && activeSubjects.length > 0"
              class="absolute z-10 w-full mt-2 bg-white rounded-xl shadow-xl border border-gray-100 max-h-60 overflow-y-auto"
            >
              <button
                v-for="subject in activeSubjects"
                :key="subject.publicId"
                type="button"
                @click="selectSubject(subject)"
                class="w-full text-left px-4 py-3 hover:bg-gray-50 transition-colors border-b border-gray-50 last:border-0"
              >
                <div class="font-bold text-primary-text">{{ subject.name }}</div>
                <div class="text-xs text-primary-text/60">{{ subject.code }}</div>
              </button>
            </div>
          </div>
        </div>

        <div>
          <label class="block text-sm font-bold text-primary-text/60 mb-2">
            {{ t("offerings.professor") }}
          </label>
          <div class="relative">
            <div class="relative">
              <input
                type="text"
                v-model="professorSearch"
                @focus="showProfessorDropdown = true"
                :placeholder="t('offerings.search_professor')"
                :disabled="!currentSemester"
                class="w-full px-4 py-3 pl-10 rounded-xl bg-gray-50 border-2 border-transparent focus:border-brand-teal focus:bg-white outline-none transition-all font-medium"
              />
              <Search
                class="w-5 h-5 text-primary-text/40 absolute left-3 top-1/2 -translate-y-1/2"
              />
            </div>

            <div
              v-if="showProfessorDropdown && activeProfessors && activeProfessors.length > 0"
              class="absolute z-10 w-full mt-2 bg-white rounded-xl shadow-xl border border-gray-100 max-h-60 overflow-y-auto"
            >
              <button
                v-for="prof in activeProfessors"
                :key="prof.publicId"
                type="button"
                @click="selectProfessor(prof)"
                class="w-full text-left px-4 py-3 hover:bg-gray-50 transition-colors border-b border-gray-50 last:border-0"
              >
                <div class="font-bold text-primary-text">{{ prof.fullName }}</div>
                <div class="text-xs text-primary-text/60">{{ prof.registration }}</div>
              </button>
            </div>
          </div>
        </div>

        <div v-if="isEditing">
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">
            {{ t("common.status") }}
          </label>
          <StatusToggle :status="offeringStatus" @toggle="toggleOfferingStatus" />
        </div>

        <FormButtons
          :loading="creating"
          :submit-label="
            isEditing ? t('offerings.update_offering') : t('offerings.create_offering')
          "
          :loading-label="isEditing ? t('common.updating') : t('common.creating')"
          :submit-disabled="!currentSemester"
          @cancel="closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
