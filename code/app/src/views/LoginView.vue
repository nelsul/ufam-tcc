<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { IdCard, Loader2, AlertCircle } from "lucide-vue-next";
import { store } from "../store";
import { useI18n } from "vue-i18n";
import { authService } from "../services/authService";

const { t } = useI18n();
const router = useRouter();

const email = ref("");
const password = ref("");
const loading = ref(false);
const error = ref<string | null>(null);

const handleLogin = async () => {
  loading.value = true;
  error.value = null;

  try {
    const data = await authService.login({
      email: email.value,
      password: password.value,
    });

    const token = data.token; // Assuming the API returns { token: "..." }

    document.cookie = `authToken=${token}; path=/; max-age=86400; Secure; SameSite=Strict`;
    store.setToken(token);

    if (data.user) {
      store.setUser(data.user);
      document.cookie = `userData=${encodeURIComponent(
        JSON.stringify(data.user)
      )}; path=/; max-age=86400; Secure; SameSite=Strict`;
    }

    router.push("/dashboard");
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : t("login_failed");
    error.value = errorMessage;
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="flex flex-col h-full">
    <div class="text-center mb-8 mt-4">
      <div
        class="w-20 h-20 bg-brand-yellow/30 rounded-full flex items-center justify-center mx-auto mb-4 border-4 border-white shadow-sm"
      >
        <IdCard class="w-10 h-10 text-primary-text" />
      </div>
      <h1 class="text-2xl font-bold text-primary-text mb-1">
        {{ $t("professional_access") }}
      </h1>
      <p class="text-sm text-primary-text/70 font-medium">
        {{ $t("professionals_professors_only") }}
      </p>
    </div>

    <form @submit.prevent="handleLogin" class="flex flex-col gap-5 px-4">
      <div
        v-if="error"
        class="bg-red-100 text-red-800 rounded-xl p-3 flex items-center gap-3 text-sm font-bold border-2 border-red-200"
      >
        <AlertCircle class="w-5 h-5 shrink-0" />
        <span>{{ error }}</span>
      </div>

      <div class="space-y-1">
        <label class="text-sm font-bold text-primary-text ml-2">{{ $t("email") }}</label>
        <input
          v-model="email"
          type="email"
          required
          class="w-full bg-[#FDFDF5] rounded-2xl border-2 border-transparent focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors shadow-inner"
          placeholder="professional@icomp.ufam.edu.br"
        />
      </div>

      <div class="space-y-1">
        <label class="text-sm font-bold text-primary-text ml-2">{{ $t("password") }}</label>
        <input
          v-model="password"
          type="password"
          required
          class="w-full bg-[#FDFDF5] rounded-2xl border-2 border-transparent focus:border-brand-teal px-4 py-3 text-primary-text outline-none transition-colors shadow-inner"
          placeholder="••••••••"
        />
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="mt-4 w-full rounded-full bg-brand-green text-white px-6 py-3.5 font-bold shadow-md transition-all duration-200 hover:scale-[1.02] active:scale-95 cursor-pointer border-4 border-white text-lg flex items-center justify-center gap-2 hover:animate-wiggle disabled:opacity-70 disabled:cursor-not-allowed"
      >
        <Loader2 v-if="loading" class="animate-spin w-5 h-5" />
        <span v-else>{{ $t("enter") }}</span>
      </button>
    </form>

    <div class="mt-auto mb-6 text-center">
      <router-link
        to="/"
        class="text-sm font-bold text-primary-text/50 hover:text-brand-teal transition-colors"
      >
        ← {{ $t("back_to_scheduling") }}
      </router-link>
    </div>
  </div>
</template>
