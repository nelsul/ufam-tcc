<script setup lang="ts">
import { X } from "lucide-vue-next";
import type { Component } from "vue";

defineProps<{
  show: boolean;
  title?: string;
  icon?: Component;
  iconBgColor?: string;
  maxWidth?: string;
}>();

defineEmits<{
  (e: "close"): void;
}>();
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="show"
        class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 backdrop-blur-sm"
        @click.self="$emit('close')"
      >
        <div
          class="bg-[#FDFDF5] rounded-[40px] p-8 w-full shadow-2xl border-4 border-white relative max-h-[90vh] overflow-y-auto"
          :class="maxWidth || 'max-w-md'"
        >
          <button
            @click="$emit('close')"
            class="absolute top-6 right-6 text-primary-text/50 hover:text-primary-text transition-colors z-10"
          >
            <X class="w-6 h-6" />
          </button>

          <div v-if="title || $slots.header" class="mb-6">
            <slot name="header">
              <h2 class="text-2xl font-bold text-primary-text flex items-center gap-2 pr-8">
                <span
                  v-if="icon"
                  class="w-8 h-8 rounded-full flex items-center justify-center text-white text-sm"
                  :class="iconBgColor || 'bg-brand-teal'"
                >
                  <component :is="icon" class="w-5 h-5" />
                </span>
                {{ title }}
              </h2>
            </slot>
          </div>

          <slot />

          <div v-if="$slots.footer" class="mt-6">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>
