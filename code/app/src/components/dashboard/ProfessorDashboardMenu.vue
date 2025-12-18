<script setup lang="ts">
import { Book, Layers, Users, LogOut, Menu, X } from "lucide-vue-next";
import { useRouter, useRoute } from "vue-router";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { store } from "../../store";
import LanguageSwitcher from "../LanguageSwitcher.vue";

const mobileMenuOpen = ref(false);

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value;
};

const closeMobileMenu = () => {
  mobileMenuOpen.value = false;
};

const router = useRouter();
const route = useRoute();
const { t } = useI18n();

const userName = computed(
  () => store.user?.name + " " + (store.user?.fullName?.split(" ").slice(-1)[0] || "")
);
const userRole = computed(() => {
  const role = store.user?.role?.toLowerCase() || "professor";
  return t(`roles.${role}`);
});

const menuItems = computed(() => [
  {
    name: "subjects",
    label: t("professor_dashboard.menu.subjects"),
    icon: Book,
    path: "/professor/subjects",
    showOnMobile: true,
  },
  {
    name: "my-offerings",
    label: t("professor_dashboard.menu.my_offerings"),
    icon: Layers,
    path: "/professor/offerings",
    showOnMobile: true,
  },
  {
    name: "my-students",
    label: t("professor_dashboard.menu.my_students"),
    icon: Users,
    path: "/professor/students",
    showOnMobile: true,
  },
]);

const mobileMenuItems = computed(() => menuItems.value.filter((item) => item.showOnMobile));

const isActive = (path: string) => route.path.includes(path);

const handleLogout = () => {
  document.cookie = "authToken=; path=/; max-age=0; Secure; SameSite=Strict";
  document.cookie = "userData=; path=/; max-age=0; Secure; SameSite=Strict";
  store.setUser(null);
  router.push("/login");
};
</script>

<template>
  <aside
    class="hidden md:flex flex-col w-64 bg-brand-teal/20 border-r-4 border-white h-full p-6 rounded-l-[40px] gap-5"
  >
    <div class="mb-8 flex items-center gap-3">
      <div class="w-10 h-10 bg-brand-orange rounded-full border-2 border-white"></div>
      <div class="flex flex-col">
        <span class="font-bold text-primary-text text-lg leading-tight">{{ userName }}</span>
        <span class="text-xs text-primary-text/60 font-medium">{{ userRole }}</span>
      </div>
    </div>

    <nav class="flex-1 flex flex-col gap-2">
      <router-link
        v-for="item in menuItems"
        :key="item.name"
        :to="item.path"
        class="flex items-center gap-2 px-3 py-2 rounded-xl font-semibold text-sm transition-all duration-200 border-2"
        :class="[
          isActive(item.path)
            ? 'bg-brand-teal text-primary-text border-white shadow-sm'
            : 'bg-transparent text-primary-text/60 border-transparent hover:bg-white/30',
        ]"
      >
        <component :is="item.icon" class="w-5 h-5" />
        {{ item.label }}
      </router-link>
    </nav>

    <div class="mb-4">
      <LanguageSwitcher />
    </div>

    <button
      @click="handleLogout"
      class="flex items-center gap-2 px-3 py-2 rounded-xl font-semibold text-sm text-red-500 hover:bg-red-100 transition-colors mt-auto"
    >
      <LogOut class="w-5 h-5" />
      {{ t("common.logout") }}
    </button>
  </aside>

  <!-- Mobile Hamburger Button -->
  <button
    @click="toggleMobileMenu"
    class="md:hidden fixed top-4 left-4 z-[110] p-3 rounded-full bg-brand-teal text-white shadow-lg border-2 border-white"
  >
    <Menu v-if="!mobileMenuOpen" class="w-6 h-6" />
    <X v-else class="w-6 h-6" />
  </button>

  <!-- Mobile Overlay -->
  <div
    v-if="mobileMenuOpen"
    @click="closeMobileMenu"
    class="md:hidden fixed inset-0 bg-black/50 z-[105] transition-opacity"
  ></div>

  <!-- Mobile Slide-out Menu -->
  <aside
    :class="[
      'md:hidden fixed top-0 left-0 h-full w-64 bg-[#81C6BC] z-[110] transform transition-transform duration-300 ease-in-out flex flex-col p-6 gap-5 shadow-2xl',
      mobileMenuOpen ? 'translate-x-0' : '-translate-x-full',
    ]"
  >
    <div class="mt-12 mb-4 flex items-center gap-3">
      <div class="w-10 h-10 bg-brand-orange rounded-full border-2 border-white"></div>
      <div class="flex flex-col">
        <span class="font-bold text-primary-text text-lg leading-tight">{{ userName }}</span>
        <span class="text-xs text-primary-text/70 font-medium">{{ userRole }}</span>
      </div>
    </div>

    <nav class="flex-1 flex flex-col gap-2 overflow-y-auto">
      <router-link
        v-for="item in menuItems"
        :key="item.name"
        :to="item.path"
        @click="closeMobileMenu"
        class="flex items-center gap-2 px-3 py-3 rounded-xl font-semibold text-sm transition-all duration-200 border-2"
        :class="[
          isActive(item.path)
            ? 'bg-white text-brand-teal border-white shadow-sm'
            : 'bg-transparent text-primary-text/80 border-transparent hover:bg-white/30 hover:text-primary-text',
        ]"
      >
        <component :is="item.icon" class="w-5 h-5" />
        {{ item.label }}
      </router-link>
    </nav>

    <div class="mb-4">
      <LanguageSwitcher />
    </div>

    <button
      @click="handleLogout"
      class="flex items-center gap-2 px-3 py-3 rounded-xl font-semibold text-sm text-white bg-brand-orange hover:bg-brand-orange/80 transition-colors border-2 border-white/50"
    >
      <LogOut class="w-5 h-5" />
      {{ t("common.logout") }}
    </button>
  </aside>
</template>
