<script setup lang="ts">
import { X, Eye, Search } from "lucide-vue-next";
import type { ObservationWithId } from "@/types/api";

defineProps<{
  show: boolean;
  observation: ObservationWithId | null;
  note: string;
  isEditing: boolean;
  searchQuery: string;
  availableObservations: ObservationWithId[];
}>();

const _emit = defineEmits<{
  (e: "close"): void;
  (e: "save"): void;
  (e: "update:note", value: string): void;
  (e: "update:searchQuery", value: string): void;
  (e: "search"): void;
  (e: "select-observation", obs: ObservationWithId): void;
}>();
</script>

<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-[70] p-4 backdrop-blur-sm"
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
          class="w-8 h-8 bg-brand-orange rounded-full flex items-center justify-center text-white text-sm"
        >
          <Eye class="w-5 h-5" />
        </span>
        {{ isEditing ? "Edit Observation" : "Add Observation" }}
      </h2>

      <div v-if="!isEditing && !observation" class="mb-4">
        <label class="block text-sm font-bold text-primary-text mb-1 ml-2">Observation</label>
        <div class="relative">
          <input
            :value="searchQuery"
            @input="
              $emit('update:searchQuery', $event.target.value);
              $emit('search');
            "
            type="text"
            placeholder="Search or add observation..."
            class="w-full bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-orange pl-10 pr-4 py-3 text-primary-text outline-none transition-colors"
          />
          <div class="absolute left-3 top-1/2 -translate-y-1/2">
            <Search class="w-4 h-4 text-primary-text/40" />
          </div>
        </div>

        <div
          v-if="availableObservations && availableObservations.length > 0"
          class="max-h-60 overflow-y-auto mt-2"
        >
          <div
            v-for="obs in availableObservations"
            :key="obs.id"
            class="p-3 bg-white rounded-xl shadow-sm mb-2 flex justify-between items-center hover:bg-soft-gray transition-colors cursor-pointer"
            @click="$emit('select-observation', obs)"
          >
            <div>
              <p class="font-bold text-primary-text">{{ obs.name }}</p>
              <p class="text-xs text-primary-text/60">
                {{ obs.description }}
              </p>
            </div>
            <span
              class="text-xs bg-brand-orange/10 text-brand-orange px-2 py-1 rounded-lg font-bold"
              >Observation</span
            >
          </div>
        </div>

        <div
          v-if="!availableObservations?.length"
          class="p-4 text-center text-primary-text/50 border-2 border-dashed border-soft-gray rounded-xl mt-4"
        >
          <p class="text-sm mb-2">Please search and select an observation to add.</p>
        </div>
      </div>

      <div v-else>
        <div class="mb-6">
          <p class="font-bold text-primary-text text-lg">
            {{ observation.name }}
          </p>
          <p class="text-sm text-primary-text/60">
            {{ observation.description }}
          </p>
        </div>

        <div class="mb-6">
          <label class="block text-sm font-bold text-primary-text mb-2 ml-2">Note</label>
          <textarea
            :value="note"
            @input="$emit('update:note', $event.target.value)"
            class="w-full h-32 bg-white rounded-2xl border-2 border-soft-gray focus:border-brand-orange p-4 text-primary-text outline-none transition-colors resize-none"
            placeholder="Enter your observation note here..."
          ></textarea>
        </div>

        <div class="flex gap-2">
          <button
            @click="$emit('close')"
            class="flex-1 bg-soft-gray text-primary-text rounded-xl px-4 py-3 font-bold hover:bg-soft-gray/80 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="$emit('save')"
            class="flex-1 bg-brand-orange text-white rounded-xl px-4 py-3 font-bold hover:scale-[1.02] active:scale-95 transition-all shadow-sm"
          >
            Save Observation
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
