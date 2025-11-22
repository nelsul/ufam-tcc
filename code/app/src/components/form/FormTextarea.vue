<script setup lang="ts">
defineProps<{
  modelValue: string;
  label?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  rows?: number;
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
    <textarea
      :id="id"
      :value="modelValue"
      @input="$emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      :rows="rows || 3"
      class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors resize-none disabled:opacity-50 disabled:cursor-not-allowed"
    />
  </div>
</template>
