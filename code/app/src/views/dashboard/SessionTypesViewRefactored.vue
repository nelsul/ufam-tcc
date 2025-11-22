<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useToast } from "vue-toastification";
import { useI18n } from "vue-i18n";
import { Plus, Pencil } from "lucide-vue-next";
import { sessionTypesService } from "../../services/sessionTypesService";
import type { SessionTypeDto } from "@/types/api";

import {
  BaseModal,
  BaseCard,
  StatusToggle,
  LoadingSpinner,
  Pagination,
  CardGrid,
} from "@/components/common";
import { FormInput, FormTextarea, FormButtons } from "@/components/form";
import { PageHeader, FilterBar } from "@/components/layout";

import { usePagination, useFilters, useCrudModal } from "@/composables";

const toast = useToast();
const { t } = useI18n();

const sessionTypes = ref<SessionTypeDto[]>([]);
const loading = ref(false);

interface SessionTypeForm {
  name: string;
  durationMinutes: number;
  description: string;
  status: string;
}

const pagination = usePagination({
  initialPageSize: 9,
  onPageChange: fetchSessionTypes,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchSessionTypes();
  },
});

const createModal = useCrudModal<SessionTypeDto, SessionTypeForm>({
  getInitialFormData: () => ({
    name: "",
    durationMinutes: 60,
    description: "",
    status: "Active",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    durationMinutes: entity.durationMinutes,
    description: entity.description,
    status: entity.status,
  }),
});

async function fetchSessionTypes() {
  loading.value = true;
  try {
    const data = await sessionTypesService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      includeInactive: filters.includeInactive.value,
      search: filters.search.value || undefined,
    });
    sessionTypes.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch session types:", error);
  } finally {
    loading.value = false;
  }
}

async function handleCreate() {
  createModal.setLoading(true);
  try {
    await sessionTypesService.create({
      name: createModal.formData.value.name,
      durationMinutes: createModal.formData.value.durationMinutes,
      description: createModal.formData.value.description,
    });
    createModal.closeModal();
    fetchSessionTypes();
  } catch (error) {
    console.error("Error creating session type:", error);
    const errorMessage = error instanceof Error ? error.message : t("common.unknown_error");
    toast.error(t("session_types.create_error") + ": " + errorMessage);
  } finally {
    createModal.setLoading(false);
  }
}

async function handleUpdate() {
  if (!createModal.entityId.value) return;

  createModal.setLoading(true);
  try {
    await sessionTypesService.update(createModal.entityId.value, createModal.formData.value);
    toast.success(t("session_types.updated"));
    createModal.closeModal();
    fetchSessionTypes();
  } catch (error) {
    console.error("Error updating session type:", error);
    const errorMessage = error instanceof Error ? error.message : t("common.unknown_error");
    toast.error(t("session_types.update_error") + ": " + errorMessage);
  } finally {
    createModal.setLoading(false);
  }
}

function toggleStatus() {
  createModal.formData.value.status =
    createModal.formData.value.status === "Active" ? "Inactive" : "Active";
}

function handleSubmit() {
  if (createModal.isEditing.value) {
    handleUpdate();
  } else {
    handleCreate();
  }
}

onMounted(() => {
  fetchSessionTypes();
});
</script>

<template>
  <div class="h-full flex flex-col relative">
    <PageHeader
      :title="t('session_types.title')"
      :subtitle="t('session_types.subtitle')"
      :button-label="t('session_types.new')"
      :show-button="true"
      :button-icon="Plus"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="createModal.openCreateModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('session_types.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" full-height />

    <CardGrid v-else>
      <BaseCard
        v-for="type in sessionTypes"
        :key="type.publicId"
        :status="type.status"
        :clickable="true"
        @click="createModal.openEditModal(type)"
      >
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="font-bold text-xl text-primary-text">{{ type.name }}</h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ type.durationMinutes }} {{ t("min") }}
            </span>
          </div>
          <div class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl">
            📝
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed">
          {{ type.description }}
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
      :show="createModal.showModal.value"
      :title="createModal.isEditing.value ? t('session_types.edit') : t('session_types.new_title')"
      :icon="createModal.isEditing.value ? Pencil : Plus"
      :icon-bg-color="createModal.isEditing.value ? 'bg-brand-orange' : 'bg-brand-teal'"
      @close="createModal.closeModal"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <FormInput
          v-model="createModal.formData.value.name"
          :label="t('common.name')"
          :placeholder="t('session_types.name_placeholder')"
          required
        />

        <FormInput
          v-model="createModal.formData.value.durationMinutes"
          :label="t('session_types.duration')"
          type="number"
          :min="1"
          required
        />

        <FormTextarea
          v-model="createModal.formData.value.description"
          :label="t('common.description')"
          :placeholder="t('session_types.description_placeholder')"
          required
        />

        <div v-if="createModal.isEditing.value">
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">
            {{ t("common.status") }}
          </label>
          <StatusToggle :status="createModal.formData.value.status" @toggle="toggleStatus" />
        </div>

        <FormButtons
          :loading="createModal.isLoading.value"
          :submit-label="
            createModal.isEditing.value ? t('common.save_changes') : t('session_types.create')
          "
          :loading-label="createModal.isEditing.value ? t('common.saving') : t('common.creating')"
          @cancel="createModal.closeModal"
        />
      </form>
    </BaseModal>
  </div>
</template>
