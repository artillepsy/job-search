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
    websiteUrl: 'https://developer.usajobs.gov/',
    logoPath: 'logos/usajobs-logo.png',
  },
};
