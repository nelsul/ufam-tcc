<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { X, Edit, KeyRound, ToggleLeft, ToggleRight } from "lucide-vue-next";
import PatientRecordSection from "./PatientRecordSection.vue";
import PatientObservationsSection from "./PatientObservationsSection.vue";
import SubjectEnrollmentsSection from "./SubjectEnrollmentsSection.vue";
import type {
  StudentDto,
  PatientRecordDto,
  PatientObservationWithId,
  EnrollmentDto,
  ObservationWithId,
  SubjectOfferingDto,
} from "@/types/api";

const { t } = useI18n();

defineProps<{
  show: boolean;
  student: StudentDto | null;
  loadingDetails: boolean;
  patientRecord: PatientRecordDto | null;
  patientObservations: PatientObservationWithId[];
  enrollments: EnrollmentDto[];
  observationSearchQuery: string;
  availableObservations: ObservationWithId[];
  enrollmentSearchQuery: string;
  availableOfferings: SubjectOfferingDto[];
}>();

const _emit = defineEmits<{
  (e: "close"): void;
  (e: "edit-student"): void;
  (e: "reset-password"): void;
  (e: "toggle-status"): void;
  (e: "open-record"): void;
  (e: "update:observationSearchQuery", value: string): void;
  (e: "search-observations"): void;
  (e: "add-observation", obs: ObservationWithId): void;
  (e: "edit-observation", obs: PatientObservationWithId): void;
  (e: "delete-observation", id: string): void;
  (e: "update:enrollmentSearchQuery", value: string): void;
  (e: "search-offerings"): void;
  (e: "enroll", offering: SubjectOfferingDto): void;
  (e: "remove-enrollment", id: string): void;
}>();
</script>

<template>
  <div
    v-if="show && student"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm"
  >
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-2xl shadow-2xl border-4 border-white relative max-h-[90vh] overflow-y-auto flex flex-col gap-8"
    >
      <button
        @click="$emit('close')"
        class="absolute top-6 right-6 text-[#5E5340]/50 hover:text-[#5E5340] transition-colors z-10"
      >
        <X class="w-6 h-6" />
      </button>

      <button
        @click="$emit('edit-student')"
        class="absolute top-6 right-16 w-8 h-8 bg-white border border-stone-200 rounded-full flex items-center justify-center text-[#5E5340] hover:bg-[#FDFDF5] hover:scale-110 transition-all shadow-sm z-10"
        :title="t('students.edit_student')"
      >
        <Edit class="w-4 h-4" />
      </button>

      <button
        @click="$emit('reset-password')"
        class="absolute top-6 right-26 w-8 h-8 bg-white border border-stone-200 rounded-full flex items-center justify-center text-[#5E5340] hover:bg-[#FDFDF5] hover:scale-110 transition-all shadow-sm z-10"
        :title="t('students.reset_password')"
      >
        <KeyRound class="w-4 h-4" />
      </button>

      <button
        @click="$emit('toggle-status')"
        class="absolute top-6 right-36 w-8 h-8 bg-white border border-stone-200 rounded-full flex items-center justify-center hover:bg-[#FDFDF5] hover:scale-110 transition-all shadow-sm z-10"
        :class="student?.status === 'Active' ? 'text-[#78D879]' : 'text-stone-400'"
        :title="student?.status === 'Active' ? t('common.deactivate') : t('common.activate')"
      >
        <ToggleRight v-if="student?.status === 'Active'" class="w-4 h-4" />
        <ToggleLeft v-else class="w-4 h-4" />
      </button>

      <div class="pr-24">
        <h2 class="text-3xl font-bold text-[#5E5340] mb-1">
          {{ student.name }}
        </h2>
        <div class="flex items-center gap-2 text-[#5E5340]/60">
          <p>{{ student.fullName }}</p>
          <span class="w-1 h-1 rounded-full bg-[#5E5340]/30"></span>
          <span
            class="text-xs font-bold px-2 py-0.5 rounded-full"
            :class="
              student.status === 'Active'
                ? 'bg-[#78D879]/20 text-[#78D879]'
                : 'bg-stone-200 text-stone-500'
            "
          >
            {{ student.status === "Active" ? t("common.active") : t("common.inactive") }}
          </span>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="md:col-span-2 bg-[#FDFDF5] rounded-2xl p-3 shadow-sm border border-stone-200">
          <p class="text-[#F6A95B] text-xs uppercase tracking-wide font-bold mb-1">
            {{ t("common.email") }}
          </p>
          <p class="text-[#5E5340] text-lg font-bold truncate">
            {{ student.institutionalEmail }}
          </p>
        </div>
        <div class="bg-[#FDFDF5] rounded-2xl p-3 shadow-sm border border-stone-200">
          <p class="text-[#F6A95B] text-xs uppercase tracking-wide font-bold mb-1">
            {{ t("common.registration") }}
          </p>
          <p class="text-[#5E5340] text-lg font-bold">
            {{ student.registration }}
          </p>
        </div>
      </div>

      <div class="flex flex-col gap-8">
        <PatientRecordSection
          :record="patientRecord"
          :loading="loadingDetails"
          @open-record="$emit('open-record')"
        />

        <PatientObservationsSection
          :observations="patientObservations"
          :loading="loadingDetails"
          :search-query="observationSearchQuery"
          :available-observations="availableObservations"
          @update:search-query="$emit('update:observationSearchQuery', $event)"
          @search="$emit('search-observations')"
          @add-observation="$emit('add-observation', $event)"
          @edit-observation="$emit('edit-observation', $event)"
          @delete-observation="$emit('delete-observation', $event)"
        />

        <SubjectEnrollmentsSection
          :enrollments="enrollments"
          :loading="loadingDetails"
          :search-query="enrollmentSearchQuery"
          :available-offerings="availableOfferings"
          @update:search-query="$emit('update:enrollmentSearchQuery', $event)"
          @search="$emit('search-offerings')"
          @enroll="$emit('enroll', $event)"
          @remove="$emit('remove-enrollment', $event)"
        />
      </div>
    </div>
  </div>
</template>
