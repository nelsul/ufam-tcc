import { httpClient } from "./httpClient";
import type { PatientObservationDto } from "@/types/api";

interface CreatePatientObservationDto {
  studentId: string;
  observationId: string;
  notes: string;
}

export const patientObservationsService = {
  create: (observation: CreatePatientObservationDto) => {
    return httpClient.post<PatientObservationDto>("/patientobservations", observation);
  },
  update: (id: string, observation: Partial<CreatePatientObservationDto>) => {
    return httpClient.put<PatientObservationDto>(`/patientobservations/${id}`, observation);
  },
  delete: (id: string) => {
    return httpClient.delete<void>(`/patientobservations/${id}`);
  },
};
