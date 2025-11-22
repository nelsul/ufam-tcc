import { httpClient } from "./httpClient";
import type {
  PagedResult,
  StudentDto,
  CreateStudentDto,
  UpdateStudentDto,
  PatientRecordDto,
  PatientObservationDto,
  EnrollmentDto,
  ResetPasswordDto,
} from "@/types/api";

export interface StudentFilters extends Record<
  string,
  string | number | boolean | null | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  includeInactive?: boolean;
  search?: string;
}

interface EnrollmentData {
  subjectOfferingId: string;
}

export const studentsService = {
  getAll: (params?: StudentFilters) => {
    return httpClient.get<PagedResult<StudentDto>>("/students", params);
  },
  getById: (id: string) => {
    return httpClient.get<StudentDto>(`/students/${id}`);
  },
  create: (student: CreateStudentDto) => {
    return httpClient.post<StudentDto>("/students", student);
  },
  update: (id: string, student: UpdateStudentDto) => {
    return httpClient.put<void>(`/students/${id}`, student);
  },
  resetPassword: (id: string, data: ResetPasswordDto) => {
    return httpClient.put<void>(`/students/${id}/reset-password`, data);
  },
  getPatientRecord: (id: string) => {
    return httpClient.get<PatientRecordDto>(`/students/${id}/patient-record`);
  },
  getPatientObservations: (id: string) => {
    return httpClient.get<PatientObservationDto[]>(`/students/${id}/patient-observations`);
  },
  getEnrollments: (id: string) => {
    return httpClient.get<EnrollmentDto[]>(`/students/${id}/enrollments`);
  },
  enroll: (id: string, enrollmentData: EnrollmentData) => {
    return httpClient.post<EnrollmentDto>(`/students/${id}/enrollments`, enrollmentData);
  },
  removeEnrollment: (studentId: string, enrollmentId: string) => {
    return httpClient.delete<void>(`/students/${studentId}/enrollments/${enrollmentId}`);
  },
};
