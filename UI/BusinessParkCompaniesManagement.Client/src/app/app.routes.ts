import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { authGuard, adminGuard, companyGuard } from './core/guards/auth.guard';
import { AuthService } from './core/services/auth.service';
import { LoginComponent } from './features/login.component/login.component';
import { ShellComponent } from './features/shell.component/shell.component';
import { CompanyListComponent } from './features/companies/company-list.component/company-list.component';
import { CompanyFormComponent } from './features/companies/company-form.component/company-form.component';
import { ComplaintListComponent } from './features/complaints/complaint-list.component/complaint-list.component';
import { ReportsComponent } from './features/reports.component/reports.component';
import { ComplaintFormComponent } from './features/complaints/complaint-form.component/complaint-form.component';

// Sends a logged-in user to the right landing page for their role.
function homeRedirect(): UrlTree {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAdmin()) return router.parseUrl('/admin/companies');
  if (auth.isCompany()) return router.parseUrl('/company/complaints');
  return router.parseUrl('/login');
}

export const routes: Routes = [
    {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', canActivate: [() => homeRedirect()], children: [] },
      {
        path: 'admin/companies',
        canActivate: [adminGuard],
        component: CompanyListComponent
      },
      {
        path: 'admin/companies/new',
        canActivate: [adminGuard],
        component: CompanyFormComponent
      },
      {
        path: 'admin/companies/:id/edit',
        canActivate: [adminGuard],
        component: CompanyFormComponent
      },
      {
        path: 'admin/complaints',
        canActivate: [adminGuard],
        component: ComplaintListComponent
      },
      {
        path: 'admin/reports',
        canActivate: [adminGuard],
        component: ReportsComponent
      },
      {
        path: 'company/complaints',
        canActivate: [companyGuard],
        component: ComplaintListComponent
      },
      {
        path: 'company/complaints/new',
        canActivate: [companyGuard],
        component: ComplaintFormComponent
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
