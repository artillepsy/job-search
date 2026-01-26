export interface SourceMetadata {
  displayName: string;
  websiteUrl: string;
  logoPath: string;
}

export const WEBSITE_SOURCE_CONFIG: Record<string, SourceMetadata> = {
  CareersInPoland: {
    displayName: 'Careers in Poland',
    websiteUrl: 'https://www.careersinpoland.com',
    logoPath: 'websites/careers-in-poland-logo.ico',
  },
  UsaJobs: {
    displayName: 'USAJOBS',
    websiteUrl: 'https://www.usajobs.gov',
    logoPath: 'websites/usajobs-logo.png',
  },
  Arbeitnow: {
    displayName: 'Arbeitnow',
    websiteUrl: 'https://www.arbeitnow.com',
    logoPath: 'websites/arbeitnow-logo.png',
  },
};
