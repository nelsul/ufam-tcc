<script setup lang="ts">
import { ref, onMounted, computed, watch } from "vue";
import { Clock, Loader2 } from "lucide-vue-next";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useApiToast, usePagination } from "../../composables";
import { Pagination } from "../../components/common";
import {
  CurrentSessionCard,
  SessionFilters,
  SessionCard,
  SessionDetailsModal,
  EndSessionModal,
} from "../../components/sessions";
import {
  sessionsService,
  type SessionDto,
  type SessionFilters as ISessionFilters,
} from "../../services/sessionsService";
import { patientRecordsService } from "../../services/patientRecordsService";

const { showSuccess, showError } = useApiToast();
const { t } = useI18n();
const router = useRouter();

const sessions = ref<SessionDto[]>([]);
const openSession = ref<SessionDto | null>(null);
const loading = ref(false);
const endingSession = ref(false);
const sessionNotes = ref("");

const pagination = usePagination({
  initialPageSize: 9,
  onPageChange: fetchSessions,
});

const searchQuery = ref("");
const dateFrom = ref("");
const dateTo = ref("");

const maxDateTo = computed(() => {
  if (!dateFrom.value) return "";
  const from = new Date(dateFrom.value);
  from.setDate(from.getDate() + 14);
  return from.toISOString().split("T")[0];
});

const minDateFrom = computed(() => {
  if (!dateTo.value) return "";
  const to = new Date(dateTo.value);
  to.setDate(to.getDate() - 14);
  return to.toISOString().split("T")[0];
});

const showEndSessionModal = ref(false);
const sessionToEnd = ref<SessionDto | null>(null);

const showDetailsModal = ref(false);
const selectedSession = ref<SessionDto | null>(null);

const openSessionDetails = (session: SessionDto) => {
  selectedSession.value = session;
  showDetailsModal.value = true;
};

const closeSessionDetails = () => {
  showDetailsModal.value = false;
  selectedSession.value = null;
};

async function fetchSessions() {
  loading.value = true;
  try {
    const filters: ISessionFilters = {
      pageNumber: pagination.pageNumber.value,
      pageSize: pagination.pageSize.value,
    };

    if (searchQuery.value) {
      filters.search = searchQuery.value;
    }
    if (dateFrom.value) {
      filters.dateFrom = new Date(dateFrom.value).toISOString();
    }
    if (dateTo.value) {
      const endDate = new Date(dateTo.value);
      endDate.setHours(23, 59, 59, 999);
      filters.dateTo = endDate.toISOString();
    }

    const [sessionsResult, open] = await Promise.all([
      sessionsService.getMySessions(filters),
      sessionsService.getMyOpenSession(),
    ]);
    sessions.value = sessionsResult.items;
    openSession.value = open;
    pagination.updateFromResponse(sessionsResult);
  } catch (error) {
    console.error("Failed to fetch sessions:", error);
    showError(error, "sessions.load_failed");
  } finally {
    loading.value = false;
  }
}

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(searchQuery, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pagination.resetPage();
    fetchSessions();
  }, 300);
});

watch([dateFrom, dateTo], () => {
  pagination.resetPage();
  fetchSessions();
});

const clearFilters = () => {
  searchQuery.value = "";
  dateFrom.value = "";
  dateTo.value = "";
  pagination.resetPage();
  fetchSessions();
};

const hasActiveFilters = computed(() => !!(searchQuery.value || dateFrom.value || dateTo.value));

const openEndSessionModal = (session: SessionDto) => {
  sessionToEnd.value = session;
  sessionNotes.value = session.notes || "";
  showEndSessionModal.value = true;
};

const closeEndSessionModal = () => {
  showEndSessionModal.value = false;
  sessionToEnd.value = null;
  sessionNotes.value = "";
};

const endSession = async () => {
  if (!sessionToEnd.value) return;

  endingSession.value = true;
  try {
    await sessionsService.endSession(sessionToEnd.value.id, sessionNotes.value);
    showSuccess("sessions.ended_success");
    closeEndSessionModal();
    await fetchSessions();
  } catch (error) {
    console.error("Failed to end session:", error);
    showError(error, "sessions.end_failed");
  } finally {
    endingSession.value = false;
  }
};

