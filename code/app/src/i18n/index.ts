import { createI18n } from "vue-i18n";
import en from "./locales/en.json";
import ptBR from "./locales/pt-BR.json";
import es from "./locales/es.json";

const savedLocale = localStorage.getItem("locale") || "pt-BR";

const i18n = createI18n({
  legacy: false,
  locale: savedLocale,
  fallbackLocale: "en",
  messages: {
    en,
    "pt-BR": ptBR,
    es,
  },
});

export default i18n;
