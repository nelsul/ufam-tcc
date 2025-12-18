import { store } from "../store";

const BASE_URL = 
  (typeof window !== 'undefined' && (window as any).__ENV__?.VITE_API_BASE_URL) ||
  import.meta.env.VITE_API_BASE_URL || 
  "http://localhost:5133/api";

const getHeaders = (contentType: string | null = "application/json"): HeadersInit => {
  const headers: Record<string, string> = {};
  if (contentType) {
    headers["Content-Type"] = contentType;
  }
  if (store.token) {
    headers["Authorization"] = `Bearer ${store.token}`;
  }
  return headers;
};

export interface ApiErrorResponse {
  ErrorCode?: string;
  Message?: string;
}

export interface ApiError extends Error {
  status?: number;
  data?: ApiErrorResponse;
}

const handleResponse = async <T>(response: Response): Promise<T | null> => {
  if (!response.ok) {
    const errorData: ApiErrorResponse = await response.json().catch(() => ({}));
    const error = new Error(errorData.Message || "API Error") as ApiError;
    error.status = response.status;
    error.data = errorData;
    throw error;
  }
  const contentType = response.headers.get("content-type");
  if (contentType && contentType.includes("application/json")) {
    return response.json();
  }
  return null;
};

export const httpClient = {
  get: async <T = unknown>(
    endpoint: string,
    params: Record<string, string | number | boolean | undefined | null> = {}
  ): Promise<T> => {
    const url = new URL(`${BASE_URL}${endpoint}`);
    Object.keys(params).forEach((key) => {
      if (params[key] !== undefined && params[key] !== null) {
        url.searchParams.append(key, String(params[key]));
      }
    });
    const response = await fetch(url.toString(), {
      method: "GET",
      headers: getHeaders(null),
    });
    return handleResponse<T>(response) as Promise<T>;
  },

  post: async <T = unknown, B = unknown>(endpoint: string, body: B): Promise<T> => {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: "POST",
      headers: getHeaders(),
      body: JSON.stringify(body),
    });
    return handleResponse<T>(response) as Promise<T>;
  },

  put: async <T = unknown, B = unknown>(endpoint: string, body: B): Promise<T> => {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: "PUT",
      headers: getHeaders(),
      body: JSON.stringify(body),
    });
    return handleResponse<T>(response) as Promise<T>;
  },

  delete: async <T = unknown>(endpoint: string): Promise<T> => {
    const response = await fetch(`${BASE_URL}${endpoint}`, {
      method: "DELETE",
      headers: getHeaders(null),
    });
    return handleResponse<T>(response) as Promise<T>;
  },
};
