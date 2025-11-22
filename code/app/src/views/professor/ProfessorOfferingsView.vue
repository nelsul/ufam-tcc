<script setup lang="ts">
import { ref, onMounted, watch, computed } from "vue";
import { useI18n } from "vue-i18n";
import { Layers, Plus, Search, AlertCircle } from "lucide-vue-next";

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
import { FormButtons } from "../../components/form";

import { professorOfferingsService } from "../../services/professorOfferingsService";
import { subjectsService } from "../../services/subjectsService";
import { semestersService } from "../../services/semestersService";

import { store } from "../../store";

import type { SubjectOfferingDto, SubjectDto, SemesterDto } from "@/types/api";

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
  subjectId: "",
});

const activeSubjects = ref<SubjectDto[]>([]);
const currentSemester = ref<SemesterDto | null>(null);
const semesterError = ref("");

const subjectSearch = ref("");
const showSubjectDropdown = ref(false);

let subjectSearchTimeout: ReturnType<typeof setTimeout>;
watch(subjectSearch, (newVal) => {
  if (!showSubjectDropdown.value) return;
  clearTimeout(subjectSearchTimeout);
  subjectSearchTimeout = setTimeout(() => {
    fetchActiveSubjects(newVal);
  }, 300);
});

async function fetchOfferings() {
  loading.value = true;
  try {
    const data = await professorOfferingsService.getMyOfferings({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    offerings.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch offerings:", error);
    showError(error, "professor_dashboard.offerings.load_failed");
  } finally {
    loading.value = false;
  }
}

async function fetchActiveSubjects(search?: string) {
  try {
    const data = await subjectsService.getActive({ search });
    activeSubjects.value = data;
  } catch (error) {
    console.error("Failed to fetch subjects:", error);
  }
}

async function fetchCurrentSemester() {
  try {
    const data = await semestersService.getCurrent();
    currentSemester.value = data;
    semesterError.value = "";
  } catch (error) {
    console.error("Failed to fetch current semester:", error);
    semesterError.value = t("professor_dashboard.offerings.no_current_semester");
    currentSemester.value = null;
  }
}

function openCreateModal() {
  if (!currentSemester.value) {
    showError(null, "professor_dashboard.offerings.no_current_semester");
    return;
  }
  isEditing.value = false;
  selectedOffering.value = null;
  newOffering.value = { subjectId: "" };
  subjectSearch.value = "";
  showModal.value = true;
  fetchActiveSubjects();
}

function openEditModal(offering: SubjectOfferingDto) {
  isEditing.value = true;
  selectedOffering.value = offering;
  offeringStatus.value = offering.status;
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
  selectedOffering.value = null;
  subjectSearch.value = "";
  showSubjectDropdown.value = false;
}

function selectSubject(subject: SubjectDto) {
  newOffering.value.subjectId = subject.publicId;
  subjectSearch.value = `${subject.code} - ${subject.name}`;
  showSubjectDropdown.value = false;
}

async function handleSubmit() {
  if (!isEditing.value) {
    await createOffering();
  } else {
    await updateOffering();
  }
}

async function createOffering() {
  if (!currentSemester.value || !newOffering.value.subjectId || !store.user) {
    return;
  }

  creating.value = true;
  try {
    await professorOfferingsService.create({
      semesterId: currentSemester.value.publicId,
      subjectId: newOffering.value.subjectId,
      professorId: store.user.publicId,
    });
    showSuccess("professor_dashboard.offerings.created_success");
    closeModal();
    fetchOfferings();
  } catch (error) {
    console.error("Error creating offering:", error);
    showError(error, "professor_dashboard.offerings.error_creating");
  } finally {
    creating.value = false;
  }
}

async function updateOffering() {
  if (!selectedOffering.value || !store.user) return;

  creating.value = true;
  try {
    await professorOfferingsService.update(selectedOffering.value.publicId, {
      semesterId: selectedOffering.value.semester.publicId,
      subjectId: selectedOffering.value.subject.publicId,
      professorId: store.user.publicId,
      status: offeringStatus.value === "Active" ? 0 : 1,
    });
    showSuccess("professor_dashboard.offerings.updated_success");
    closeModal();
    fetchOfferings();
  } catch (error) {
    console.error("Error updating offering:", error);
    showError(error, "professor_dashboard.offerings.error_updating");
  } finally {
    creating.value = false;
  }
}

function toggleStatus() {
  offeringStatus.value = offeringStatus.value === "Active" ? "Inactive" : "Active";
}

const selectedSubjectDisplay = computed(() => {
  if (!newOffering.value.subjectId) return "";
  const subject = activeSubjects.value.find((s) => s.publicId === newOffering.value.subjectId);
  return subject ? `${subject.code} - ${subject.name}` : "";
});

onMounted(() => {
  fetchCurrentSemester();
  fetchOfferings();
});
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <PageHeader
      :icon="Layers"
      :title="t('professor_dashboard.offerings.title')"
      :subtitle="t('professor_dashboard.offerings.subtitle')"
    >
      <template #actions>
        <button
          @click="openCreateModal"
          :disabled="!currentSemester"
          class="px-4 py-2 bg-brand-teal text-white rounded-xl font-bold text-sm flex items-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Plus class="w-4 h-4" />
          {{ t("professor_dashboard.offerings.new") }}
        </button>
      </template>
    </PageHeader>

    <div
      v-if="semesterError"
      class="bg-red-100 text-red-800 rounded-xl p-3 flex items-center gap-3 text-sm font-bold border-2 border-red-200"
    >
      <AlertCircle class="w-5 h-5 shrink-0" />
      <span>{{ semesterError }}</span>
    </div>

    <div
      v-else-if="currentSemester"
      class="bg-brand-teal/10 rounded-xl p-3 flex items-center gap-3 text-sm font-medium border-2 border-brand-teal/20"
    >
      <span class="text-primary-text/60"
        >{{ t("professor_dashboard.offerings.current_semester") }}:</span
      >
      <span class="font-bold text-primary-text">{{ currentSemester.name }}</span>
    </div>

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('professor_dashboard.offerings.search_placeholder')"
      :show-inactive-filter="true"
    />

    <LoadingSpinner v-if="loading" />

    <div
      v-else-if="offerings.length === 0"
      class="flex flex-col items-center justify-center py-20 text-primary-text/50"
    >
      <Layers class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("professor_dashboard.offerings.no_offerings") }}</p>
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
              {{ offering.subject.name }}
            </h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ offering.subject.code }}
            </span>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
          >
            📖
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed">
          {{ t("professor_dashboard.offerings.semester") }}: {{ offering.semester.name }}
        </p>
      </BaseCard>
    </CardGrid>

    <Pagination
      :page-number="pagination.pageNumber.value"
      :total-pages="pagination.totalPages.value"
      :can-go-prev="pagination.canGoPrev.value"
      :can-go-next="pagination.canGoNext.value"
      @prev="pagination.prevPage"
      @next="pagination.nextPage"
    />

    <BaseModal
      :show="showModal"
      :title="
        isEditing
          ? t('professor_dashboard.offerings.edit_title')
          : t('professor_dashboard.offerings.create_title')
      "
      @close="closeModal"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <template v-if="!isEditing">
          <div class="space-y-2">
            <label class="block text-sm font-bold text-primary-text">
              {{ t("professor_dashboard.offerings.subject") }}
            </label>
            <div class="relative">
              <div class="relative">
                <Search
                  class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-primary-text/40"
                />
                <input
                  v-model="subjectSearch"
                  type="text"
                  :placeholder="t('professor_dashboard.offerings.search_subject')"
                  class="w-full pl-10 pr-4 py-2 rounded-xl border-2 border-stone-200 focus:border-brand-teal focus:outline-none text-sm"
                  @focus="
                    showSubjectDropdown = true;
                    fetchActiveSubjects(subjectSearch);
                  "
                />
              </div>
              <div
                v-if="showSubjectDropdown && activeSubjects.length > 0"
                class="absolute z-10 w-full mt-1 bg-white border-2 border-stone-200 rounded-xl shadow-lg max-h-48 overflow-y-auto"
              >
                <div
                  v-for="subject in activeSubjects"
                  :key="subject.publicId"
                  @click="selectSubject(subject)"
                  class="px-4 py-2 hover:bg-brand-teal/10 cursor-pointer text-sm"
                >
                  <span class="font-medium">{{ subject.code }}</span> - {{ subject.name }}
                </div>
              </div>
            </div>
          </div>

          <div v-if="selectedSubjectDisplay" class="bg-brand-teal/10 rounded-xl p-3 text-sm">
            <span class="font-medium text-primary-text"
              >{{ t("professor_dashboard.offerings.selected") }}:</span
            >
            {{ selectedSubjectDisplay }}
          </div>

          <div class="space-y-2">
            <label class="block text-sm font-bold text-primary-text">
              {{ t("professor_dashboard.offerings.semester") }}
            </label>
            <div class="px-4 py-2 rounded-xl bg-stone-100 text-sm text-primary-text/70">
              {{ currentSemester?.name || "-" }}
            </div>
          </div>
        </template>

        <template v-else>
          <div class="space-y-4">
            <div class="bg-stone-100 rounded-xl p-4 space-y-2">
              <p class="text-sm">
                <span class="font-bold text-primary-text"
                  >{{ t("professor_dashboard.offerings.subject") }}:</span
                >
                {{ selectedOffering?.subject.code }} - {{ selectedOffering?.subject.name }}
              </p>
              <p class="text-sm">
                <span class="font-bold text-primary-text"
                  >{{ t("professor_dashboard.offerings.semester") }}:</span
                >
                {{ selectedOffering?.semester.name }}
              </p>
            </div>

            <div class="space-y-2">
              <label class="block text-sm font-bold text-primary-text">
                {{ t("common.status") }}
              </label>
              <StatusToggle
                :status="offeringStatus"
                :active-label="t('common.active')"
                :inactive-label="t('common.inactive')"
                @toggle="toggleStatus"
              />
            </div>
          </div>
        </template>

        <FormButtons
          :loading="creating"
          :submit-label="isEditing ? t('common.save_changes') : t('common.save')"
          :cancel-label="t('common.cancel')"
          :submit-disabled="!isEditing && !newOffering.subjectId"
          @cancel="closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
