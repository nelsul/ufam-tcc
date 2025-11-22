<script setup lang="ts">
import { Settings, Clock, User, ChevronDown } from "lucide-vue-next";
import type { SessionTypeDto } from "@/types/api";
import type { ProfessionalDto } from "@/services/professionalsService";

defineProps<{
  sessionTypes: SessionTypeDto[];
  professionals: ProfessionalDto[];
  sessionTypeId: string;
  professionalId: string;
}>();

defineEmits<{
  (e: "update:sessionTypeId", value: string): void;
  (e: "update:professionalId", value: string): void;
}>();
</script>

<template>
  <div class="space-y-8 sm:space-y-10">
    <div
      class="bg-brand-teal/20 px-4 py-3 sm:px-6 sm:py-4 rounded-3xl border-4 border-brand-teal mb-8 sm:mb-10"
    >
      <h3 class="text-xl font-bold text-primary-text flex items-center gap-3">
        <Settings class="h-6 w-6 text-brand-teal" />
        {{ $t("session_configuration") }}
      </h3>
    </div>
    <div class="flex flex-col gap-2">
      <label for="sessionType" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <Clock class="h-5 w-5 text-brand-orange" />
        {{ $t("session_type") }}
      </label>
      <div class="relative">
        <select
          id="sessionType"
          :value="sessionTypeId"
          @change="$emit('update:sessionTypeId', $event.target.value)"
          required
          class="input-nook appearance-none"
        >
          <option value="" disabled>{{ $t("select_session_type") }}</option>
          <option v-for="type in sessionTypes" :key="type.publicId" :value="type.publicId">
            {{ type.name }} ({{ type.durationMinutes }} {{ $t("min") }})
          </option>
        </select>
        <div
          class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-primary-text"
        >
          <ChevronDown class="h-5 w-5" />
        </div>
      </div>
    </div>

    <div class="flex flex-col gap-2">
      <label for="professional" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <User class="h-5 w-5 text-brand-orange" />
        {{ $t("professional") }}
      </label>
      <div class="relative">
        <select
          id="professional"
          :value="professionalId"
          @change="$emit('update:professionalId', $event.target.value)"
          required
          :disabled="!sessionTypeId"
          class="input-nook appearance-none disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <option value="" disabled>{{ $t("select_professional") }}</option>
          <option
            v-for="professional in professionals"
            :key="professional.publicId"
            :value="professional.publicId"
          >
            {{ professional.fullName }}
          </option>
        </select>
        <div
          class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-primary-text"
        >
          <ChevronDown class="h-5 w-5" />
        </div>
      </div>
    </div>
  </div>
</template>
