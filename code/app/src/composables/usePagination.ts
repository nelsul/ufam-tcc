import { ref, computed } from "vue";

export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}

export interface UsePaginationOptions {
  initialPageSize?: number;
  onPageChange?: () => void | Promise<void>;
}

export function usePagination(options: UsePaginationOptions = {}) {
  const { initialPageSize = 9, onPageChange } = options;

  const pageNumber = ref(1);
  const pageSize = ref(initialPageSize);
  const totalPages = ref(0);
  const totalCount = ref(0);

  const canGoNext = computed(() => pageNumber.value < totalPages.value);
  const canGoPrev = computed(() => pageNumber.value > 1);
  const hasMultiplePages = computed(() => totalPages.value > 1);

  const nextPage = async () => {
    if (canGoNext.value) {
      pageNumber.value++;
      await onPageChange?.();
    }
  };

  const prevPage = async () => {
    if (canGoPrev.value) {
      pageNumber.value--;
      await onPageChange?.();
    }
  };

  const goToPage = async (page: number) => {
    if (page >= 1 && page <= totalPages.value && page !== pageNumber.value) {
      pageNumber.value = page;
      await onPageChange?.();
    }
  };

  const resetPage = () => {
    pageNumber.value = 1;
  };

  const updateFromResponse = (response: {
    totalPages?: number;
    totalCount?: number;
    pageNumber?: number;
  }) => {
    if (response.totalPages !== undefined) totalPages.value = response.totalPages;
    if (response.totalCount !== undefined) totalCount.value = response.totalCount;
    if (response.pageNumber !== undefined) pageNumber.value = response.pageNumber;
  };

  return {
    pageNumber,
    pageSize,
    totalPages,
    totalCount,
    canGoNext,
    canGoPrev,
    hasMultiplePages,
    nextPage,
    prevPage,
    goToPage,
    resetPage,
    updateFromResponse,
  };
}
