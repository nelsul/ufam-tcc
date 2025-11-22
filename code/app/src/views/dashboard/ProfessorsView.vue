<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Plus, Pencil, Key, Loader2 } from "lucide-vue-next";
import { professorsService } from "../../services/professorsService";
import type { ProfessorDto } from "@/types/api";

import { usePagination, useFilters, useCrudModal, useApiToast } from "../../composables";

import { PageHeader, FilterBar } from "../../components/layout";
import { BaseModal, BaseCard, LoadingSpinner, Pagination, CardGrid } from "../../components/common";
import { FormInput, FormButtons } from "../../components/form";
import StatusToggle from "../../components/common/StatusToggle.vue";

const { t } = useI18n();
const { showSuccess, showError } = useApiToast();

interface ProfessorForm {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: string;
}

const professors = ref<ProfessorDto[]>([]);
const loading = ref(false);

const showPasswordModal = ref(false);
const resettingPassword = ref(false);
const newPassword = ref("");

const pagination = usePagination({
  onPageChange: fetchProfessors,
});

const filters = useFilters({
  onFilterChange: () => {
    pagination.resetPage();
    fetchProfessors();
  },
});

const modal = useCrudModal<ProfessorDto, ProfessorForm>({
  getInitialFormData: () => ({
    name: "",
    fullName: "",
    institutionalEmail: "",
    registration: "",
    status: "Active",
  }),
  mapEntityToForm: (entity) => ({
    name: entity.name,
    fullName: entity.fullName,
    institutionalEmail: entity.institutionalEmail,
    registration: entity.registration,
    status: entity.status,
  }),
});

async function fetchProfessors() {
  loading.value = true;
  try {
    const data = await professorsService.getAll({
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
      ...filters.getFilterParams(),
    });
    professors.value = data.items;
    pagination.updateFromResponse(data);
  } catch (error) {
    console.error("Failed to fetch professors:", error);
  } finally {
    loading.value = false;
  }
}

async function handleSubmit() {
  modal.setLoading(true);
  try {
    if (modal.isEditing.value && modal.entityId.value) {
      await professorsService.update(modal.entityId.value, modal.formData.value);
      showSuccess("professors.updated_success");
    } else {
      await professorsService.create(modal.formData.value);
      showSuccess("professors.created_success");
    }
    modal.closeModal();
    fetchProfessors();
  } catch (error) {
    console.error("Error saving professor:", error);
    showError(
      error,
      modal.isEditing.value ? "professors.error_updating" : "professors.error_creating"
    );
  } finally {
    modal.setLoading(false);
  }
}

function toggleStatus() {
  modal.formData.value.status = modal.formData.value.status === "Active" ? "Inactive" : "Active";
}

function openPasswordModal() {
  newPassword.value = "";
  showPasswordModal.value = true;
}

function closePasswordModal() {
  showPasswordModal.value = false;
  newPassword.value = "";
}

async function resetPassword() {
  if (!modal.entityId.value || !newPassword.value) return;

  resettingPassword.value = true;
  try {
    await professorsService.resetPassword(modal.entityId.value, {
      newPassword: newPassword.value,
    });
    showSuccess("professors.password_reset_success");
    closePasswordModal();
  } catch (error) {
    console.error("Error resetting password:", error);
    showError(error, "professors.error_resetting_password");
  } finally {
    resettingPassword.value = false;
  }
}

onMounted(fetchProfessors);
</script>

