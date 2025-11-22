import { httpClient } from "./httpClient";

export interface CreateAppointmentDto {
  professionalId: string;
  studentEmail: string;
  studentRegistration: string;
  studentFullName: string;
  startTime: string;
  reasonForVisit: string;
}

export interface UpdateAppointmentDto {
  studentId?: string | null;
  sessionTypeId?: string | null;
  startTime?: string;
  endTime?: string | null;
  reasonForVisit?: string;
  status?: string;
}

export interface AppointmentDto {
  id: string;
  professionalId: string;
  professionalName: string;
  studentId: string | null;
  studentEmail: string;
  studentRegistration: string;
  studentFullName: string;
  sessionTypeId: string | null;
  sessionTypeName: string | null;
  startTime: string;
  endTime: string | null;
  status: string;
  reasonForVisit: string;
  createdAt: string;
  updatedAt: string;
}

export const appointmentsService = {
  create: (appointment: CreateAppointmentDto): Promise<AppointmentDto> => {
    return httpClient.post<AppointmentDto, CreateAppointmentDto>("/appointments", appointment);
  },
  getById: (id: string): Promise<AppointmentDto> => {
    return httpClient.get<AppointmentDto>(`/appointments/${id}`);
  },
  update: (id: string, data: UpdateAppointmentDto): Promise<AppointmentDto> => {
    return httpClient.put<AppointmentDto>(`/appointments/${id}`, data);
  },
};
