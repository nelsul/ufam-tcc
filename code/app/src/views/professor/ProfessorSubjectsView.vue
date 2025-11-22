<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Plus, Book } from "lucide-vue-next";

import { usePagination, useFilters, useCrudModal, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import { BaseModal, BaseCard, LoadingSpinner, Pagination, CardGrid } from "../../components/common";
import { FormInput, FormTextarea, FormButtons } from "../../components/form";

import { subjectsService } from "../../services/subjectsService";

import type { SubjectDto } from "@/types/api";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

interface SubjectForm {
  name: string;
  code: string;
  description: string;
}

const subjects = ref<SubjectDto[]>([]);
const loading = ref(false);

const pagination = usePagination({
  onPageChange: fetchSubjects,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchSubjects();
  },
});

const modal = useCrudModal<SubjectDto, SubjectForm>({
  getInitialFormData: () => ({
    name: "",
    code: "",
    description: "",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    code: entity.code,
    description: entity.description,
  }),
});

async function fetchSubjects() {
  loading.value = true;
  try {
    const data = await subjectsService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    subjects.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch subjects:", error);
    showError(error, "professor_dashboard.subjects.load_failed");
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  modal.setLoading(true);
  try {
    await subjectsService.create(modal.formData.value);
    showSuccess("professor_dashboard.subjects.created_success");
    modal.closeModal();
    fetchSubjects();
  } catch (error) {
    console.error("Error creating subject:", error);
    showError(error, "professor_dashboard.subjects.error_creating");
  } finally {
    modal.setLoading(false);
  }
}

onMounted(fetchSubjects);
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <PageHeader
      :icon="Book"
      :title="t('professor_dashboard.subjects.title')"
      :subtitle="t('professor_dashboard.subjects.subtitle')"
    >
      <template #actions>
        <button
          @click="modal.openCreateModal()"
          class="px-4 py-2 bg-brand-teal text-white rounded-xl font-bold text-sm flex items-center gap-2 hover:scale-[1.02] active:scale-95 transition-all shadow-md"
        >
          <Plus class="w-4 h-4" />
          {{ t("professor_dashboard.subjects.new") }}
        </button>
      </template>
    </PageHeader>

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('professor_dashboard.subjects.search_placeholder')"
      :show-inactive-filter="false"
    />

    <LoadingSpinner v-if="loading" />

    <div
      v-else-if="subjects.length === 0"
      class="flex flex-col items-center justify-center py-20 text-primary-text/50"
    >
      <Book class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("professor_dashboard.subjects.no_subjects") }}</p>
    </div>

    <CardGrid v-else>
      <BaseCard
        v-for="subject in subjects"
        :key="subject.publicId"
        :status="subject.status"
        :clickable="false"
        :show-edit-indicator="false"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="flex-1 pr-2">
            <h3
              class="font-bold text-xl text-primary-text min-h-[3.5rem] line-clamp-2 overflow-hidden"
            >
              {{ subject.name }}
            </h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ subject.code }}
            </span>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
          >
            📚
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed line-clamp-3">
          {{ subject.description || t("common.no_description") }}
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
      :show="modal.showModal.value"
      :title="t('professor_dashboard.subjects.create_title')"
      @close="modal.closeModal()"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <FormInput
          v-model="modal.formData.value.name"
          :label="t('common.name')"
          :placeholder="t('professor_dashboard.subjects.name_placeholder')"
          required
        />

        <FormInput
          v-model="modal.formData.value.code"
          :label="t('common.code')"
          :placeholder="t('professor_dashboard.subjects.code_placeholder')"
          required
        />

        <FormTextarea
          v-model="modal.formData.value.description"
          :label="t('common.description')"
          :placeholder="t('professor_dashboard.subjects.description_placeholder')"
        />

        <FormButtons
          :loading="modal.isLoading.value"
          :submit-label="t('common.save')"
          :cancel-label="t('common.cancel')"
          @cancel="modal.closeModal()"
        />
      </form>
    </BaseModal>
  </div>
</template>
