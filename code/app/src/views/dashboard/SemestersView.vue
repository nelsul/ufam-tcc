<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Plus, Pencil } from "lucide-vue-next";
import { semestersService } from "../../services/semestersService";
import type { SemesterDto } from "@/types/api";

import { usePagination, useFilters, useCrudModal, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import { BaseModal, BaseCard, LoadingSpinner, Pagination, CardGrid } from "../../components/common";
import { FormInput, FormDateInput, FormButtons } from "../../components/form";
import StatusToggle from "../../components/common/StatusToggle.vue";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

interface SemesterForm {
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}

const semesters = ref<SemesterDto[]>([]);
const loading = ref(false);

const pagination = usePagination({
  onPageChange: fetchSemesters,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchSemesters();
  },
});

const formatDateForInput = (dateString: string) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toISOString().split("T")[0];
};

const modal = useCrudModal<SemesterDto, SemesterForm>({
  getInitialFormData: () => ({
    name: "",
    startDate: "",
    endDate: "",
    status: "Active",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    startDate: formatDateForInput(entity.startDate) || "",
    endDate: formatDateForInput(entity.endDate) || "",
    status: entity.status,
  }),
});

async function fetchSemesters() {
  loading.value = true;
  try {
    const data = await semestersService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    semesters.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch semesters:", error);
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  modal.setLoading(true);
  try {
    if (modal.isEditing.value && modal.entityId.value) {
      await semestersService.update(modal.entityId.value, modal.formData.value);
      showSuccess("semesters.updated_success");
    } else {
      await semestersService.create(modal.formData.value);
      showSuccess("semesters.created_success");
    }
    modal.closeModal();
    fetchSemesters();
  } catch (error) {
    console.error("Error saving semester:", error);
    showError(
      error,
      modal.isEditing.value ? "semesters.error_updating" : "semesters.error_creating"
    );
  } finally {
    modal.setLoading(false);
  }
}

function toggleStatus() {
  modal.formData.value.status = modal.formData.value.status === "Active" ? "Inactive" : "Active";
}

const formatDate = (dateString: string) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

onMounted(fetchSemesters);
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <PageHeader
      :title="t('semesters.title')"
      :subtitle="t('semesters.subtitle')"
      :button-label="t('semesters.new_semester')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="modal.openCreateModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('semesters.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <CardGrid v-else>
      <BaseCard
        v-for="semester in semesters"
        :key="semester.publicId"
        :status="semester.status"
        :clickable="true"
        @click="modal.openEditModal(semester)"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="flex-1 pr-2">
            <h3
              class="font-bold text-xl text-[#5E5340] min-h-[3.5rem] line-clamp-2 overflow-hidden"
            >
              {{ semester.name }}
            </h3>
            <div class="flex flex-col gap-1 mt-2">
              <span class="text-xs font-bold text-[#5E5340]/60">
                {{ t("semesters.start_date") }}: {{ formatDate(semester.startDate) }}
              </span>
              <span class="text-xs font-bold text-[#5E5340]/60">
                {{ t("semesters.end_date") }}: {{ formatDate(semester.endDate) }}
              </span>
            </div>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-[#E9E7D8] flex items-center justify-center text-xl flex-shrink-0"
          >
            📅
          </div>
        </div>
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
      :title="modal.isEditing.value ? t('semesters.edit_semester') : t('semesters.new_semester')"
      :icon="modal.isEditing.value ? Pencil : Plus"
      :icon-bg-color="modal.isEditing.value ? 'bg-brand-orange' : 'bg-brand-teal'"
      @close="modal.closeModal"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <FormInput
          v-model="modal.formData.value.name"
          :label="t('common.name')"
          :placeholder="t('semesters.name_placeholder')"
          required
        />

        <FormDateInput
          v-model="modal.formData.value.startDate"
          :label="t('semesters.start_date')"
          required
        />

        <FormDateInput
          v-model="modal.formData.value.endDate"
          :label="t('semesters.end_date')"
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
            modal.isEditing.value ? t('common.save_changes') : t('semesters.create_semester')
          "
          :loading-label="modal.isEditing.value ? t('common.saving') : t('common.creating')"
          @cancel="modal.closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
