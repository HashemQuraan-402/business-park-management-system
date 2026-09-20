import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ComplaintService } from '../../../core/services/complaint.service';
import { I18nService } from '../../../core/services/i18n.service';
import { ComplaintListItem, ComplaintStatus } from '../../../core/models/complaint.models';

@Component({
  selector: 'app-complaint-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './complaint-list.component.html',
  styleUrl: './complaint-list.component.css'
})
export class ComplaintListComponent implements OnInit {
  readonly complaints = signal<ComplaintListItem[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly actionError = signal<string | null>(null);
  readonly updatingId = signal<number | null>(null);

  readonly total = signal(0);
  readonly page = signal(1);
  readonly pageSize = 10;

  statusFilter: ComplaintStatus | '' = '';

  readonly statuses: ComplaintStatus[] = ['New', 'InProgress', 'Resolved', 'Rejected'];

  constructor(
    public auth: AuthService,
    private complaintService: ComplaintService,
    public i18n: I18nService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  onFilterChange(): void {
    this.page.set(1);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.complaintService
      .list({
        status: this.statusFilter || undefined,
        page: this.page(),
        pageSize: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.complaints.set(res.items);
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

  updateStatus(complaint: ComplaintListItem, status: ComplaintStatus): void {
    if (status === complaint.status) return;
    this.updatingId.set(complaint.id);
    this.actionError.set(null);

    this.complaintService.updateStatus(complaint.id, status).subscribe({
      next: () => {
        this.updatingId.set(null);
        this.load();
      },
      error: () => {
        this.updatingId.set(null);
        this.actionError.set(this.i18n.t('save_error'));
      }
    });
  }

  canDelete(complaint: ComplaintListItem): boolean {
    if (this.auth.isAdmin()) return true;
    return complaint.status === 'New';
  }

  deleteComplaint(complaint: ComplaintListItem): void {
    if (!confirm(this.i18n.t('confirmDelete'))) return;

    this.actionError.set(null);
    this.complaintService.delete(complaint.id).subscribe({
      next: () => this.load(),
      error: (err) => this.actionError.set(err?.error?.message || this.i18n.t('save_error'))
    });
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'New':
        return 'badge-new';
      case 'InProgress':
        return 'badge-inprogress';
      case 'Resolved':
        return 'badge-resolved';
      case 'Rejected':
        return 'badge-rejected';
      default:
        return '';
    }
  }

  statusLabel(status: string): string {
    return this.i18n.t('status' + status);
  }
}
