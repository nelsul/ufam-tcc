<script setup lang="ts">
import { ref, watch, computed } from "vue";
import { Link, Search, Loader2, Check, UserPlus, X, User, Lightbulb } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import type { StudentDto } from "@/types/api";

const props = defineProps<{
  studentEmail: string;
  studentRegistration: string;
  studentFullName: string;
  searching: boolean;
  searchResults: StudentDto[];
  creating: boolean;
  linkedStudent?: StudentDto | null;
}>();

const selectedStudentId = defineModel<string | null>("selectedStudentId", { required: true });
const searchQuery = defineModel<string>("searchQuery", { required: true });

const emit = defineEmits<{
  search: [];
  createStudent: [
    data: { name: string; fullName: string; institutionalEmail: string; registration: string },
  ];
}>();

const { t } = useI18n();

const showCreateForm = ref(false);
const newStudentForm = ref({
  name: "",
  fullName: "",
  institutionalEmail: "",
  registration: "",
});

const suggestions = computed(() => {
  if (!props.searchResults.length) return [];
  return props.searchResults.filter((s) => s.publicId !== props.linkedStudent?.publicId);
});

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(searchQuery, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    emit("search");
  }, 300);
});

const selectStudent = (student: StudentDto) => {
  selectedStudentId.value = student.publicId;
};

const unlinkStudent = () => {
  selectedStudentId.value = null;
};

const openCreateForm = () => {
  const nameParts = props.studentFullName.split(" ");
  newStudentForm.value = {
    name: nameParts[0] || "",
    fullName: props.studentFullName,
    institutionalEmail: props.studentEmail,
    registration: props.studentRegistration,
  };
  showCreateForm.value = true;
};

const handleCreateStudent = () => {
  emit("createStudent", { ...newStudentForm.value });
};

watch(
  () => props.creating,
  (newVal, oldVal) => {
    if (oldVal && !newVal && selectedStudentId.value) {
      showCreateForm.value = false;
    }
  }
);
</script>

