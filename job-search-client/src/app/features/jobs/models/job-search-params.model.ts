export class JobSearchParams {
  keywords?: string = '';
  location?: string = '';
  isRemote?: boolean;
  isSalaryVisible?: boolean;
  pageNumber: number = 1;
  pageSize: number = 20;
}

export const JOB_SEARCH_KEYS = Object.keys(new JobSearchParams()) as Array<keyof JobSearchParams>;
