# Job Search - Job Aggregator

Welcome to **Job Search**, a centralized platform designed to help job seekers discover opportunities across various industries in one streamlined interface.

## Live Demo
Test the application and see it in action here: **[Website](https://salmon-meadow-0d94e0103.1.azurestaticapps.net)**. This demo is hosted on Azure Static Web Apps.
> **Note:** The demo is currently in development and may not be fully functional. Please wait a few seconds for the VM hosting the website to start.

## About the Project
This website is a **job posting aggregator**. Its main mission is to simplify the job search process by collecting listings from multiple reputable sources and presenting them in a single, searchable database.

> **Note:** This is primarily a **learning and portfolio project**. It is currently in active development to explore web scraping, API integration, and database management.

---

## Tech Stack

### Frontend
* **Angular:** Framework used for building the modern, reactive user interface.
* **PrimeNG:** UI component library used for professional layouts and high-quality widgets.

### Backend
The backend is built with **ASP.NET Core** and **Entity Framework Core**, consisting of:
* **API:** The core server providing endpoints for the web application.
* **Data Scrapers:** Scheduled background jobs that programmatically retrieve data from public APIs such as USAJobs, Arbeitnow, and CareersInPoland.

### Database
* **PostgreSQL:** A robust relational database used to store job listings, company data, and search metadata.

---

## DevOps & Hosting
The entire infrastructure is automated and hosted in the cloud:

* **Hosting:** The solution is fully hosted on **Microsoft Azure**.
* **CI (Continuous Integration):** **GitHub Actions** handles automated builds and testing.
* **CD (Continuous Deployment):** Managed via **Azure Bicep** (Infrastructure as Code) to ensure consistent environment provisioning.

---

## Local Development
To ensure a consistent environment across development and production, this project supports **Docker**.

Docker allows for spinning up the API, the PostgreSQL database, and the Scrapers locally for testing and development without requiring the full dependency chain on the host machine.

---

## Fair Use & Legal Compliance
This project aims to be a "good citizen" of the web:
- **Backlinks:** Every job posting redirects traffic directly to the original poster's website.
- **Robots.txt:** Instructions in `robots.txt` are strictly followed, and crawling occurs at low frequencies to avoid server strain.
- **Public Data:** Only publicly accessible information is aggregated.
- **Removal Requests:** Site owners wishing to have listings removed can send a request to **artillepsy@gmail.com** for immediate processing.