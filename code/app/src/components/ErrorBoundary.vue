<script setup lang="ts">
import { ref, onErrorCaptured } from "vue";

const error = ref<Error | null>(null);
const errorInfo = ref<string>("");

const resetError = () => {
  error.value = null;
  errorInfo.value = "";
};

onErrorCaptured((err: Error, instance, info: string) => {
  error.value = err;
  errorInfo.value = info;
  console.error("Error captured by ErrorBoundary:", err, info);
  return false;
});
</script>

<template>
  <div v-if="error" class="min-h-screen flex items-center justify-center bg-gray-50 p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-lg p-8 text-center">
      <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-red-100 flex items-center justify-center">
        <span class="text-3xl">⚠️</span>
      </div>
      <h2 class="text-xl font-bold text-gray-800 mb-2">Something went wrong</h2>
      <p class="text-gray-600 mb-4">An unexpected error occurred. Please try again.</p>
      <div v-if="errorInfo" class="mb-4 p-3 bg-gray-100 rounded-lg text-left">
        <p class="text-xs text-gray-500 font-mono break-all">
          {{ error?.message }}
        </p>
      </div>
      <button
        @click="resetError"
        class="px-6 py-3 bg-brand-teal text-white font-bold rounded-xl hover:opacity-90 transition-opacity"
      >
        Try Again
      </button>
    </div>
  </div>
  <slot v-else />
</template>
