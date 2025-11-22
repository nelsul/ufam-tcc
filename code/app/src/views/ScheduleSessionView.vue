<script setup lang="ts">
import { ref, onMounted, watch, computed } from "vue";
import { AlertCircle } from "lucide-vue-next";
import PatientInfoForm from "../components/schedule/PatientInfoForm.vue";
import ProfessionalSelector from "../components/schedule/ProfessionalSelector.vue";
import AvailabilityGrid from "../components/schedule/AvailabilityGrid.vue";
import SessionSummary from "../components/schedule/SessionSummary.vue";
import { professionalsService, type ProfessionalDto } from "../services/professionalsService";
import { appointmentsService } from "../services/appointmentsService";
import { useApiToast } from "../composables";
import type { AvailabilityDay, AvailabilityTimeRange } from "@/types/api";

const { showSuccess, showError, getErrorMessage, getErrorCode } = useApiToast();

const professionals = ref<ProfessionalDto[]>([]);
const availabilities = ref<AvailabilityDay[]>([]);
const loading = ref(true);
const submitting = ref(false);
const loadingAvailability = ref(false);
const error = ref<string | null>(null);
const selectedSlot = ref<string | null>(null);
const selectedDate = ref<string | null>(null);
const selectedStartTime = ref<string | null>(null);

const formData = ref({
  fullName: "",
  institutionalEmail: "",
  registration: "",
  reason: "",
  professionalId: "",
  startTime: "",
});

const selectedProfessional = computed(() =>
  professionals.value.find((p) => p.publicId === formData.value.professionalId)
);

const fetchProfessionals = async () => {
  try {
    professionals.value = await professionalsService.getAll();
  } catch (err) {
    error.value = getErrorMessage(err);
    showError(err, "errors.unknown");
  } finally {
    loading.value = false;
  }
};

const fetchAvailability = async () => {
  const professionalId = formData.value.professionalId;

  if (!professionalId) return;

  loadingAvailability.value = true;
  availabilities.value = [];
  selectedSlot.value = null;
  formData.value.startTime = "";
  error.value = null;

  try {
    const rawAvailabilities = await professionalsService.getAvailability(
      professionalId,
      undefined
    );

    const defaultSlotDuration = 60;
    availabilities.value = rawAvailabilities.map((day) => {
      const newRanges = day.timeRanges.flatMap((range) =>
        splitTimeRangeISO(range.start, range.end, defaultSlotDuration)
      );
      return {
        ...day,
        timeRanges: newRanges,
      };
    });
  } catch (err) {
    console.error(err);
    showError(err, "errors.unknown");
  } finally {
    loadingAvailability.value = false;
  }
};

const splitTimeRangeISO = (
  startISO: string,
  endISO: string,
  durationMinutes: number
): AvailabilityTimeRange[] => {
  const slots: AvailabilityTimeRange[] = [];

  if (!durationMinutes || durationMinutes <= 0) {
    return slots;
  }

  let current = new Date(startISO);
  const end = new Date(endISO);

  let iterations = 0;
  const maxIterations = 100;

  while (iterations < maxIterations) {
    const nextSlot = new Date(current.getTime() + durationMinutes * 60000);

    if (nextSlot > end) break;

    slots.push({
      start: formatTime(current),
      end: formatTime(nextSlot),
      startISO: current.toISOString(),
    });

    current = nextSlot;
    iterations++;
  }

  return slots;
};

const formatTime = (date: Date) => {
  return date.toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
};

const selectSlot = (day: AvailabilityDay, slot: AvailabilityTimeRange) => {
  const slotId = `${day.date}-${slot.start}`;
  selectedSlot.value = slotId;
  selectedDate.value = day.date;
  selectedStartTime.value = slot.start;

  formData.value.startTime = slot.startISO || slot.start;
};

watch(
  () => formData.value.professionalId,
  (newProfessionalId) => {
    if (newProfessionalId) {
      fetchAvailability();
    } else {
      availabilities.value = [];
    }
  }
);

