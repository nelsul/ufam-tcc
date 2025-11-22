import { httpClient } from "./httpClient";
import type { PatientRecordDto } from "@/types/api";

interface CreatePatientRecordDto {
  studentId: string;
  content: string;
}

export const patientRecordsService = {
  create: (record: CreatePatientRecordDto) => {
    return httpClient.post<PatientRecordDto>("/PatientRecords", record);
  },
  update: (id: string, record: Partial<CreatePatientRecordDto>) => {
    return httpClient.put<PatientRecordDto>(`/PatientRecords/${id}`, record);
  },
  getById: (id: string) => {
    return httpClient.get<PatientRecordDto>(`/PatientRecords/${id}`);
  },
  getByStudentId: (studentId: string) => {
    return httpClient.get<PatientRecordDto[]>(`/PatientRecords/student/${studentId}`);
  },
};
