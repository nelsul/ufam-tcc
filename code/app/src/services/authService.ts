import { httpClient } from "./httpClient";
import type { LoginCredentials, LoginResponse } from "@/types/auth";

export const authService = {
  login: (credentials: LoginCredentials): Promise<LoginResponse> => {
    return httpClient.post<LoginResponse, LoginCredentials>("/auth/login", credentials);
  },
};
