import { httpClient } from "./httpClient";
import type { PagedResult, SubjectOfferingDto } from "@/types/api";

export interface ProfessorOfferingFilters extends Record<
  string,
  string | number | boolean | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export interface CreateProfessorOfferingDto {
  semesterId: string;
  subjectId: string;
  professorId: string;
}

export interface UpdateProfessorOfferingDto {
  semesterId: string;
  subjectId: string;
  professorId: string;
  status: number;
}

export const professorOfferingsService = {
  getMyOfferings: (params?: ProfessorOfferingFilters) => {
    return httpClient.get<PagedResult<SubjectOfferingDto>>("/SubjectOfferings/my", params);
  },
  create: (offering: CreateProfessorOfferingDto) => {
    return httpClient.post<SubjectOfferingDto>("/SubjectOfferings", offering);
  },
  update: (id: string, data: UpdateProfessorOfferingDto) => {
    return httpClient.put<void>(`/SubjectOfferings/${id}`, data);
  },
  deactivate: (id: string) => {
    return httpClient.delete<void>(`/SubjectOfferings/${id}`);
  },
};
