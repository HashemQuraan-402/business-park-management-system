import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';
import { I18nService } from '../../../core/services/i18n.service';
import { CompanySize, Sector } from '../../../core/models/company.models';

@Component({
  selector: 'app-company-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './company-form.component.html',
  styleUrl: './company-form.component.css'
})
export class CompanyFormComponent implements OnInit {
  readonly sectors = signal<Sector[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);



  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  public readonly i18n = inject(I18nService);

  companyId: number | null = null;
  get isEdit(): boolean {
    return this.companyId !== null;
  }

  constructor() {}

  readonly form = this.fb.group({
    nameEn: ['', [Validators.required, Validators.maxLength(200)]],
    nameAr: ['', [Validators.required, Validators.maxLength(200)]],
    sectorId: [null as number | null, Validators.required],
    size: ['Medium' as CompanySize, Validators.required],
    employeeCount: [0, [Validators.required, Validators.min(0)]],
    email: ['', Validators.email],
    phone: [''],
    buildingNumber: [''],
    floorNumber: [''],
    address: [''],
    username: ['', Validators.required],
    password: ['']
  });



  ngOnInit(): void {
    this.companyService.sectors().subscribe((s) => this.sectors.set(s));

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.companyId = Number(idParam);
      // Password is not required on edit - leaving it blank keeps the current one.
      this.form.controls.password.clearValidators();
      this.loadCompany(this.companyId);
    } else {
      this.form.controls.password.setValidators(Validators.required);
    }
  }

  private loadCompany(id: number): void {
    this.loading.set(true);
    this.companyService.get(id).subscribe({
      next: (c) => {
        this.form.patchValue({
          nameEn: c.nameEn,
          nameAr: c.nameAr,
          sectorId: c.sectorId,
          size: c.size as CompanySize,
          employeeCount: c.employeeCount,
          email: c.email ?? '',
          phone: c.phone ?? '',
          buildingNumber: c.buildingNumber ?? '',
          floorNumber: c.floorNumber ?? '',
          address: c.address ?? '',
          username: c.username ?? ''
        });
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

    this.saving.set(true);
    this.errorMessage.set(null);

    const v = this.form.value;
    const payload = {
      nameEn: v.nameEn!,
      nameAr: v.nameAr!,
      sectorId: v.sectorId!,
      size: v.size!,
      employeeCount: v.employeeCount!,
      email: v.email || null,
      phone: v.phone || null,
      buildingNumber: v.buildingNumber || null,
      floorNumber: v.floorNumber || null,
      address: v.address || null,
      username: v.username!,
      password: v.password || null
    };

   if (this.isEdit) {
  this.companyService.update(this.companyId!, payload).subscribe({
    next: () => {
      this.saving.set(false);
      this.router.navigateByUrl('/admin/companies');
    },
    error: (err: any) => {
      this.saving.set(false);
      this.errorMessage.set(
        err?.error?.message || this.i18n.t('save_error')
      );
    }
  });
} else {
  this.companyService.create(payload).subscribe({
    next: () => {
      this.saving.set(false);
      this.router.navigateByUrl('/admin/companies');
    },
    error: (err: any) => {
      this.saving.set(false);
      this.errorMessage.set(
        err?.error?.message || this.i18n.t('save_error')
      );
    }
  });
}
  }

}
