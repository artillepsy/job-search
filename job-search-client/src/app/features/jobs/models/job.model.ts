export interface Job {
  id?: number;
  title: string;
  companyName: string;
  location: string;
  salaryMin?: number;
  salaryMax?: number;
  currency?: string;
  createdAt: string | Date;
}
