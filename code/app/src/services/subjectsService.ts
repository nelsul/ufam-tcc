import { httpClient } from "./httpClient";
import type { PagedResult, SubjectDto, CreateSubjectDto, UpdateSubjectDto } from "@/types/api";

export interface SubjectFilters extends Record<
  string,
  string | number | boolean | null | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const subjectsService = {
  getActive: (params?: SubjectFilters) => {
    return httpClient.get<SubjectDto[]>("/subjects/active", params);
  },
  getAll: (params?: SubjectFilters) => {
    return httpClient.get<PagedResult<SubjectDto>>("/subjects", params);
  },
  create: (subject: CreateSubjectDto) => {
    return httpClient.post<SubjectDto>("/subjects", subject);
  },
  update: (id: string, subject: UpdateSubjectDto) => {
    return httpClient.put<void>(`/subjects/${id}`, subject);
  },
};