const pastSessions = computed(() =>
  sessions.value.filter((s) => s.status.toLowerCase() !== "inprogress")
);

const openingRecord = ref<string | null>(null);

const openPatientRecord = async (session: SessionDto) => {
  openingRecord.value = session.id;
  try {
    const records = await patientRecordsService.getByStudentId(session.studentId);

    if (records && records.length > 0 && records[0]) {
      router.push({
        name: "dashboard-patient-record",
        params: { recordId: records[0].id },
        query: { studentId: session.studentId },
      });
    } else {
      const newRecord = await patientRecordsService.create({
        studentId: session.studentId,
        content: "",
      });
      showSuccess("sessions.patient_record_created");
      router.push({
        name: "dashboard-patient-record",
        params: { recordId: newRecord.id },
        query: { studentId: session.studentId },
      });
    }
  } catch (error) {
    console.error("Failed to open patient record:", error);
    showError(error, "sessions.patient_record_failed");
  } finally {
    openingRecord.value = null;
  }
};

onMounted(fetchSessions);
</script>

<template>
  <div class="h-full flex flex-col gap-6 pb-6">
    <header class="mb-4 md:mb-8">
      <h1 class="text-2xl md:text-4xl font-bold text-primary-text mb-2">
        {{ t("sessions.title") }}
      </h1>
      <p class="text-sm md:text-base text-primary-text/60">{{ t("sessions.subtitle") }}</p>
    </header>

    <div v-if="loading" class="flex items-center justify-center py-20">
      <Loader2 class="w-10 h-10 animate-spin text-brand-teal" />
    </div>

    <template v-else>
      <CurrentSessionCard
        v-if="openSession"
        :session="openSession"
        :opening-record="openingRecord"
        @open-patient-record="openPatientRecord"
        @open-end-modal="openEndSessionModal"
      />

      <div v-else class="mb-6 bg-stone-100 rounded-3xl p-6 border-2 border-stone-200">
        <p class="text-primary-text/60 text-center">
          {{ t("sessions.no_open_session") }}
        </p>
      </div>

      <div class="flex flex-col gap-4">
        <h2 class="text-lg font-bold text-primary-text mb-4 flex items-center">
          <Clock class="w-5 h-5 text-brand-teal" />
          {{ t("sessions.past_sessions") }}
        </h2>

        <SessionFilters
          v-model:search="searchQuery"
          v-model:date-from="dateFrom"
          v-model:date-to="dateTo"
          :max-date-to="maxDateTo"
          :min-date-from="minDateFrom"
          :has-active-filters="hasActiveFilters"
          @clear-filters="clearFilters"
        />

        <div
          v-if="pastSessions.length === 0"
          class="flex flex-col items-center justify-center py-20 text-primary-text/50"
        >
          <Clock class="w-16 h-16 mb-4" />
          <p class="text-lg font-medium">{{ t("sessions.no_sessions") }}</p>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <SessionCard
            v-for="session in pastSessions"
            :key="session.id"
            :session="session"
            :opening-record="openingRecord"
            @click="openSessionDetails(session)"
            @open-patient-record="openPatientRecord"
          />
        </div>

        <Pagination
          :page-number="pagination.pageNumber.value"
          :total-pages="pagination.totalPages.value"
          :can-go-prev="pagination.canGoPrev.value"
          :can-go-next="pagination.canGoNext.value"
          @prev="pagination.prevPage"
          @next="pagination.nextPage"
        />
      </div>
    </template>

    <SessionDetailsModal
      v-if="showDetailsModal && selectedSession"
      :session="selectedSession"
      :opening-record="openingRecord"
      @close="closeSessionDetails"
      @open-patient-record="openPatientRecord"
    />

    <EndSessionModal
      v-if="showEndSessionModal && sessionToEnd"
      :session="sessionToEnd"
      v-model:notes="sessionNotes"
      :loading="endingSession"
      @close="closeEndSessionModal"
      @confirm="endSession"
    />
  </div>
</template>
