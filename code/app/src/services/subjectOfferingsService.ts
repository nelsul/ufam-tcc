import { httpClient } from "./httpClient";
import type {
  PagedResult,
  SubjectOfferingDto,
  CreateSubjectOfferingDto,
  UpdateSubjectOfferingDto,
} from "@/types/api";

export interface SubjectOfferingFilters extends Record<
  string,
  string | number | boolean | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const subjectOfferingsService = {
  getAll: (params?: SubjectOfferingFilters) => {
    return httpClient.get<PagedResult<SubjectOfferingDto>>("/subjectofferings", params);
  },
  create: (offering: CreateSubjectOfferingDto) => {
    return httpClient.post<SubjectOfferingDto>("/subjectofferings", offering);
  },
  update: (id: string, offering: UpdateSubjectOfferingDto) => {
    return httpClient.put<SubjectOfferingDto>(`/subjectofferings/${id}`, offering);
  },
  delete: (id: string) => {
    return httpClient.delete<void>(`/subjectofferings/${id}`);
  },
};
