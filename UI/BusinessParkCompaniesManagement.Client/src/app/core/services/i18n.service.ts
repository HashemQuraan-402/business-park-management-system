import { Injectable, computed, effect, signal } from '@angular/core';

export type Lang = 'en' | 'ar';

const STORAGE_KEY = 'bp_lang';

const TRANSLATIONS: Record<Lang, Record<string, string>> = {
  en: {
    appTitle: 'Nexora Business Park',
    login: 'Login',
    admin: 'Admin',
    company: 'Company',
    username: 'Username',
    password: 'Password',
    loginAsAdmin: 'Login as Administrator',
    loginAsCompany: 'Login as Company',
    invalidCredentials: 'Invalid username or password.',
    logout: 'Logout',
    companies: 'Companies',
    complaints: 'Complaints & Suggestions',
    reports: 'Statistics',
    newComplaint: 'New Complaint',
    newCompany: 'New Company',
    editCompany: 'Edit Company',
    search: 'Search companies...',
    sector: 'Sector',
    allSectors: 'All Sectors',
    size: 'Size',
    allSizes: 'All Sizes',
    small: 'Small',
    medium: 'Medium',
    large: 'Large',
    nameEn: 'Name (English)',
    nameAr: 'Name (Arabic)',
    employeeCount: 'Employee Count',
    email: 'Email',
    phone: 'Phone',
    buildingNumber: 'Building Number',
    floorNumber: 'Floor Number',
    address: 'Address',
    save: 'Save',
    cancel: 'Cancel',
    edit: 'Edit',
    delete: 'Delete',
    actions: 'Actions',
    exportExcel: 'Export to Excel',
    createdAt: 'Created',
    confirmDelete: 'Are you sure you want to delete this?',
    noResults: 'No results found.',
    loading: 'Loading...',
    save_success: 'Saved successfully.',
    save_error: 'Something went wrong. Please check the form and try again.',
    passwordOptional: 'Password (leave blank to keep current)',
    passwordRequired: 'Password',
    complaintType: 'Complaint / Suggestion Type',
    description: 'Description',
    priority: 'Priority',
    low: 'Low',
    high: 'High',
    submit: 'Submit',
    status: 'Status',
    allStatuses: 'All Statuses',
    statusNew: 'New',
    statusInProgress: 'In Progress',
    statusResolved: 'Resolved',
    statusRejected: 'Rejected',
    updateStatus: 'Update Status',
    myComplaints: 'My Complaints',
    incomingComplaints: 'Incoming Complaints Box',
    submittedBy: 'Submitted by',
    totalCompanies: 'Total Companies',
    bySector: 'Companies by Sector',
    bySize: 'Companies by Size',
    back: 'Back',
    page: 'Page',
    of: 'of',
    next: 'Next',
    previous: 'Previous',
    deleteNote: 'Companies can only delete a complaint while it is still New.',
    requiredField: 'This field is required.',
    invalidEmail: 'Enter a valid email address.'
  },
  ar: {
    appTitle: 'مجمع نكسورا للأعمال',
    login: 'تسجيل الدخول',
    admin: 'المسؤول',
    company: 'الشركة',
    username: 'اسم المستخدم',
    password: 'كلمة المرور',
    loginAsAdmin: 'الدخول كمسؤول',
    loginAsCompany: 'الدخول كشركة',
    invalidCredentials: 'اسم المستخدم أو كلمة المرور غير صحيحة.',
    logout: 'تسجيل الخروج',
    companies: 'الشركات',
    complaints: 'الشكاوى والاقتراحات',
    reports: 'الإحصائيات',
    newComplaint: 'شكوى جديدة',
    newCompany: 'شركة جديدة',
    editCompany: 'تعديل الشركة',
    search: 'بحث عن شركات...',
    sector: 'القطاع',
    allSectors: 'جميع القطاعات',
    size: 'الحجم',
    allSizes: 'جميع الأحجام',
    small: 'صغيرة',
    medium: 'متوسطة',
    large: 'كبيرة',
    nameEn: 'الاسم (إنجليزي)',
    nameAr: 'الاسم (عربي)',
    employeeCount: 'عدد الموظفين',
    email: 'البريد الإلكتروني',
    phone: 'الهاتف',
    buildingNumber: 'رقم المبنى',
    floorNumber: 'رقم الطابق',
    address: 'العنوان',
    save: 'حفظ',
    cancel: 'إلغاء',
    edit: 'تعديل',
    delete: 'حذف',
    actions: 'إجراءات',
    exportExcel: 'تصدير إلى إكسل',
    createdAt: 'تاريخ الإنشاء',
    confirmDelete: 'هل أنت متأكد من الحذف؟',
    noResults: 'لا توجد نتائج.',
    loading: 'جارٍ التحميل...',
    save_success: 'تم الحفظ بنجاح.',
    save_error: 'حدث خطأ ما. يرجى التحقق من النموذج والمحاولة مرة أخرى.',
    passwordOptional: 'كلمة المرور (اتركها فارغة للاحتفاظ بالحالية)',
    passwordRequired: 'كلمة المرور',
    complaintType: 'نوع الشكوى / الاقتراح',
    description: 'الوصف',
    priority: 'الأولوية',
    low: 'منخفضة',
    high: 'عالية',
    submit: 'إرسال',
    status: 'الحالة',
    allStatuses: 'جميع الحالات',
    statusNew: 'جديدة',
    statusInProgress: 'قيد المعالجة',
    statusResolved: 'تم الحل',
    statusRejected: 'مرفوضة',
    updateStatus: 'تحديث الحالة',
    myComplaints: 'شكاواي',
    incomingComplaints: 'صندوق الشكاوى الواردة',
    submittedBy: 'مقدمة من',
    totalCompanies: 'إجمالي الشركات',
    bySector: 'الشركات حسب القطاع',
    bySize: 'الشركات حسب الحجم',
    back: 'رجوع',
    page: 'صفحة',
    of: 'من',
    next: 'التالي',
    previous: 'السابق',
    deleteNote: 'يمكن للشركة حذف الشكوى فقط طالما أنها لا تزال جديدة.',
    requiredField: 'هذا الحقل مطلوب.',
    invalidEmail: 'أدخل بريدًا إلكترونيًا صحيحًا.'
  }
};

@Injectable({ providedIn: 'root' })
export class I18nService {
  private readonly _lang = signal<Lang>(this.readInitialLang());

  readonly lang = this._lang.asReadonly();
  readonly dir = computed<'ltr' | 'rtl'>(() => (this._lang() === 'ar' ? 'rtl' : 'ltr'));

  constructor() {
    effect(() => {
      const lang = this._lang();
      document.documentElement.lang = lang;
      document.documentElement.dir = this.dir();
      document.body.style.fontFamily =
        lang === 'ar' ? "'Cairo', sans-serif" : "'Inter', sans-serif";
      localStorage.setItem(STORAGE_KEY, lang);
    });
  }

  setLang(lang: Lang): void {
    this._lang.set(lang);
  }

  toggle(): void {
    this._lang.set(this._lang() === 'en' ? 'ar' : 'en');
  }

  t(key: string): string {
    return TRANSLATIONS[this._lang()][key] ?? key;
  }

  // Picks whichever of the two bilingual fields matches the current language.
  pick(en: string | null | undefined, ar: string | null | undefined): string {
    const value = this._lang() === 'ar' ? ar : en;
    return value ?? en ?? ar ?? '';
  }

  private readInitialLang(): Lang {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'ar' || stored === 'en' ? stored : 'en';
  }
}
