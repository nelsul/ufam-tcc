import { ref, computed } from "vue";

export interface UseCrudModalOptions<T, F> {
  getInitialFormData: () => F;
  mapEntityToForm?: (entity: T) => F;
}

export function useCrudModal<T extends { publicId?: string; id?: string }, F>(
  options: UseCrudModalOptions<T, F>
) {
  const { getInitialFormData, mapEntityToForm } = options;

  const showModal = ref(false);
  const isEditing = ref(false);
  const isLoading = ref(false);
  const selectedEntity = ref<T | null>(null) as { value: T | null };
  const formData = ref<F>(getInitialFormData()) as { value: F };

  const entityId = computed(() => {
    if (!selectedEntity.value) return null;
    return selectedEntity.value.publicId || selectedEntity.value.id || null;
  });

  const openCreateModal = () => {
    isEditing.value = false;
    selectedEntity.value = null;
    formData.value = getInitialFormData();
    showModal.value = true;
  };

  const openEditModal = (entity: T) => {
    isEditing.value = true;
    selectedEntity.value = entity;
    formData.value = mapEntityToForm ? mapEntityToForm(entity) : ({ ...entity } as unknown as F);
    showModal.value = true;
  };

  const closeModal = () => {
    showModal.value = false;
    isEditing.value = false;
    selectedEntity.value = null;
  };

  const setLoading = (loading: boolean) => {
    isLoading.value = loading;
  };

  const resetForm = () => {
    formData.value = getInitialFormData();
  };

  return {
    showModal,
    isEditing,
    isLoading,
    selectedEntity,
    formData,
    entityId,
    openCreateModal,
    openEditModal,
    closeModal,
    setLoading,
    resetForm,
  };
}
