import { httpClient } from "./httpClient";
import type { PagedResult, PaginationParams } from "@/types/api";

export interface SessionDto {
  id: string;
  appointmentId: string;
  professionalId: string;
  professionalName: string;
  studentId: string;
  studentName: string;
  startedAt: string;
  endedAt: string | null;
  notes: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSessionDto {
  appointmentId: string;
  professionalId: string;
  studentId: string;
  startedAt: string;
  notes?: string;
}

export interface UpdateSessionDto {
  endedAt?: string;
  notes?: string;
  status?: string;
}

export interface SessionFilters extends PaginationParams {
  dateFrom?: string;
  dateTo?: string;
  search?: string;
}

export const sessionsService = {
  getAll: (params?: PaginationParams): Promise<PagedResult<SessionDto>> => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber) queryParams.append("pageNumber", params.pageNumber.toString());
    if (params?.pageSize) queryParams.append("pageSize", params.pageSize.toString());
    const queryString = queryParams.toString();
    return httpClient.get<PagedResult<SessionDto>>(
      `/sessions${queryString ? `?${queryString}` : ""}`
    );
  },

  getMySessions: (params?: SessionFilters): Promise<PagedResult<SessionDto>> => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber) queryParams.append("pageNumber", params.pageNumber.toString());
    if (params?.pageSize) queryParams.append("pageSize", params.pageSize.toString());
    if (params?.dateFrom) queryParams.append("dateFrom", params.dateFrom);
    if (params?.dateTo) queryParams.append("dateTo", params.dateTo);
    if (params?.search) queryParams.append("search", params.search);
    const queryString = queryParams.toString();
    return httpClient.get<PagedResult<SessionDto>>(
      `/sessions/my${queryString ? `?${queryString}` : ""}`
    );
  },

  getMyOpenSession: (): Promise<SessionDto | null> => {
    return httpClient.get<SessionDto | null>("/sessions/my/open");
  },

  getById: (id: string): Promise<SessionDto> => {
    return httpClient.get<SessionDto>(`/sessions/${id}`);
  },

  create: (data: CreateSessionDto): Promise<SessionDto> => {
    return httpClient.post<SessionDto, CreateSessionDto>("/sessions", data);
  },

  startSession: (appointmentId: string): Promise<SessionDto> => {
    return httpClient.post<SessionDto, object>(`/sessions/start/${appointmentId}`, {});
  },

  update: (id: string, data: UpdateSessionDto): Promise<SessionDto> => {
    return httpClient.put<SessionDto>(`/sessions/${id}`, data);
  },

  delete: (id: string): Promise<void> => {
    return httpClient.delete(`/sessions/${id}`);
  },

  endSession: (id: string, notes?: string): Promise<SessionDto> => {
    return httpClient.put<SessionDto>(`/sessions/${id}`, {
      endedAt: new Date().toISOString(),
      status: "Completed",
      notes,
    });
  },
};
