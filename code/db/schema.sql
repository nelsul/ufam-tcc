
CREATE OR REPLACE FUNCTION update_modified_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';



CREATE TYPE general_status_enum AS ENUM ('active', 'inactive');



CREATE TYPE user_status_enum AS ENUM ('active', 'inactive');
CREATE TYPE user_role_enum AS ENUM ('admin', 'professional', 'student', 'assistant', 'professor');
CREATE TABLE public.users (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    "name" text NOT NULL,
    full_name text NOT NULL,
    institutional_email text NOT NULL,
    registration text NOT NULL,
    password_hash text NULL,
    status public.user_status_enum DEFAULT 'active'::user_status_enum NOT NULL,
    "role" public.user_role_enum DEFAULT 'student'::user_role_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT users_institutional_email_check CHECK (position('@' in institutional_email) > 1),
    CONSTRAINT users_institutional_email_key UNIQUE (institutional_email),
    CONSTRAINT users_pkey PRIMARY KEY (id),
    CONSTRAINT users_public_id_key UNIQUE (public_id),
    CONSTRAINT users_registration_key UNIQUE (registration)
);
CREATE TRIGGER update_users_modtime
    BEFORE UPDATE ON public.users
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.semesters (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	public_id uuid DEFAULT gen_random_uuid() NOT NULL,
	"name" text NOT NULL,
	start_date date NOT NULL,
	end_date date NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
	created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
	updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT semesters_date_check CHECK ((end_date > start_date)),
	CONSTRAINT semesters_name_key UNIQUE (name),
	CONSTRAINT semesters_pkey PRIMARY KEY (id),
	CONSTRAINT semesters_public_id_key UNIQUE (public_id)
);
CREATE TRIGGER update_semesters_modtime
    BEFORE UPDATE ON public.semesters
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.subjects (
	id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
	public_id uuid DEFAULT gen_random_uuid() NOT NULL,
	"name" text NOT NULL,
	code text NOT NULL,
	description text NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
	created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
	updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT subjects_code_key UNIQUE (code),
	CONSTRAINT subjects_pkey PRIMARY KEY (id),
	CONSTRAINT subjects_public_id_key UNIQUE (public_id)
);
CREATE TRIGGER update_subjects_modtime
    BEFORE UPDATE ON public.subjects
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.subject_offerings (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    semester_id int8 NOT NULL,
    subject_id int8 NOT NULL,
    professor_id int8 NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT subject_offerings_pkey PRIMARY KEY (id),
    CONSTRAINT subject_offerings_public_id_key UNIQUE (public_id),
    CONSTRAINT subject_offerings_unique_assignment UNIQUE (semester_id, subject_id, professor_id),
    CONSTRAINT subject_offerings_semester_id_fkey FOREIGN KEY (semester_id) REFERENCES public.semesters(id),
    CONSTRAINT subject_offerings_subject_id_fkey FOREIGN KEY (subject_id) REFERENCES public.subjects(id),
    CONSTRAINT subject_offerings_professor_id_fkey FOREIGN KEY (professor_id) REFERENCES public.users(id)
);
CREATE INDEX idx_subject_offerings_professor ON public.subject_offerings (professor_id);
CREATE INDEX idx_subject_offerings_semester ON public.subject_offerings (semester_id);
CREATE TRIGGER update_subject_offerings_modtime
    BEFORE UPDATE ON public.subject_offerings
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.availabilities (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    professional_id int8 NOT NULL,
    start_time timestamptz NOT NULL,
    end_time timestamptz NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT availabilities_pkey PRIMARY KEY (id),
    CONSTRAINT availabilities_public_id_key UNIQUE (public_id),
    CONSTRAINT availabilities_professional_id_fkey FOREIGN KEY (professional_id) REFERENCES public.users(id),
    CONSTRAINT availabilities_time_check CHECK (end_time > start_time)
);
CREATE INDEX idx_availabilities_time ON public.availabilities (start_time, end_time);
CREATE INDEX idx_availabilities_professional ON public.availabilities (professional_id);
CREATE TRIGGER update_availabilities_modtime
    BEFORE UPDATE ON public.availabilities
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.session_types (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    "name" text NOT NULL,          
    duration_minutes int4 NOT NULL,
    description text NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT session_types_pkey PRIMARY KEY (id),
    CONSTRAINT session_types_public_id_key UNIQUE (public_id),
    CONSTRAINT session_types_name_key UNIQUE (name),
    CONSTRAINT session_types_duration_check CHECK (duration_minutes > 0)
);
CREATE TRIGGER update_session_types_modtime
    BEFORE UPDATE ON public.session_types
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TYPE appointment_status_enum AS ENUM ('pending', 'confirmed', 'in_session', 'cancelled', 'completed');
CREATE TABLE public.appointments (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    professional_id int8 NOT NULL,
    student_id int8 NULL,
    student_email text NOT NULL,
    student_registration text NOT NULL,
    student_full_name text NOT NULL,
    session_type_id int8 NULL,
    start_time timestamptz NOT NULL,
    end_time timestamptz NULL,
    status public.appointment_status_enum DEFAULT 'pending'::appointment_status_enum NOT NULL,
    reason_for_visit text NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT appointments_pkey PRIMARY KEY (id),
    CONSTRAINT appointments_public_id_key UNIQUE (public_id),
    CONSTRAINT appointments_professional_id_fkey FOREIGN KEY (professional_id) REFERENCES public.users(id),
    CONSTRAINT appointments_student_id_fkey FOREIGN KEY (student_id) REFERENCES public.users(id) ON DELETE SET NULL,
    CONSTRAINT appointments_session_type_fkey FOREIGN KEY (session_type_id) REFERENCES public.session_types(id),
    CONSTRAINT appointments_time_check CHECK (end_time IS NULL OR end_time > start_time),
    CONSTRAINT appointments_student_email_check CHECK (position('@' in student_email) > 1)
);
CREATE INDEX idx_appointments_time ON public.appointments (start_time, end_time);
CREATE INDEX idx_appointments_professional ON public.appointments (professional_id);
CREATE INDEX idx_appointments_student ON public.appointments (student_id);
CREATE INDEX idx_appointments_session_type ON public.appointments (session_type_id);
CREATE TRIGGER update_appointments_modtime
    BEFORE UPDATE ON public.appointments
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TYPE session_status_enum AS ENUM ('in_progress', 'completed', 'cancelled');
CREATE TABLE public.sessions (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    appointment_id int8 NOT NULL,
    professional_id int8 NOT NULL,
    student_id int8 NOT NULL,
    started_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    ended_at timestamptz NULL,
    notes TEXT NOT NULL,
    status public.session_status_enum DEFAULT 'in_progress'::session_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT sessions_pkey PRIMARY KEY (id),
    CONSTRAINT sessions_public_id_key UNIQUE (public_id),
    CONSTRAINT sessions_appointment_id_key UNIQUE (appointment_id),
    CONSTRAINT sessions_appointment_id_fkey FOREIGN KEY (appointment_id) REFERENCES public.appointments(id),
    CONSTRAINT sessions_professional_id_fkey FOREIGN KEY (professional_id) REFERENCES public.users(id),
    CONSTRAINT sessions_student_id_fkey FOREIGN KEY (student_id) REFERENCES public.users(id),
    CONSTRAINT sessions_time_check CHECK (ended_at IS NULL OR ended_at > started_at),
    CONSTRAINT sessions_participants_check CHECK (professional_id <> student_id)
);
CREATE INDEX idx_sessions_professional ON public.sessions (professional_id);
CREATE INDEX idx_sessions_appointment ON public.sessions (appointment_id);
CREATE INDEX idx_sessions_student ON public.sessions (student_id);
CREATE TRIGGER update_sessions_modtime
    BEFORE UPDATE ON public.sessions
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.patient_records (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    student_id int8 NOT NULL,
    content text NOT NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT patient_records_pkey PRIMARY KEY (id),
    CONSTRAINT patient_records_public_id_key UNIQUE (public_id),
    CONSTRAINT patient_records_student_id_fkey FOREIGN KEY (student_id) REFERENCES public.users(id)
);
CREATE INDEX idx_patient_records_student ON public.patient_records (student_id);
CREATE TRIGGER update_patient_records_modtime
    BEFORE UPDATE ON public.patient_records
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.observations (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    "name" text NOT NULL,
    description text NULL,
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT observations_pkey PRIMARY KEY (id),
    CONSTRAINT observations_public_id_key UNIQUE (public_id),
    CONSTRAINT observations_name_key UNIQUE (name)
);
CREATE TRIGGER update_observations_modtime
    BEFORE UPDATE ON public.observations
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TABLE public.patient_observations (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    student_id int8 NOT NULL,
    observation_id int8 NOT NULL, 
    professional_id int8 NOT NULL,      
    notes text NOT NULL, 
    status public.general_status_enum DEFAULT 'active'::general_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT patient_observations_pkey PRIMARY KEY (id),
    CONSTRAINT patient_observations_public_id_key UNIQUE (public_id),
    CONSTRAINT patient_observations_student_id_fkey FOREIGN KEY (student_id) REFERENCES public.users(id),
    CONSTRAINT patient_observations_observation_id_fkey FOREIGN KEY (observation_id) REFERENCES public.observations(id),
    CONSTRAINT patient_observations_professional_id_fkey FOREIGN KEY (professional_id) REFERENCES public.users(id),
    CONSTRAINT patient_observations_professional_check CHECK (professional_id <> student_id)
);
CREATE INDEX idx_patient_observations_student ON public.patient_observations (student_id);
CREATE INDEX idx_patient_observations_professional ON public.patient_observations (professional_id);
CREATE INDEX idx_patient_observations_type ON public.patient_observations (observation_id);
CREATE TRIGGER update_patient_observations_modtime
    BEFORE UPDATE ON public.patient_observations
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();



