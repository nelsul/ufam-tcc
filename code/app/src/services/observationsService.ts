import { httpClient } from "./httpClient";
import type { PagedResult, ObservationDto, UpdateObservationDto } from "@/types/api";

interface CreateObservationDto {
  name: string;
  description?: string;
}

export interface ObservationFilters extends Record<string, string | number | boolean | undefined> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const observationsService = {
  getAll: (params?: ObservationFilters) => {
    return httpClient.get<PagedResult<ObservationDto>>("/observations", params);
  },
  create: (observation: CreateObservationDto) => {
    return httpClient.post<ObservationDto>("/observations", observation);
  },
  update: (id: string, observation: UpdateObservationDto) => {
    return httpClient.put<ObservationDto>(`/observations/${id}`, observation);
  },
  delete: (id: string) => {
    return httpClient.delete<void>(`/observations/${id}`);
  },
};
