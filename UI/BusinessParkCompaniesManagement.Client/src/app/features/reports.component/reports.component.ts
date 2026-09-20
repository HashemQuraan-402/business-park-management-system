import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportService } from '../../core/services/report.service';
import { I18nService } from '../../core/services/i18n.service';
import { CompanyStatisticsReport } from '../../core/models/report.models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent implements OnInit {
  readonly report = signal<CompanyStatisticsReport | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  constructor(private reportService: ReportService, public i18n: I18nService) {}

  ngOnInit(): void {
    this.loading.set(true);
    this.reportService.statistics().subscribe({
      next: (r) => {
        this.report.set(r);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set(this.i18n.t('save_error'));
        this.loading.set(false);
      }
    });
  }

  sizeLabel(size: string): string {
    const key = size.toLowerCase();
    return this.i18n.t(key === 'small' || key === 'medium' || key === 'large' ? key : size);
  }
}
