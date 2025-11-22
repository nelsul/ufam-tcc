import { reactive } from "vue";

export interface User {
  publicId: string;
  name: string;
  email: string;
  role: string;
  fullName?: string;
  institutionalEmail?: string;
  registration?: string;
}

interface Store {
  user: User | null;
  token: string | null;
  setUser(user: User | null): void;
  setToken(token: string | null): void;
}

const getUserFromCookie = (): User | null => {
  const cookie = document.cookie.split("; ").find((row) => row.startsWith("userData="));
  if (cookie) {
    try {
      return JSON.parse(decodeURIComponent(cookie.split("=")[1]));
    } catch {
      return null;
    }
  }
  return null;
};

const getTokenFromCookie = (): string | null => {
  const cookie = document.cookie.split("; ").find((row) => row.startsWith("authToken="));
  if (cookie) {
    return cookie.split("=")[1];
  }
  return null;
};

export const store = reactive<Store>({
  user: getUserFromCookie(),
  token: getTokenFromCookie(),
  setUser(user: User | null) {
    this.user = user;
  },
  setToken(token: string | null) {
    this.token = token;
  },
});
