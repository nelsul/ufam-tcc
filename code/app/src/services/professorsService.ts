import { httpClient } from "./httpClient";
import type {
  PagedResult,
  ProfessorDto,
  CreateProfessorDto,
  UpdateProfessorDto,
  ResetPasswordDto,
} from "@/types/api";

export interface ProfessorFilters extends Record<
  string,
  string | number | boolean | null | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const professorsService = {
  getAll: (params?: ProfessorFilters) => {
    return httpClient.get<PagedResult<ProfessorDto>>("/professors", params);
  },
  create: (professor: CreateProfessorDto) => {
    return httpClient.post<ProfessorDto>("/professors", professor);
  },
  update: (id: string, professor: UpdateProfessorDto) => {
    return httpClient.put<void>(`/professors/${id}`, professor);
  },
  delete: (id: string) => {
    return httpClient.delete<void>(`/professors/${id}`);
  },
  resetPassword: (id: string, data: ResetPasswordDto) => {
    return httpClient.put<void>(`/professors/${id}/reset-password`, data);
  },
};
