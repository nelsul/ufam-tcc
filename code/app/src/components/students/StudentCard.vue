<script setup lang="ts">
import type { StudentDto } from "@/types/api";
import { BaseCard } from "../common";

defineProps<{
  student: StudentDto;
}>();

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <BaseCard :status="student.status" :clickable="true" @click="$emit('click')">
    <div class="flex justify-between items-start mb-4">
      <div class="flex-1 pr-2">
        <h3 class="font-bold text-xl text-primary-text min-h-[3.5rem] line-clamp-2 overflow-hidden">
          {{ student.name }}
        </h3>
        <span
          class="text-xs font-bold px-2 py-1 rounded-lg bg-brand-orange/20 text-primary-text/70 mt-1 inline-block"
        >
          {{ student.registration }}
        </span>
      </div>
      <div
        class="w-10 h-10 rounded-full bg-soft-gray flex items-center justify-center text-xl flex-shrink-0"
      >
        🎓
      </div>
    </div>

    <p class="text-primary-text/80 text-sm leading-relaxed line-clamp-2">
      {{ student.fullName }}
    </p>
    <p class="text-primary-text/60 text-xs font-medium mt-1">
      {{ student.institutionalEmail }}
    </p>
  </BaseCard>
</template>
