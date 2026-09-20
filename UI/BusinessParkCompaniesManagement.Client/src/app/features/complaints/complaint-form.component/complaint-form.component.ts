import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ComplaintService } from '../../../core/services/complaint.service';
import { I18nService } from '../../../core/services/i18n.service';
import { ComplaintPriority, ComplaintType } from '../../../core/models/complaint.models';

@Component({
  selector: 'app-complaint-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './complaint-form.component.html',
  styleUrl: './complaint-form.component.css'
})
export class ComplaintFormComponent implements OnInit {
  // The 10 predefined complaint/suggestion categories, populated from the backend dropdown endpoint.
  readonly complaintTypes = signal<ComplaintType[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);

   private readonly fb = inject(FormBuilder);
   private readonly complaintService = inject(ComplaintService);
    private readonly auth = inject(AuthService);
    private readonly router = inject(Router);
    public readonly i18n = inject(I18nService);


  readonly form = this.fb.group({
    complaintTypeId: [null as number | null, Validators.required],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    priority: ['Medium' as ComplaintPriority, Validators.required]
  });

  constructor(

  ) {}

  ngOnInit(): void {
    this.loading.set(true);
    this.complaintService.types().subscribe({
      next: (types) => {
        this.complaintTypes.set(types);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set(this.i18n.t('save_error'));
        this.loading.set(false);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const companyId = this.auth.companyId();
    if (!companyId) {
      this.errorMessage.set(this.i18n.t('save_error'));
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    const v = this.form.value;
    this.complaintService
      .create({
        companyId,
        complaintTypeId: v.complaintTypeId!,
        description: v.description!,
        priority: v.priority!
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigateByUrl('/company/complaints');
        },
        error: (err) => {
          this.saving.set(false);
          this.errorMessage.set(err?.error?.message || this.i18n.t('save_error'));
        }
      });
  }
}
