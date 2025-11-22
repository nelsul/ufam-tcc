import "./assets/main.css";
import "vue-toastification/dist/index.css";
import "./assets/toast.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import i18n from "./i18n";
import Toast from "vue-toastification";

const app = createApp(App);

app.use(router);
app.use(i18n);
app.use(Toast, {
  position: "top-right",
  timeout: 3000,
  closeOnClick: true,
  pauseOnFocusLoss: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  showCloseButtonOnHover: false,
  hideProgressBar: false,
  closeButton: "button",
  icon: true,
  rtl: false,
});
app.mount("#app");
