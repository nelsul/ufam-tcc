import { httpClient } from "./httpClient";
import type {
  PagedResult,
  StudentEnrollmentDto,
  SubjectOfferingDto,
  StudentDto,
  PatientObservationDto,
} from "@/types/api";

export interface ProfessorStudentsFilters extends Record<
  string,
  string | number | boolean | undefined
> {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}

export const professorStudentsService = {
  getMyActiveOfferings: () => {
    return httpClient.get<PagedResult<SubjectOfferingDto>>("/SubjectOfferings/my", {
      pageNumber: 1,
      pageSize: 100,
      includeInactive: false,
    });
  },
  getStudentsByOffering: (offeringId: string, params?: ProfessorStudentsFilters) => {
    return httpClient.get<PagedResult<StudentEnrollmentDto>>(
      `/SubjectOfferings/${offeringId}/students`,
      params
    );
  },
  getStudentDetails: (studentId: string) => {
    return httpClient.get<StudentDto>(`/SubjectOfferings/students/${studentId}`);
  },
  getStudentObservations: (studentId: string) => {
    return httpClient.get<PatientObservationDto[]>(
      `/SubjectOfferings/students/${studentId}/observations`
    );
  },
};
