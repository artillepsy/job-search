
# Job Aggregator Website

This repository contains a small personal project -- centralized platform with job vacancies from various websites. The idea of making such a website is to have everything in one place, in uniform style and automatically updated. 
> **Note:** This is primarily a learning and portfolio project, to explore web scraping, API integration, and database management.

### Source Code
Here you can find the source code for backend, frontend, data scrapers and infrastructure.
1. Frontend – represents a static website;
2. API – utilized by frontend to retrieve available jobs;
3. Data scrapers – job units, needed to retrieve data from 3 websites: CareersInPoland, Arbeitnow, USAJobs;
4. Unit tests for API and data scrapers;
5. Infrastructure.

### Video Demonstration
Currently the website is inaccessible due to hosting costs on Azure, but you can check out the video demonstration: <a href="https://youtu.be/cSO9Mjrl9BY" target="_blank">YouTube</a>.

### Tech Stack

* **Frontend:** Angular framework, with help of PrimeNG library for faster prototyping.
* **Backend:** ASP .NET Core and EF Core.
* **Database:** PostgreSQL.
* **Hosting:** It's fully hosted on Microsoft Azure. SWA (Frontend), ACA (API, Data Scrapers), KeyVault (Secrets), DB.
* **CI:** GitHub Actions.
* **CD:** Azure Bicep.
