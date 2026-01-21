export interface JobSearchParams {
  jobTitle?: string;
  country?: string;
  isRemote?: boolean;
  isSalaryVisible?: boolean;
  pageNumber: number;
  pageSize: number;
}
