import { ref, watch } from "vue";

export interface FilterState {
  search: string;
  includeInactive: boolean;
}

export interface UseFiltersOptions {
  debounceMs?: number;
  onFilterChange?: () => void | Promise<void>;
}

export function useFilters(options: UseFiltersOptions = {}) {
  const { debounceMs = 300, onFilterChange } = options;

  const search = ref("");
  const includeInactive = ref(false);

  let searchTimeout: ReturnType<typeof setTimeout> | null = null;

  const handleSearchChange = () => {
    if (searchTimeout) clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
      onFilterChange?.();
    }, debounceMs);
  };

  const handleFilterChange = () => {
    onFilterChange?.();
  };

  const clearFilters = () => {
    search.value = "";
    includeInactive.value = false;
    onFilterChange?.();
  };

  const hasActiveFilters = () => {
    return search.value !== "" || includeInactive.value;
  };

  watch(search, () => {
    handleSearchChange();
  });

  watch(includeInactive, () => {
    handleFilterChange();
  });

  const getFilterParams = () => ({
    search: search.value || undefined,
    includeInactive: includeInactive.value || undefined,
  });

  return {
    search,
    includeInactive,
    handleSearchChange,
    handleFilterChange,
    clearFilters,
    hasActiveFilters,
    getFilterParams,
  };
}
