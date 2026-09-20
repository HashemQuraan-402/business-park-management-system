import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CompanyStatisticsReport } from '../models/report.models';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly base = `${environment.apiBaseUrl}/reports`;

  constructor(private http: HttpClient) {}

  statistics(): Observable<CompanyStatisticsReport> {
    return this.http.get<CompanyStatisticsReport>(`${this.base}/statistics`);
  }
}