const submitForm = async () => {
  submitting.value = true;
  error.value = null;

  const payload = {
    professionalId: formData.value.professionalId,
    studentEmail: formData.value.institutionalEmail,
    studentRegistration: formData.value.registration,
    studentFullName: formData.value.fullName,
    startTime: new Date(formData.value.startTime).toISOString(),
    reasonForVisit: formData.value.reason,
  };

  try {
    await appointmentsService.create(payload);

    showSuccess("schedule.appointment_requested");

    formData.value = {
      fullName: "",
      institutionalEmail: "",
      registration: "",
      reason: "",
      professionalId: "",
      startTime: "",
    };
    selectedSlot.value = null;
    availabilities.value = [];
  } catch (err) {
    const errorMsg = getErrorMessage(err);
    error.value = errorMsg;
    showError(err);

    const errorCode = getErrorCode(err);
    if (errorCode === "Appointment.Overlap") {
      selectedSlot.value = null;
      formData.value.startTime = "";
      await fetchAvailability();
    }
  } finally {
    submitting.value = false;
  }
};

onMounted(() => {
  fetchProfessionals();
});
</script>

<template>
  <div class="flex flex-col gap-6 pb-6">
    <div
      class="text-center bg-brand-yellow/30 p-4 rounded-[30px] border-4 border-brand-yellow/50 border-dashed"
    >
      <h1 class="text-2xl font-bold text-primary-text mb-1">
        {{ $t("schedule_session") }}
      </h1>
      <p class="text-sm text-primary-text/80 font-bold">
        {{ $t("book_appointment") }}
      </p>
    </div>

    <div v-if="loading" class="flex flex-col items-center justify-center py-10">
      <div class="animate-spin rounded-full h-10 w-10 border-b-4 border-brand-teal mb-4"></div>
      <p class="text-primary-text font-bold">{{ $t("loading") }}</p>
    </div>

    <template v-else>
      <div
        v-if="error"
        class="bg-red-50 border-2 border-red-200 text-red-700 px-4 py-3 rounded-2xl flex items-start gap-3"
      >
        <AlertCircle class="w-5 h-5 flex-shrink-0 mt-0.5" />
        <div>
          <p class="font-bold text-sm">{{ error }}</p>
          <p class="text-xs text-red-600 mt-1">Please try again or select a different time slot.</p>
        </div>
      </div>

      <form @submit.prevent="submitForm" class="flex flex-col gap-8">
        <PatientInfoForm
          v-model:full-name="formData.fullName"
          v-model:institutional-email="formData.institutionalEmail"
          v-model:registration="formData.registration"
          v-model:reason="formData.reason"
        />

        <ProfessionalSelector
          :professionals="professionals"
          v-model:professional-id="formData.professionalId"
        />

        <AvailabilityGrid
          v-if="formData.professionalId"
          :availabilities="availabilities"
          :loading="loadingAvailability"
          :selected-slot="selectedSlot"
          @select-slot="({ day, slot }) => selectSlot(day, slot)"
        />

        <SessionSummary
          v-if="selectedSlot && selectedDate && selectedStartTime"
          :professional-name="selectedProfessional?.fullName ?? ''"
          :date="selectedDate"
          :start-time="selectedStartTime"
        />

        <button
          type="submit"
          class="btn-nook w-full flex items-center justify-center gap-2 mt-2 disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="
            !formData.startTime ||
            !formData.reason ||
            !formData.fullName ||
            !formData.institutionalEmail ||
            !formData.registration ||
            submitting
          "
        >
          <span v-if="submitting" class="flex items-center justify-center gap-2">
            <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
            Submitting...
          </span>
          <span v-else class="flex items-center justify-center gap-2">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-8.707l-3-3a1 1 0 00-1.414 1.414L10.586 9H7a1 1 0 100 2h3.586l-1.293 1.293a1 1 0 101.414 1.414l3-3a1 1 0 000-1.414z"
                clip-rule="evenodd"
              />
            </svg>
            {{ $t("request_session") }}
          </span>
        </button>
      </form>
    </template>
  </div>
</template>
