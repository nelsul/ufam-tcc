<script setup lang="ts">
import { User, Mail, CreditCard, FileText } from "lucide-vue-next";

defineProps<{
  fullName: string;
  institutionalEmail: string;
  registration: string;
  reason: string;
}>();

defineEmits<{
  (e: "update:fullName", value: string): void;
  (e: "update:institutionalEmail", value: string): void;
  (e: "update:registration", value: string): void;
  (e: "update:reason", value: string): void;
}>();
</script>

<template>
  <div class="sflex flex-col gap-2">
    <div
      class="bg-brand-green/20 px-4 py-3 sm:px-6 sm:py-4 rounded-3xl border-4 border-brand-green mb-8 sm:mb-10"
    >
      <h3 class="text-xl font-bold text-primary-text flex items-center gap-3">
        <User class="h-6 w-6 text-brand-green"></User>
        {{ $t("personal_information") }}
      </h3>
    </div>
    <div class="flex flex-col gap-2">
      <label for="fullName" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <User class="h-5 w-5 text-brand-orange"></User>
        {{ $t("full_name") }}
      </label>
      <input
        id="fullName"
        :value="fullName"
        @input="$emit('update:fullName', $event.target.value)"
        type="text"
        required
        :placeholder="$t('enter_full_name')"
        class="input-nook"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label for="email" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <Mail class="h-5 w-5 text-brand-orange"></Mail>
        {{ $t("institutional_email") }}
      </label>
      <input
        id="email"
        :value="institutionalEmail"
        @input="$emit('update:institutionalEmail', $event.target.value)"
        type="email"
        required
        placeholder="email@icomp.ufam.edu.br"
        class="input-nook"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label for="registration" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <CreditCard class="h-5 w-5 text-brand-orange"></CreditCard>
        {{ $t("registration") }}
      </label>
      <input
        id="registration"
        :value="registration"
        @input="$emit('update:registration', $event.target.value)"
        type="text"
        required
        :placeholder="$t('enter_registration')"
        class="input-nook"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label for="reason" class="font-bold text-primary-text flex items-center gap-2 text-lg">
        <FileText class="h-5 w-5 text-brand-orange"></FileText>
        {{ $t("reason") }}
      </label>
      <textarea
        id="reason"
        :value="reason"
        @input="$emit('update:reason', $event.target.value)"
        required
        :placeholder="$t('enter_reason')"
        class="input-nook h-32 resize-none py-3"
      ></textarea>
    </div>
  </div>
</template>
