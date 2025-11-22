import { httpClient } from "./httpClient";
import type { PagedResult, SemesterDto, CreateSemesterDto, UpdateSemesterDto } from "@/types/api";

export interface SemesterFilters extends Record<
  string,
  string | number | boolean | null | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const semestersService = {
  getAll: (params?: SemesterFilters) => {
    return httpClient.get<PagedResult<SemesterDto>>("/semesters", params);
  },
  create: (semester: CreateSemesterDto) => {
    return httpClient.post<SemesterDto>("/semesters", semester);
  },
  update: (id: string, semester: UpdateSemesterDto) => {
    return httpClient.put<void>(`/semesters/${id}`, semester);
  },
  delete: (id: string) => {
    return httpClient.delete<void>(`/semesters/${id}`);
  },
  getCurrent: () => {
    return httpClient.get<SemesterDto>("/semesters/current");
  },
};
