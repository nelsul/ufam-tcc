<script setup lang="ts">
import { User, Mail } from "lucide-vue-next";
import { useI18n } from "vue-i18n";

defineProps<{
  fullName: string;
  registration: string;
  email: string;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="bg-white rounded-2xl p-4 space-y-3">
    <h3 class="font-bold text-primary-text flex items-center gap-2 pb-3">
      <User class="w-5 h-5 text-brand-teal" />
      {{ t("appointments.patient_information") }}
    </h3>
    <div class="grid grid-cols-2 gap-4 text-sm">
      <div>
        <span class="text-primary-text/50">{{ t("common.name") }}</span>
        <p class="font-medium text-primary-text">{{ fullName }}</p>
      </div>
      <div>
        <span class="text-primary-text/50">{{ t("registration") }}</span>
        <p class="font-medium text-primary-text">{{ registration }}</p>
      </div>
      <div class="col-span-2">
        <span class="text-primary-text/50 flex items-center gap-1">
          <Mail class="w-3 h-3" /> {{ t("email") }}
        </span>
        <p class="font-medium text-primary-text">{{ email }}</p>
      </div>
    </div>
  </div>
</template>
