import { useToast } from "vue-toastification";
import { useI18n } from "vue-i18n";

export interface ApiErrorResponse {
  ErrorCode?: string;
  Message?: string;
}

export interface ApiError extends Error {
  status?: number;
  data?: ApiErrorResponse;
}

/**
 * Composable for handling API responses with localized toast notifications.
 * Automatically translates error codes to user-friendly messages.
 */
export function useApiToast() {
  const toast = useToast();
  const { t, te } = useI18n();

  /**
   * Get the translation key for an error code
   */
  const getErrorTranslationKey = (errorCode: string): string => {
    const parts = errorCode.split(".");
    if (parts.length === 2 && parts[0] && parts[1]) {
      const domain = parts[0];
      const code = parts[1];
      const snakeCase = code
        .replace(/([A-Z])/g, "_$1")
        .toLowerCase()
        .replace(/^_/, "");
      return `errors.${domain.toLowerCase()}.${snakeCase}`;
    }
    return `errors.${errorCode.toLowerCase().replace(/\./g, "_")}`;
  };

  /**
   * Get localized error message from API error
   */
  const getErrorMessage = (error: unknown): string => {
    if (error instanceof Error) {
      const apiError = error as ApiError;
      const errorCode = apiError.data?.ErrorCode;

      if (errorCode) {
        const translationKey = getErrorTranslationKey(errorCode);
        if (te(translationKey)) {
          return t(translationKey);
        }
      }

      if (apiError.data?.Message) {
        return apiError.data.Message;
      }

      return apiError.message;
    }
    return t("errors.unknown");
  };

  /**
   * Get the error code from an API error
   */
  const getErrorCode = (error: unknown): string | undefined => {
    if (error instanceof Error) {
      const apiError = error as ApiError;
      return apiError.data?.ErrorCode;
    }
    return undefined;
  };

  /**
   * Show a success toast with a localized message
   */
  const showSuccess = (messageKey: string, params?: Record<string, unknown>) => {
    toast.success(t(messageKey, params ?? {}));
  };

  /**
   * Show an error toast with a localized message from an API error
   */
  const showError = (error: unknown, fallbackKey?: string) => {
    const message = getErrorMessage(error);

    if (fallbackKey && message === (error instanceof Error ? error.message : "")) {
      toast.error(t(fallbackKey));
    } else {
      toast.error(message);
    }
  };

  /**
   * Show a generic error toast with a localized message
   */
  const showErrorMessage = (messageKey: string, params?: Record<string, unknown>) => {
    toast.error(t(messageKey, params ?? {}));
  };

  /**
   * Show an info toast
   */
  const showInfo = (messageKey: string, params?: Record<string, unknown>) => {
    toast.info(t(messageKey, params ?? {}));
  };

  /**
   * Show a warning toast
   */
  const showWarning = (messageKey: string, params?: Record<string, unknown>) => {
    toast.warning(t(messageKey, params ?? {}));
  };

  /**
   * Execute an API call with automatic success/error toast handling
   */
  const withToast = async <T>(
    apiCall: () => Promise<T>,
    options: {
      successKey?: string;
      errorKey?: string;
      successParams?: Record<string, unknown>;
    } = {}
  ): Promise<T | null> => {
    try {
      const result = await apiCall();
      if (options.successKey) {
        showSuccess(options.successKey, options.successParams);
      }
      return result;
    } catch (error) {
      showError(error, options.errorKey);
      return null;
    }
  };

  return {
    showSuccess,
    showError,
    showErrorMessage,
    showInfo,
    showWarning,
    withToast,
    getErrorMessage,
    getErrorCode,
  };
}
