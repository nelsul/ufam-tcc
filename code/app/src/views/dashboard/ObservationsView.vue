<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Eye, Pencil, Plus } from "lucide-vue-next";
import { observationsService } from "../../services/observationsService";
import type { ObservationDto } from "@/types/api";

import { usePagination, useFilters, useCrudModal, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import {
  BaseModal,
  BaseCard,
  LoadingSpinner,
  Pagination,
  CardGrid,
  StatusToggle,
} from "../../components/common";
import { FormInput, FormTextarea, FormButtons } from "../../components/form";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

interface ObservationForm {
  name: string;
  description: string;
  status: string;
}

const observations = ref<ObservationDto[]>([]);
const loading = ref(false);

const pagination = usePagination({
  onPageChange: fetchObservations,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchObservations();
  },
});

const modal = useCrudModal<ObservationDto, ObservationForm>({
  getInitialFormData: () => ({
    name: "",
    description: "",
    status: "Active",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    description: entity.description ?? "",
    status: entity.status ?? "Active",
  }),
});

async function fetchObservations() {
  loading.value = true;
  try {
    const data = await observationsService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    observations.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch observations:", error);
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  modal.setLoading(true);
  try {
    if (modal.isEditing.value && modal.entityId.value) {
      await observationsService.update(modal.entityId.value, modal.formData.value);
      showSuccess("observations.updated_success");
    } else {
      await observationsService.create(modal.formData.value);
      showSuccess("observations.created_success");
    }
    modal.closeModal();
    fetchObservations();
  } catch (error) {
    console.error("Error saving observation:", error);
    showError(error, "observations.error_saving");
  } finally {
    modal.setLoading(false);
  }
}

function toggleStatus() {
  modal.formData.value.status = modal.formData.value.status === "Active" ? "Inactive" : "Active";
}

onMounted(fetchObservations);
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <PageHeader
      :title="t('observations.title')"
      :subtitle="t('observations.subtitle')"
      :button-label="t('observations.new_observation')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="modal.openCreateModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('observations.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <div
      v-else-if="observations.length === 0"
      class="flex-1 flex flex-col justify-center items-center text-primary-text/40"
    >
      <Eye class="w-16 h-16 mb-4" />
      <p class="text-lg font-medium">{{ t("observations.no_observations") }}</p>
    </div>

    <CardGrid v-else>
      <BaseCard
        v-for="obs in observations"
        :key="obs.id"
        :status="obs.status"
        :clickable="true"
        @click="modal.openEditModal(obs)"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="flex-1 pr-2">
            <h3
              class="font-bold text-xl text-primary-text min-h-[3.5rem] line-clamp-2 overflow-hidden"
            >
              {{ obs.name }}
            </h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ obs.status === "Active" ? t("common.active") : t("common.inactive") }}
            </span>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
          >
            👁️
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed line-clamp-3">
          {{ obs.description }}
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
      :title="
        modal.isEditing.value
          ? t('observations.edit_observation')
          : t('observations.new_observation')
      "
      :icon="modal.isEditing.value ? Pencil : Plus"
      :icon-bg-color="modal.isEditing.value ? 'bg-brand-orange' : 'bg-brand-teal'"
      @close="modal.closeModal"
    >
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <FormInput
          v-model="modal.formData.value.name"
          :label="t('common.name')"
          :placeholder="t('observations.name_placeholder')"
          required
        />

        <FormTextarea
          v-model="modal.formData.value.description"
          :label="t('common.description')"
          :placeholder="t('observations.description_placeholder')"
          :rows="4"
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
            modal.isEditing.value ? t('common.save_changes') : t('observations.create_observation')
          "
          :loading-label="modal.isEditing.value ? t('common.saving') : t('common.creating')"
          @cancel="modal.closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