<template>
  <div class="bg-brand-yellow/20 rounded-2xl p-4 gap-3 flex flex-col">
    <h3 class="font-bold text-primary-text flex items-center gap-2">
      <Link class="w-5 h-5 text-brand-orange" />
      {{ t("appointments.link_to_student") }}
    </h3>

    <div v-if="linkedStudent" class="bg-green-50 border-2 border-brand-green rounded-xl p-3">
      <div class="flex items-center gap-2 mb-2">
        <User class="w-4 h-4 text-brand-green" />
        <span class="text-xs font-bold text-brand-green uppercase tracking-wide">
          {{ t("appointments.currently_linked") }}
        </span>
      </div>
      <div class="flex items-center justify-between">
        <div>
          <p class="font-medium text-sm text-primary-text">{{ linkedStudent.fullName }}</p>
          <p class="text-xs text-primary-text/50">
            {{ linkedStudent.registration }} • {{ linkedStudent.institutionalEmail }}
          </p>
        </div>
        <button
          @click="unlinkStudent"
          class="p-1.5 rounded-lg text-red-400 hover:bg-red-50 hover:text-red-500 transition-colors"
          :title="t('appointments.unlink')"
        >
          <X class="w-4 h-4" />
        </button>
      </div>
    </div>

    <div class="relative">
      <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-primary-text/40" />
      <input
        type="text"
        v-model="searchQuery"
        :placeholder="t('appointments.search_placeholder')"
        class="w-full pl-10 pr-4 py-2 rounded-xl border-2 border-brand-orange/30 focus:border-brand-orange outline-none text-sm"
      />
    </div>

    <div
      v-if="suggestions.length > 0 && !searchQuery"
      class="flex items-center gap-2 text-xs text-primary-text/60"
    >
      <Lightbulb class="w-3.5 h-3.5 text-brand-orange" />
      <span>{{ t("appointments.suggestions_based_on_patient") }}</span>
    </div>

    <div
      v-if="searchQuery && searchResults.length > 0"
      class="flex items-center gap-2 text-xs text-primary-text/60"
    >
      <Search class="w-3.5 h-3.5 text-brand-teal" />
      <span>{{ t("appointments.search_results") }}</span>
    </div>

    <div v-if="searching" class="flex items-center justify-center py-4">
      <Loader2 class="w-5 h-5 animate-spin text-brand-orange" />
    </div>

    <div v-else-if="suggestions.length > 0" class="space-y-2 max-h-40 overflow-y-auto">
      <div
        v-for="student in suggestions"
        :key="student.publicId"
        @click="selectStudent(student)"
        class="p-3 rounded-xl border-2 cursor-pointer transition-all"
        :class="
          selectedStudentId === student.publicId
            ? 'border-brand-green bg-green-50'
            : 'border-transparent bg-white hover:border-brand-orange/50'
        "
      >
        <div class="flex items-center justify-between">
          <div>
            <p class="font-medium text-sm text-primary-text">{{ student.fullName }}</p>
            <p class="text-xs text-primary-text/50">
              {{ student.registration }} • {{ student.institutionalEmail }}
            </p>
          </div>
          <Check v-if="selectedStudentId === student.publicId" class="w-5 h-5 text-brand-green" />
        </div>
      </div>
    </div>

    <div
      v-else-if="(searchQuery || studentEmail) && !linkedStudent"
      class="text-center py-4 text-sm text-primary-text/50"
    >
      {{ t("appointments.no_matching_students") }}
    </div>

    <button
      v-if="!showCreateForm"
      @click="openCreateForm"
      class="w-full py-2 rounded-xl border-2 border-dashed border-brand-orange/50 text-brand-orange font-medium text-sm flex items-center justify-center gap-2 hover:bg-brand-orange/10 transition-colors"
    >
      <UserPlus class="w-4 h-4" />
      {{ t("appointments.create_from_appointment") }}
    </button>

    <div v-if="showCreateForm" class="bg-white rounded-xl p-4 space-y-3">
      <div class="flex items-center justify-between">
        <h4 class="font-bold text-sm text-primary-text">
          {{ t("appointments.new_student") }}
        </h4>
        <button
          @click="showCreateForm = false"
          class="text-primary-text/40 hover:text-primary-text"
        >
          <X class="w-4 h-4" />
        </button>
      </div>

      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="text-xs text-primary-text/50 block mb-1">{{ t("common.name") }}</label>
          <input
            v-model="newStudentForm.name"
            type="text"
            class="w-full px-3 py-2 rounded-lg border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
          />
        </div>
        <div>
          <label class="text-xs text-primary-text/50 block mb-1">{{ t("registration") }}</label>
          <input
            v-model="newStudentForm.registration"
            type="text"
            class="w-full px-3 py-2 rounded-lg border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
          />
        </div>
      </div>

      <div>
        <label class="text-xs text-primary-text/50 block mb-1">{{ t("full_name") }}</label>
        <input
          v-model="newStudentForm.fullName"
          type="text"
          class="w-full px-3 py-2 rounded-lg border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>

      <div>
        <label class="text-xs text-primary-text/50 block mb-1">{{
          t("institutional_email")
        }}</label>
        <input
          v-model="newStudentForm.institutionalEmail"
          type="email"
          class="w-full px-3 py-2 rounded-lg border-2 border-brand-teal/30 focus:border-brand-teal outline-none text-sm"
        />
      </div>

      <button
        @click="handleCreateStudent"
        :disabled="creating"
        class="w-full py-2 rounded-lg bg-brand-orange text-white font-bold text-sm flex items-center justify-center gap-2 hover:scale-[1.02] active:scale-95 transition-all disabled:opacity-70"
      >
        <Loader2 v-if="creating" class="w-4 h-4 animate-spin" />
        <UserPlus v-else class="w-4 h-4" />
        {{ creating ? t("common.creating") : t("students.create_student") }}
      </button>
    </div>
  </div>
</template>
