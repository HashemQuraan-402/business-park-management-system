export interface LoginRequest {
  username: string;
  password: string;
}

// Matches LoginResponseDto - note "Displayname" (lowercase n) as sent by the backend.
export interface LoginResponse {
  token: string;
  expiresAt: string;
  role: 'Admin' | 'Company';
  displayname: string;
  companyId?: number | null;
}

export interface CurrentUser {
  token: string;
  role: 'Admin' | 'Company';
  displayName: string;
  companyId: number | null;
  expiresAt: string;
}
