export type ComplaintPriority = 'Low' | 'Medium' | 'High';
export type ComplaintStatus = 'New' | 'InProgress' | 'Resolved' | 'Rejected';

export interface ComplaintType {
  id: number;
  nameEn: string;
  nameAr: string;
}

// Matches ComplaintCreateDto - the backend does not collect a Subject field,
// only a free-text Description plus the ComplaintTypeId chosen from the dropdown.
export interface ComplaintCreatePayload {
  companyId: number;
  complaintTypeId: number;
  description: string;
  priority: ComplaintPriority;
}

// Matches ComplaintListItemDto. Note: the backend's mapping method never
// actually populates "subject"/"adminResponse" even though the DTO declares
// them, so treat those two as always empty in practice.
export interface ComplaintListItem {
  id: number;
  companyId: number;
  companyNameEn: string;
  companyNameAr: string;
  complaintTypeId: number;
  complaintTypeNameEn: string;
  complaintTypeNameAr: string;
  subject: string;
  description: string;
  priority: string;
  status: string;
  adminResponse?: string | null;
  createdAt: string;
  resolvedAt?: string | null;
}

export interface ComplaintStatusUpdatePayload {
  status: ComplaintStatus;
}
