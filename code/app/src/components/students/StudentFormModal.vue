<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { X, Plus, Edit, Loader2 } from "lucide-vue-next";
import type { StudentFormData } from "@/types/api";

const { t } = useI18n();

const props = defineProps<{
  show: boolean;
  isEditing: boolean;
  loading: boolean;
  initialData: StudentFormData;
}>();

const emit = defineEmits<{
  (e: "close"): void;
  (e: "save", data: StudentFormData): void;
}>();

const formData = ref<StudentFormData>({ ...props.initialData });

watch(
  () => props.initialData,
  (newVal) => {
    formData.value = { ...newVal };
  },
  { deep: true }
);

const handleSubmit = () => {
  emit("save", formData.value);
};
</script>

<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm"
  >
    <div
      class="bg-[#FDFDF5] rounded-[40px] p-8 w-full max-w-md shadow-2xl border-4 border-white relative"
    >
      <button
        @click="$emit('close')"
        class="absolute top-6 right-6 text-primary-text/50 hover:text-primary-text transition-colors"
      >
        <X class="w-6 h-6" />
      </button>

      <h2 class="text-2xl font-bold text-primary-text mb-6 flex items-center gap-2">
        <span
          class="w-8 h-8 bg-brand-teal rounded-full flex items-center justify-center text-white text-sm"
        >
          <Plus v-if="!isEditing" class="w-5 h-5" />
          <Edit v-else class="w-5 h-5" />
        </span>
        {{ isEditing ? t("students.edit_student") : t("students.new_student") }}
      </h2>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">{{
            t("common.name")
          }}</label>
          <input
            v-model="formData.name"
            type="text"
            required
            :placeholder="t('students.name_placeholder')"
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors"
          />
        </div>

        <div>
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">{{
            t("common.full_name")
          }}</label>
          <input
            v-model="formData.fullName"
            type="text"
            required
            :placeholder="t('students.full_name_placeholder')"
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors"
          />
        </div>

        <div>
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">{{
            t("common.email")
          }}</label>
          <input
            v-model="formData.institutionalEmail"
            type="email"
            required
            :placeholder="t('students.email_placeholder')"
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors"
          />
        </div>

        <div>
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">{{
            t("common.registration")
          }}</label>
          <input
            v-model="formData.registration"
            type="text"
            required
            :placeholder="t('students.registration_placeholder')"
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors"
          />
        </div>

        <div v-if="!isEditing">
          <label class="block text-sm font-bold text-primary-text mb-1 ml-2">{{
            t("common.password")
          }}</label>
          <input
            v-model="formData.password"
            type="password"
            required
            :placeholder="t('students.password_placeholder')"
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors"
          />
        </div>

        <div class="pt-4 flex gap-3">
          <button
            type="button"
            @click="$emit('close')"
            class="flex-1 py-3 rounded-xl font-bold text-primary-text/70 hover:bg-black/5 transition-colors"
          >
            {{ t("common.cancel") }}
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="flex-1 py-3 rounded-xl bg-brand-teal text-white font-bold hover:scale-[1.02] active:scale-95 transition-all shadow-sm border-2 border-white disabled:opacity-70 disabled:cursor-not-allowed flex justify-center items-center gap-2"
          >
            <Loader2 v-if="loading" class="w-5 h-5 animate-spin" />
            {{
              loading
                ? t("common.saving")
                : isEditing
                  ? t("common.save_changes")
                  : t("students.create_student")
            }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
