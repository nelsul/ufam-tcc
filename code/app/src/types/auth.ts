export interface LoginCredentials {
  email: string;
  password: string;
}

export interface UserDto {
  publicId: string;
  name: string;
  fullName: string;
  institutionalEmail: string;
  registration: string;
  status: "active" | "inactive";
  role: "admin" | "professional" | "student" | "assistant" | "professor";
}

export interface LoginResponse {
  token: string;
  user: UserDto;
}
