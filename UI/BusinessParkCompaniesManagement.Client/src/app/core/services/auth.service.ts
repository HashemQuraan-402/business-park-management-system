import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CurrentUser, LoginRequest, LoginResponse } from '../models/auth.models';

const STORAGE_KEY = 'bp_auth_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _currentUser = signal<CurrentUser | null>(this.readFromStorage());

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => !!this._currentUser());
  readonly role = computed(() => this._currentUser()?.role ?? null);
  readonly isAdmin = computed(() => this._currentUser()?.role === 'Admin');
  readonly isCompany = computed(() => this._currentUser()?.role === 'Company');
  readonly companyId = computed(() => this._currentUser()?.companyId ?? null);

  constructor(private http: HttpClient) {}

  loginAdmin(payload: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login/admin`, payload)
      .pipe(tap((res) => this.setSession(res)));
  }

  loginCompany(payload: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login/company`, payload)
      .pipe(tap((res) => this.setSession(res)));
  }

  logout(): void {
    this._currentUser.set(null);
    sessionStorage.removeItem(STORAGE_KEY);
  }

  getToken(): string | null {
    return this._currentUser()?.token ?? null;
  }

  private setSession(res: LoginResponse): void {
    const user: CurrentUser = {
      token: res.token,
      role: res.role,
      displayName: res.displayname,
      companyId: res.companyId ?? null,
      expiresAt: res.expiresAt
    };
    this._currentUser.set(user);
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  }

  private readFromStorage(): CurrentUser | null {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      const user = JSON.parse(raw) as CurrentUser;
      // Drop the session if the token has already expired.
      if (user.expiresAt && new Date(user.expiresAt).getTime() < Date.now()) {
        sessionStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return user;
    } catch {
      return null;
    }
  }
}
