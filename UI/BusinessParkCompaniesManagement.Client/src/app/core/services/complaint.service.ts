import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ComplaintCreatePayload,
  ComplaintListItem,
  ComplaintStatus,
  ComplaintType
} from '../models/complaint.models';
import { PagedResult } from '../models/company.models';

export interface ComplaintQueryParams {
  status?: ComplaintStatus;
  companyId?: number;
  complaintTypeId?: number;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class ComplaintService {
  private readonly base = `${environment.apiBaseUrl}/complaints`;

  constructor(private http: HttpClient) {}

  types(): Observable<ComplaintType[]> {
    return this.http.get<ComplaintType[]>(`${this.base}/types`);
  }

  list(params: ComplaintQueryParams = {}): Observable<PagedResult<ComplaintListItem>> {
    let httpParams: Record<string, string> = {};
    if (params.status) httpParams['status'] = params.status;
    if (params.companyId) httpParams['companyId'] = String(params.companyId);
    if (params.complaintTypeId) httpParams['complaintTypeId'] = String(params.complaintTypeId);
    httpParams['page'] = String(params.page ?? 1);
    httpParams['pageSize'] = String(params.pageSize ?? 20);

    return this.http.get<PagedResult<ComplaintListItem>>(this.base, { params: httpParams });
  }

  create(payload: ComplaintCreatePayload): Observable<ComplaintListItem> {
    return this.http.post<ComplaintListItem>(this.base, payload);
  }

  updateStatus(id: number, status: ComplaintStatus): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}/status`, { status });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
