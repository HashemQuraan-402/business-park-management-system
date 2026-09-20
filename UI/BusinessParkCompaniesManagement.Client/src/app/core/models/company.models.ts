export type CompanySize = 'Small' | 'Medium' | 'Large';

export interface Sector {
  id: number;
  nameEn: string;
  nameAr: string;
}

export interface CompanyListItem {
  id: number;
  nameEn: string;
  nameAr: string;
  sectorNameEn: string;
  sectorNameAr: string;
  size: string;
  employeeCount: number;
  email?: string | null;
  phone?: string | null;
  createdAt: string;
}

export interface CompanyDetail extends CompanyListItem {
  sectorId: number;
  buildingNumber?: string | null;
  floorNumber?: string | null;
  address?: string | null;
  username?: string | null;
}

// Matches CompanyCreateDto - also used for PUT (update) since the backend
// reuses the same DTO shape for both create and update.
export interface CompanyCreatePayload {
  nameEn: string;
  nameAr: string;
  sectorId: number;
  size: CompanySize;
  employeeCount: number;
  email?: string | null;
  phone?: string | null;
  buildingNumber?: string | null;
  floorNumber?: string | null;
  address?: string | null;
  // Required by the backend (Companies.Username is NOT NULL) - always send these.
  username: string;
  password?: string | null;
}

export interface PagedResult<T> {
  total: number;
  page: number;
  pageSize: number;
  items: T[];
}
