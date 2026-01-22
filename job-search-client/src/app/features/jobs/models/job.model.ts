export interface Job {
  id?: number;
  title: string;
  companyName: string;
  location: string;
  isRemote?: boolean;
  salaryMin?: number;
  salaryMax?: number;
  currency?: string;
  createdAt: string | Date;
}