<template>
  <div class="h-full flex flex-col relative gap-6 pb-6">
    <PageHeader
      :title="t('professors.title')"
      :subtitle="t('professors.subtitle')"
      :button-label="t('professors.new_professor')"
      :show-button="true"
      button-color="bg-[#81F2DD] hover:bg-[#70e0cb] text-[#5E5340]"
      @action="modal.openCreateModal"
    />

    <FilterBar
      v-model:search="filters.search.value"
      v-model:include-inactive="filters.includeInactive.value"
      :search-placeholder="t('professors.search_placeholder')"
      :show-inactive-filter="true"
      @clear="filters.clearFilters"
    />

    <LoadingSpinner v-if="loading" />

    <CardGrid v-else>
      <BaseCard
        v-for="professor in professors"
        :key="professor.publicId"
        :status="professor.status"
        :clickable="true"
        @click="modal.openEditModal(professor)"
      >
        <div class="flex justify-between items-start mb-4">
          <div class="flex-1 pr-2">
            <h3
              class="font-bold text-xl text-primary-text min-h-[3.5rem] line-clamp-2 overflow-hidden"
            >
              {{ professor.name }}
            </h3>
            <span
              class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
            >
              {{ professor.registration }}
            </span>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
          >
            🎓
          </div>
        </div>
        <p class="text-primary-text/80 text-sm leading-relaxed line-clamp-3">
          {{ professor.fullName }}
        </p>
        <p class="text-primary-text/60 text-xs font-medium mt-1">
          {{ professor.institutionalEmail }}
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
        modal.isEditing.value ? t('professors.edit_professor') : t('professors.new_professor')
      "
      :icon="modal.isEditing.value ? Pencil : Plus"
      :icon-bg-color="modal.isEditing.value ? 'bg-brand-orange' : 'bg-brand-teal'"
      @close="modal.closeModal"
    >
      <form @submit.prevent="handleSubmit" class="flex flex-col gap-3">
        <FormInput
          v-model="modal.formData.value.name"
          :label="t('professors.name_short')"
          :placeholder="t('professors.name_short_placeholder')"
          required
        />

        <FormInput
          v-model="modal.formData.value.fullName"
          :label="t('professors.full_name')"
          :placeholder="t('professors.full_name_placeholder')"
          required
        />

        <FormInput
          v-model="modal.formData.value.institutionalEmail"
          :label="t('professors.institutional_email')"
          :placeholder="t('professors.email_placeholder')"
          type="email"
          required
        />

        <FormInput
          v-model="modal.formData.value.registration"
          :label="t('professors.registration')"
          :placeholder="t('professors.registration_placeholder')"
          required
        />

        <div v-if="modal.isEditing.value">
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">
            {{ t("common.status") }}
          </label>
          <StatusToggle :status="modal.formData.value.status" @toggle="toggleStatus" />
        </div>

        <div v-if="modal.isEditing.value">
          <button
            type="button"
            @click="openPasswordModal"
            class="w-full flex items-center justify-center gap-2 bg-white rounded-2xl border-2 border-amber-300 px-4 py-3 text-amber-700 font-medium hover:bg-amber-50 transition-colors"
          >
            <Key class="w-4 h-4" />
            {{ t("professors.reset_password") }}
          </button>
        </div>

        <FormButtons
          :loading="modal.isLoading.value"
          :submit-label="
            modal.isEditing.value ? t('common.save_changes') : t('professors.create_professor')
          "
          :loading-label="modal.isEditing.value ? t('common.saving') : t('common.creating')"
          @cancel="modal.closeModal"
        />
      </form>
    </BaseModal>

    <BaseModal
      :show="showPasswordModal"
      :title="t('professors.reset_password')"
      :icon="Key"
      icon-bg-color="bg-amber-500"
      max-width="max-w-sm"
      @close="closePasswordModal"
    >
      <form @submit.prevent="resetPassword" class="space-y-4">
        <FormInput
          v-model="newPassword"
          :label="t('professors.new_password')"
          :placeholder="t('professors.password_placeholder')"
          type="password"
          required
        />

        <div class="pt-4 flex gap-3">
          <button
            type="button"
            @click="closePasswordModal"
            class="flex-1 py-3 rounded-xl font-bold text-primary-text/70 hover:bg-black/5 transition-colors"
          >
            {{ t("common.cancel") }}
          </button>
          <button
            type="submit"
            :disabled="resettingPassword || newPassword.length < 6"
            class="flex-1 py-3 rounded-xl bg-amber-500 text-white font-bold hover:scale-[1.02] active:scale-95 transition-all shadow-sm border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed flex justify-center items-center gap-2"
          >
            <Loader2 v-if="resettingPassword" class="w-5 h-5 animate-spin" />
            {{ resettingPassword ? t("professors.resetting") : t("professors.reset_password") }}
          </button>
        </div>
      </form>
    </BaseModal>
  </div>
</template>
