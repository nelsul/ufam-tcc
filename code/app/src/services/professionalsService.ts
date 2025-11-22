import { httpClient } from "./httpClient";

export interface ProfessionalDto {
  publicId: string;
  name: string;
  fullName: string;
  institutionalEmail: string;
}

export interface TimeRange {
  start: string;
  end: string;
}

export interface AvailabilityDay {
  date: string;
  timeRanges: TimeRange[];
}

export interface AppointmentDto {
  id: string;
  startTime: string;
  endTime: string | null;
  studentFullName: string;
  sessionTypeName: string | null;
  status: string;
}

export interface MyAvailabilityDto {
  publicId: string;
  startTime: string;
  endTime: string;
  status: string;
}

export interface CreateMyAvailabilityDto {
  startTime: string;
  endTime: string;
}

export interface UpdateMyAvailabilityDto {
  startTime: string;
  endTime: string;
  status: string;
}

export interface PagedAppointmentsResult {
  items: AppointmentDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AppointmentFilters {
  pageNumber?: number;
  pageSize?: number;
  status?: string;
  sessionTypeId?: string;
  search?: string;
}

export const professionalsService = {
  getAll: (): Promise<ProfessionalDto[]> => {
    return httpClient.get<ProfessionalDto[]>("/Professionals");
  },
  getAvailability: (id: string, date?: string): Promise<AvailabilityDay[]> => {
    return httpClient.get<AvailabilityDay[]>(`/Professionals/${id}/availability`, { date });
  },
  getMyAppointments: (filters: AppointmentFilters = {}): Promise<PagedAppointmentsResult> => {
    const { pageNumber = 1, pageSize = 20, status, sessionTypeId, search } = filters;
    return httpClient.get<PagedAppointmentsResult>("/Professionals/me/appointments", {
      pageNumber,
      pageSize,
      ...(status && { status }),
      ...(sessionTypeId && { sessionTypeId }),
      ...(search && { search }),
    });
  },
  getMyAvailabilities: (): Promise<MyAvailabilityDto[]> => {
    return httpClient.get<MyAvailabilityDto[]>("/Professionals/me/availabilities");
  },
  createMyAvailability: (dto: CreateMyAvailabilityDto): Promise<MyAvailabilityDto> => {
    return httpClient.post<MyAvailabilityDto>("/Professionals/me/availabilities", dto);
  },
  updateMyAvailability: (id: string, dto: UpdateMyAvailabilityDto): Promise<void> => {
    return httpClient.put(`/Professionals/me/availabilities/${id}`, dto);
  },
  deleteMyAvailability: (id: string): Promise<void> => {
    return httpClient.delete(`/Professionals/me/availabilities/${id}`);
  },
};
