import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, Subject } from 'rxjs';
import { CompanyService } from '../../../core/services/company.service';
import { I18nService } from '../../../core/services/i18n.service';
import { CompanyListItem, CompanySize, Sector } from '../../../core/models/company.models';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './company-list.component.html',
  styleUrl: './company-list.component.css'
})
export class CompanyListComponent implements OnInit {
  readonly companies = signal<CompanyListItem[]>([]);
  readonly sectors = signal<Sector[]>([]);
  readonly loading = signal(false);
  readonly exporting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly deleteError = signal<string | null>(null);

  readonly total = signal(0);
  readonly page = signal(1);
  readonly pageSize = 10;

  search = '';
  sectorId: number | null = null;
  size: CompanySize | '' = '';

  private searchInput$ = new Subject<void>();

  constructor(private companyService: CompanyService, public i18n: I18nService) {
    this.searchInput$.pipe(debounceTime(350)).subscribe(() => {
      this.page.set(1);
      this.load();
    });
  }

  ngOnInit(): void {
    this.companyService.sectors().subscribe((s) => this.sectors.set(s));
    this.load();
  }

  onSearchChange(): void {
    this.searchInput$.next();
  }

  onFilterChange(): void {
    this.page.set(1);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.companyService
      .list({
        search: this.search || undefined,
        sectorId: this.sectorId ?? undefined,
        size: this.size || undefined,
        page: this.page(),
        pageSize: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.companies.set(res.items);
          this.total.set(res.total);
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set(this.i18n.t('save_error'));
          this.loading.set(false);
        }
      });
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.total() / this.pageSize));
  }

  goToPage(delta: number): void {
    const next = this.page() + delta;
    if (next < 1 || next > this.totalPages) return;
    this.page.set(next);
    this.load();
  }

  deleteCompany(company: CompanyListItem): void {
    if (!confirm(this.i18n.t('confirmDelete'))) return;

    this.deleteError.set(null);
    this.companyService.delete(company.id).subscribe({
      next: () => this.load(),
      error: () => this.deleteError.set(this.i18n.t('save_error'))
    });
  }

  exportExcel(): void {
    this.exporting.set(true);
    this.companyService.exportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Companies_${new Date().toISOString().slice(0, 10)}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.exporting.set(false);
      },
      error: () => {
        this.errorMessage.set(this.i18n.t('save_error'));
        this.exporting.set(false);
      }
    });
  }

  sizeLabel(size: string): string {
    const key = size.toLowerCase();
    return this.i18n.t(key === 'small' || key === 'medium' || key === 'large' ? key : size);
  }
}
