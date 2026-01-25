export interface SourceMetadata {
  displayName: string;
  websiteUrl: string;
  logoPath: string;
}

export const WEBSITE_SOURCE_CONFIG: Record<string, SourceMetadata> = {
  CareersInPoland: {
    displayName: 'Careers in Poland',
    websiteUrl: 'https://www.careersinpoland.com',
    logoPath: 'logos/careers-in-poland-logo.ico',
  },
  UsaJobs: {
    displayName: 'USAJOBS',
    websiteUrl: 'https://www.usajobs.gov',
    logoPath: 'logos/usajobs-logo.png',
  },
  Arbeitnow: {
    displayName: 'Arbeitnow',
    websiteUrl: 'https://www.arbeitnow.com',
    logoPath: 'logos/arbeitnow-logo.png',
  },
};
