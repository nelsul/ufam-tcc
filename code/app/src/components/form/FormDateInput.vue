<script setup lang="ts">
defineProps<{
  modelValue: string;
  label?: string;
  required?: boolean;
  disabled?: boolean;
  min?: string;
  max?: string;
  id?: string;
}>();

defineEmits<{
  (e: "update:modelValue", value: string): void;
}>();
</script>

<template>
  <div>
    <label v-if="label" :for="id" class="block text-sm font-bold text-primary-text mb-1 ml-2">
      {{ label }}
    </label>
    <input
      :id="id"
      :value="modelValue"
      @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
      type="date"
      :required="required"
      :disabled="disabled"
      :min="min"
      :max="max"
      class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    />
  </div>
</template>
