export interface Job {
  id?: number;
  title: string;
  companyName: string;
  location: string;
  isSalaryVisible: boolean;
  salary?: number;
  createdAt: Date;
}
