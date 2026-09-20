export interface SectorStat {
  sectorNameEn: string;
  sectorNameAr: string;
  companyCount: number;
  percentageOfTotal: number;
}

export interface SizeStat {
  size: string;
  companyCount: number;
  percentageOfTotal: number;
}

export interface CompanyStatisticsReport {
  totalCompanies: number;
  bySector: SectorStat[];
  bySize: SizeStat[];
}
