<script setup lang="ts">
const props = defineProps<{
  modelValue: string | number;
  label?: string;
  type?: "text" | "email" | "password" | "number" | "tel";
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  min?: number;
  max?: number;
  step?: number;
  id?: string;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: string | number): void;
}>();

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const value = props.type === "number" ? Number(target.value) : target.value;
  emit("update:modelValue", value);
};
</script>

<template>
  <div>
    <label v-if="label" :for="id" class="block text-sm font-bold text-primary-text mb-1 ml-2">
      {{ label }}
    </label>
    <input
      :id="id"
      :value="modelValue"
      @input="handleInput"
      :type="type || 'text'"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      :min="min"
      :max="max"
      :step="step"
      class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    />
  </div>
</template>
