<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Plus, Pencil } from "lucide-vue-next";
import { subjectsService } from "../../services/subjectsService";
import type { SubjectDto } from "@/types/api";

import { usePagination, useFilters, useCrudModal, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import { BaseModal, BaseCard, LoadingSpinner, Pagination, CardGrid } from "../../components/common";
import { FormInput, FormTextarea, FormButtons } from "../../components/form";
import StatusToggle from "../../components/common/StatusToggle.vue";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

interface SubjectForm {
  name: string;
  code: string;
  description: string;
  status: string;
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
    status: "Active",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    code: entity.code,
    description: entity.description,
    status: entity.status,
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
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  modal.setLoading(true);
  try {
    if (modal.isEditing.value && modal.entityId.value) {
      await subjectsService.update(modal.entityId.value, modal.formData.value);
      showSuccess("subjects.updated_success");
    } else {
      await subjectsService.create(modal.formData.value);
      showSuccess("subjects.created_success");
    }
    modal.closeModal();
    fetchSubjects();
  } catch (error) {
    console.error("Error saving subject:", error);
    showError(error, modal.isEditing.value ? "subjects.error_updating" : "subjects.error_creating");
  } finally {
    modal.setLoading(false);
  }
}

function toggleStatus() {
  modal.formData.value.status = modal.formData.value.status === "Active" ? "Inactive" : "Active";
}

onMounted(fetchSubjects);
</script>

<template>
  <div class="h-full flex flex-col relative gap-6 pb-6">
    <PageHeader
      :title="t('subjects.title')"
      :subtitle="t('subjects.subtitle')"
      :button-label="t('subjects.new_subject')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="modal.openCreateModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('subjects.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <CardGrid v-else>
      <BaseCard
        v-for="subject in subjects"
        :key="subject.publicId"
        :status="subject.status"
        :clickable="true"
        @click="modal.openEditModal(subject)"
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
          {{ subject.description }}
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
      :show="modal.showModal.value"
      :title="modal.isEditing.value ? t('subjects.edit_subject') : t('subjects.new_subject')"
      :icon="modal.isEditing.value ? Pencil : Plus"
      :icon-bg-color="modal.isEditing.value ? 'bg-brand-orange' : 'bg-brand-teal'"
      @close="modal.closeModal"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <FormInput
          v-model="modal.formData.value.name"
          :label="t('common.name')"
          :placeholder="t('subjects.name_placeholder')"
          required
        />

        <FormInput
          v-model="modal.formData.value.code"
          :label="t('subjects.code')"
          :placeholder="t('subjects.code_placeholder')"
          required
        />

        <FormTextarea
          v-model="modal.formData.value.description"
          :label="t('common.description')"
          :placeholder="t('subjects.description_placeholder')"
          required
        />

        <div v-if="modal.isEditing.value">
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">
            {{ t("common.status") }}
          </label>
          <StatusToggle :status="modal.formData.value.status" @toggle="toggleStatus" />
        </div>

        <FormButtons
          :loading="modal.isLoading.value"
          :submit-label="
            modal.isEditing.value ? t('common.save_changes') : t('subjects.create_subject')
          "
          :loading-label="modal.isEditing.value ? t('common.saving') : t('common.creating')"
          @cancel="modal.closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
