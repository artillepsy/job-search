export class JobSearchParams {
  keywords?: string = '';
  location?: string = '';

  isRemoteOnly = false;
  withSalaryOnly = false;

  isCareersInPoland = true;
  isUsaJobs = true;
  isArbeitnow = true;

  page: number = 1;
  pageSize: number = 20; // is excluded from the result url
}

export const PAGE_STR_KEY = 'page';
export const JOB_SEARCH_KEYS_EXCLUDED_FROM_URL: Array<keyof JobSearchParams> = ['pageSize'];
export const JOB_SEARCH_KEYS = Object.keys(new JobSearchParams()) as Array<keyof JobSearchParams>;
