export interface JobInfo {
  id?: number;
  title: string;
  companyName: string;
  salaryMin?: number;
  salaryMax?: number;
  currency?: string;
  location: string;
  isRemote?: boolean;
  website: string;
  url: string;
  createdAt: string | Date;
}
