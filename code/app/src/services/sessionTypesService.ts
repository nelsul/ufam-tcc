import { httpClient } from "./httpClient";
import type {
  PagedResult,
  SessionTypeDto,
  CreateSessionTypeDto,
  UpdateSessionTypeDto,
} from "@/types/api";

export interface SessionTypeFilters extends Record<
  string,
  string | number | boolean | null | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

export const sessionTypesService = {
  getAll: (params?: SessionTypeFilters) => {
    return httpClient.get<PagedResult<SessionTypeDto>>("/SessionTypes", params);
  },
  create: (sessionType: CreateSessionTypeDto) => {
    return httpClient.post<SessionTypeDto>("/SessionTypes", sessionType);
  },
  update: (id: string, sessionType: UpdateSessionTypeDto) => {
    return httpClient.put<void>(`/SessionTypes/${id}`, sessionType);
  },
};
