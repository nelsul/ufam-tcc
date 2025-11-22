<script setup lang="ts">
import { ClipboardCheck, User, Clock, Calendar } from "lucide-vue-next";
import { useI18n } from "vue-i18n";

const { locale } = useI18n();

defineProps<{
  professionalName: string;
  date: string;
  startTime: string;
}>();
</script>

<template>
  <div
    class="mt-6 p-4 bg-brand-teal/10 border-4 border-dashed border-brand-teal rounded-[30px] shadow-sm relative"
  >
    <div
      class="absolute -top-3 right-4 bg-brand-yellow text-primary-text text-xs font-bold px-3 py-1 rounded-full transform rotate-3 shadow-md border-2 border-white"
    >
      {{ $t("summary") }}
    </div>

    <div class="flex items-center gap-3 mb-4 pb-3 border-b-2 border-dashed border-brand-teal/30">
      <div class="bg-brand-teal p-2 rounded-full border-2 border-white shadow-sm">
        <ClipboardCheck class="h-5 w-5 text-white" />
      </div>
      <h3 class="text-xl font-bold text-primary-text">
        {{ $t("appointment_summary") }}
      </h3>
    </div>
    <div class="flex flex-col gap-4 text-primary-text">
      <div
        class="flex items-start gap-3 bg-white p-4 rounded-2xl border-2 border-soft-gray shadow-sm"
      >
        <User class="h-5 w-5 text-brand-teal mt-0.5" />
        <div>
          <p class="text-xs text-brand-teal font-bold uppercase mb-0.5">
            {{ $t("professional") }}
          </p>
          <p class="font-bold text-base">{{ professionalName }}</p>
        </div>
      </div>
      <div
        class="flex items-start gap-3 bg-white p-4 rounded-2xl border-2 border-soft-gray shadow-sm"
      >
        <Calendar class="h-5 w-5 text-brand-teal mt-0.5" />
        <div>
          <p class="text-xs text-brand-teal font-bold uppercase mb-0.5">
            {{ $t("date") }}
          </p>
          <p class="font-bold text-base">
            {{
              new Date(date).toLocaleDateString(locale, {
                weekday: "short",
                month: "short",
                day: "numeric",
              })
            }}
          </p>
        </div>
      </div>
      <div
        class="flex items-start gap-3 bg-white p-4 rounded-2xl border-2 border-soft-gray shadow-sm"
      >
        <Clock class="h-5 w-5 text-brand-teal mt-0.5" />
        <div>
          <p class="text-xs text-brand-teal font-bold uppercase mb-0.5">
            {{ $t("time") }}
          </p>
          <p class="font-bold text-base">{{ startTime }}</p>
        </div>
      </div>
      <div
        class="bg-brand-yellow/30 p-3 rounded-xl border-2 border-dashed border-brand-yellow text-sm text-primary-text/80"
      >
        <p class="font-medium">
          {{ $t("session_type_info") }}
        </p>
      </div>
    </div>
  </div>
</template>
