import { Component, computed, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { I18nService } from '../../core/services/i18n.service';

type LoginRole = 'admin' | 'company';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  readonly role = signal<LoginRole>('admin');
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  public readonly i18n = inject(I18nService);


  readonly form = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  readonly roleLabel = computed(() =>
    this.role() === 'admin' ? this.i18n.t('loginAsAdmin') : this.i18n.t('loginAsCompany')
  );

  constructor() {}

  setRole(role: LoginRole): void {
    this.role.set(role);
    this.errorMessage.set(null);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    const payload = {
      username: this.form.value.username!.trim(),
      password: this.form.value.password!
    };

    const request$ =
      this.role() === 'admin' ? this.auth.loginAdmin(payload) : this.auth.loginCompany(payload);

    request$.subscribe({
      next: () => {
        this.loading.set(false);
        const target = this.role() === 'admin' ? '/admin/companies' : '/company/complaints';
        this.router.navigateByUrl(target);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set(this.i18n.t('invalidCredentials'));
      }
    });
  }
}
