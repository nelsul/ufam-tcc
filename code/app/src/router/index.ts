import {
  createRouter,
  createWebHistory,
  type RouteLocationNormalized,
  type NavigationGuardNext,
} from "vue-router";
import ScheduleSessionView from "../views/ScheduleSessionView.vue";
import LoginView from "../views/LoginView.vue";
import DashboardLayout from "../views/dashboard/DashboardLayout.vue";
import AvailabilityView from "../views/dashboard/AvailabilityView.vue";
import AppointmentsView from "../views/dashboard/AppointmentsView.vue";
import SessionsView from "../views/dashboard/SessionsView.vue";
import SessionTypesView from "../views/dashboard/SessionTypesView.vue";
import SemestersView from "../views/dashboard/SemestersView.vue";
import SubjectsView from "../views/dashboard/SubjectsView.vue";
import StudentsView from "../views/dashboard/StudentsView.vue";
import ProfessorsView from "../views/dashboard/ProfessorsView.vue";
import SubjectOfferingsView from "../views/dashboard/SubjectOfferingsView.vue";
import ObservationsView from "../views/dashboard/ObservationsView.vue";
import PatientRecordEditorView from "../views/dashboard/PatientRecordEditorView.vue";
import PhoneLayout from "../layouts/PhoneLayout.vue";
import ProfessorDashboardLayout from "../views/professor/ProfessorDashboardLayout.vue";
import ProfessorSubjectsView from "../views/professor/ProfessorSubjectsView.vue";
import ProfessorOfferingsView from "../views/professor/ProfessorOfferingsView.vue";
import ProfessorStudentsView from "../views/professor/ProfessorStudentsView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      component: PhoneLayout,
      children: [
        {
          path: "",
          name: "home",
          component: ScheduleSessionView,
        },
        {
          path: "login",
          name: "login",
          component: LoginView,
        },
      ],
    },
    {
      path: "/dashboard",
      component: DashboardLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: "",
          redirect: "/dashboard/appointments",
        },
        {
          path: "availability",
          name: "dashboard-availability",
          component: AvailabilityView,
        },
        {
          path: "appointments",
          name: "dashboard-appointments",
          component: AppointmentsView,
        },
        {
          path: "sessions",
          name: "dashboard-sessions",
          component: SessionsView,
        },
        {
          path: "session-types",
          name: "dashboard-session-types",
          component: SessionTypesView,
        },
        {
          path: "semesters",
          name: "dashboard-semesters",
          component: SemestersView,
        },
        {
          path: "subjects",
          name: "dashboard-subjects",
          component: SubjectsView,
        },
        {
          path: "professors",
          name: "dashboard-professors",
          component: ProfessorsView,
        },
        {
          path: "students",
          name: "dashboard-students",
          component: StudentsView,
        },
        {
          path: "subject-offerings",
          name: "dashboard-subject-offerings",
          component: SubjectOfferingsView,
        },
        {
          path: "observations",
          name: "dashboard-observations",
          component: ObservationsView,
        },
        {
          path: "patient-record/:recordId",
          name: "dashboard-patient-record",
          component: PatientRecordEditorView,
        },
      ],
    },
    {
      path: "/professor",
      component: ProfessorDashboardLayout,
      meta: { requiresAuth: true, role: "professor" },
      children: [
        {
          path: "",
          redirect: "/professor/offerings",
        },
        {
          path: "subjects",
          name: "professor-subjects",
          component: ProfessorSubjectsView,
        },
        {
          path: "offerings",
          name: "professor-offerings",
          component: ProfessorOfferingsView,
        },
        {
          path: "students",
          name: "professor-students",
          component: ProfessorStudentsView,
        },
      ],
    },
  ],
});

const getUserFromCookie = () => {
  const cookie = document.cookie.split("; ").find((row) => row.startsWith("userData="));
  if (cookie) {
    try {
      const [, value] = cookie.split("=");
      if (value) {
        return JSON.parse(decodeURIComponent(value));
      }
    } catch {
      return null;
    }
  }
  return null;
};

router.beforeEach(
  (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext) => {
    const token = document.cookie.split("; ").find((row) => row.startsWith("authToken="));
    const isAuthenticated = !!token;
    const user = getUserFromCookie();

    if (to.meta.requiresAuth && !isAuthenticated) {
      next({ name: "login" });
    } else if (to.name === "login" && isAuthenticated) {
      if (user?.role?.toLowerCase() === "professor") {
        next({ path: "/professor" });
      } else {
        next({ path: "/dashboard" });
      }
    } else if (to.meta.role === "professor" && user?.role?.toLowerCase() !== "professor") {
      next({ path: "/dashboard" });
    } else if (to.path.startsWith("/dashboard") && user?.role?.toLowerCase() === "professor") {
      next({ path: "/professor" });
    } else {
      next();
    }
  }
);

export default router;
