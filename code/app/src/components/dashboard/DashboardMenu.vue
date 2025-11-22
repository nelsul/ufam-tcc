<script setup lang="ts">
import {
  Calendar,
  Clock,
  LogOut,
  List,
  BookOpen,
  Book,
  GraduationCap,
  Layers,
  Eye,
  Users,
  Play,
} from "lucide-vue-next";
import { useRouter, useRoute } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { store } from "../../store";
import LanguageSwitcher from "../LanguageSwitcher.vue";

const router = useRouter();
const route = useRoute();
const { t } = useI18n();

const userName = computed(
  () => store.user?.name + " " + store.user?.fullName?.split(" ").slice(-1)[0] || "Dr. Nook"
);
const userRole = computed(() => {
  const role = store.user?.role?.toLowerCase() || "professional";
  return t(`roles.${role}`);
});

const menuItems = computed(() => [
  {
    name: "availability",
    label: t("dashboard.menu.availability"),
    icon: Calendar,
    path: "/dashboard/availability",
    showOnMobile: true,
  },
  {
    name: "appointments",
    label: t("dashboard.menu.appointments"),
    icon: Clock,
    path: "/dashboard/appointments",
    showOnMobile: true,
  },
  {
    name: "sessions",
    label: t("dashboard.menu.sessions"),
    icon: Play,
    path: "/dashboard/sessions",
    showOnMobile: true,
  },
  {
    name: "session-types",
    label: t("dashboard.menu.session_types"),
    icon: List,
    path: "/dashboard/session-types",
    showOnMobile: false,
  },
  {
    name: "observations",
    label: t("dashboard.menu.observations"),
    icon: Eye,
    path: "/dashboard/observations",
    showOnMobile: true,
  },
  {
    name: "semesters",
    label: t("dashboard.menu.semesters"),
    icon: BookOpen,
    path: "/dashboard/semesters",
    showOnMobile: true,
  },
  {
    name: "subjects",
    label: t("dashboard.menu.subjects"),
    icon: Book,
    path: "/dashboard/subjects",
    showOnMobile: false,
  },
  {
    name: "professors",
    label: t("dashboard.menu.professors"),
    icon: GraduationCap,
    path: "/dashboard/professors",
    showOnMobile: false,
  },
  {
    name: "subject-offerings",
    label: t("dashboard.menu.offerings"),
    icon: Layers,
    path: "/dashboard/subject-offerings",
    showOnMobile: false,
  },
  {
    name: "students",
    label: t("dashboard.menu.students"),
    icon: Users,
    path: "/dashboard/students",
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

  <nav
    class="md:hidden fixed bottom-6 left-4 right-4 z-[100] rounded-full h-16 shadow-2xl flex items-center justify-around px-2 border-4 border-white"
  >
    <router-link
      v-for="item in mobileMenuItems"
      :key="item.name"
      :to="item.path"
      class="flex items-center justify-center p-3 rounded-full transition-all duration-200"
      :class="[
        isActive(item.path)
          ? 'bg-[#81F2DD] text-[#5E5340] scale-110'
          : 'text-[#E9E7D8] hover:text-white hover:scale-105',
      ]"
    >
      <component :is="item.icon" class="w-5 h-5" />
    </router-link>

    <button
      @click="handleLogout"
      class="flex items-center justify-center p-3 rounded-full text-[#F6A95B] hover:text-[#ff9f4d] hover:scale-105 transition-all duration-200"
    >
      <LogOut class="w-5 h-5" />
    </button>
  </nav>
</template>
