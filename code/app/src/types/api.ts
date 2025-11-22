
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
}

export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  includeInactive?: boolean;
  [key: string]: string | number | boolean | undefined;
}

export interface ProfessorDto {
  publicId: string;
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: string;
}

export interface CreateProfessorDto {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
}

export interface UpdateProfessorDto {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: string;
}

export interface ResetPasswordDto {
  newPassword: string;
}

export interface SemesterDto {
  publicId: string;
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}

export interface CreateSemesterDto {
  name: string;
  startDate: string;
  endDate: string;
}

export interface UpdateSemesterDto {
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}

export interface SubjectDto {
  publicId: string;
  name: string;
  code: string;
  description: string;
  status: string;
}

export interface CreateSubjectDto {
  name: string;
  code: string;
  description: string;
}

export interface UpdateSubjectDto {
  name: string;
  code: string;
  description: string;
  status: string;
}

export interface SessionTypeDto {
  publicId: string;
  name: string;
  durationMinutes: number;
  description: string;
  status: string;
}

export interface CreateSessionTypeDto {
  name: string;
  durationMinutes: number;
  description: string;
}

export interface UpdateSessionTypeDto {
  name: string;
  durationMinutes: number;
  description: string;
  status: string;
}

export interface SubjectOfferingDto {
  publicId: string;
  subject: {
    publicId: string;
    name: string;
    code: string;
  };
  professor: {
    publicId: string;
    name: string;
    fullName: string;
  };
  semester: {
    publicId: string;
    name: string;
  };
  status: string;
}

export interface CreateSubjectOfferingDto {
  semesterId: string;
  subjectId: string;
  professorId: string;
}

export interface UpdateSubjectOfferingDto {
  semesterId: string;
  subjectId: string;
  professorId: string;
  status: string;
}

export interface StudentDto {
  publicId: string;
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: string;
}

export interface CreateStudentDto {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
}

export interface UpdateStudentDto {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: string;
}

export interface PatientRecordDto {
  id: string;
  studentId: string;
  studentName: string;
  content: string;
  createdAt: string;
  updatedAt: string;
}

export interface ObservationDto {
  id: string;
  name: string;
  description: string | null;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateObservationDto {
  name?: string;
  description?: string;
  status?: string;
}

export interface PatientObservationDto {
  id: string;
  studentId: string;
  studentName: string;
  observationId: string;
  observationName: string;
  professionalId: string;
  professionalName: string;
  notes: string;
  createdAt: string;
  updatedAt: string;
}

export interface EnrollmentDto {
  subjectCode: any;
  publicId: string;
  studentId: string;
  subjectOfferingId: string;
  subjectName: string;
  professorName: string;
  semesterName: string;
  status: string;
}

export interface StudentEnrollmentDto {
  publicId: string;
  studentId: string;
  studentName: string;
  studentRegistration: string;
  studentEmail: string;
  subjectOfferingId: string;
  subjectName: string;
  subjectCode: string;
  semesterName: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface AvailabilityTimeRange {
  start: string;
  end: string;
  startISO?: string;
}

export interface AvailabilityDay {
  date: string;
  timeRanges: AvailabilityTimeRange[];
}

export interface AppointmentDto {
  id: string;
  publicId: string;
  studentId: string | null;
  studentFullName: string;
  startTime: string;
  endTime: string | null;
  sessionTypeName: string | null;
  status: string;
}

export interface StudentFormData {
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  password?: string;
}

export type ObservationWithId = ObservationDto;
export type PatientObservationWithId = PatientObservationDto;