CREATE TYPE enrollment_status_enum AS ENUM ('enrolled', 'approved', 'failed', 'dropped', 'cancelled');

CREATE TABLE public.student_enrollments (
    id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL,
    public_id uuid DEFAULT gen_random_uuid() NOT NULL,
    student_id int8 NOT NULL,
    subject_offering_id int8 NOT NULL,
    status public.enrollment_status_enum DEFAULT 'enrolled'::enrollment_status_enum NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT student_enrollments_pkey PRIMARY KEY (id),
    CONSTRAINT student_enrollments_public_id_key UNIQUE (public_id),
    CONSTRAINT student_enrollments_student_id_fkey FOREIGN KEY (student_id) REFERENCES public.users(id),
    CONSTRAINT student_enrollments_offering_id_fkey FOREIGN KEY (subject_offering_id) REFERENCES public.subject_offerings(id),
    CONSTRAINT student_enrollments_unique_entry UNIQUE (student_id, subject_offering_id)
);
CREATE INDEX idx_student_enrollments_student ON public.student_enrollments (student_id);
CREATE INDEX idx_student_enrollments_offering ON public.student_enrollments (subject_offering_id);
CREATE TRIGGER update_student_enrollments_modtime
    BEFORE UPDATE ON public.student_enrollments
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_column();