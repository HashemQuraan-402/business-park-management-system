import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CompanyCreatePayload,
  CompanyDetail,
  CompanyListItem,
  CompanySize,
  PagedResult,
  Sector
} from '../models/company.models';

export interface CompanyQueryParams {
  search?: string;
  sectorId?: number;
  size?: CompanySize;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly base = `${environment.apiBaseUrl}/companies`;

  constructor(private http: HttpClient) {}

  list(params: CompanyQueryParams = {}): Observable<PagedResult<CompanyListItem>> {
    let httpParams: Record<string, string> = {};
    if (params.search) httpParams['search'] = params.search;
    if (params.sectorId) httpParams['sectorId'] = String(params.sectorId);
    if (params.size) httpParams['size'] = params.size;
    httpParams['page'] = String(params.page ?? 1);
    httpParams['pageSize'] = String(params.pageSize ?? 20);

    return this.http.get<PagedResult<CompanyListItem>>(this.base, { params: httpParams });
  }

  get(id: number): Observable<CompanyDetail> {
    return this.http.get<CompanyDetail>(`${this.base}/${id}`);
  }

  create(payload: CompanyCreatePayload): Observable<CompanyDetail> {
    return this.http.post<CompanyDetail>(this.base, payload);
  }

  update(id: number, payload: CompanyCreatePayload): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  sectors(): Observable<Sector[]> {
    return this.http.get<Sector[]>(`${this.base}/sectors`);
  }

  exportExcel(): Observable<Blob> {
    return this.http.get(`${this.base}/export`, { responseType: 'blob' });
  }
}
