# EcoSphere – Biodiversity Observation & Management Platform

EcoSphere is a web-based biodiversity observation and management platform developed to support nature conservation efforts. The platform enables users to record, visualize, and manage species observations through an interactive map and data management interface.

## 🌱 Project Purpose

Prior to EcoSphere, nature conservation staff in government agencies and NGOs were managing observation data manually in Excel sheets. This method was time-consuming, error-prone, and lacked spatial context. EcoSphere was built to address these limitations.

With EcoSphere:
- Observations can be recorded and visualized on an interactive **OpenLayers** map.
- Filtering by species, region, endemic status, and date is possible.
- Verified data can be exported as **filtered Excel or CSV files**.
- Role-based access control ensures secure data handling.
- Admins and experts can verify or reject submitted observations.
- Endemic species are hidden from unauthorized users to prevent risk of harm.
- A dynamic dashboard summarizes total species, plant/animal ratio, and observation count by region.

## 👨‍💻 Technologies Used

- **ASP.NET Core MVC** – Backend Framework  
- **Entity Framework Core** – Database ORM  
- **MSSQL / Azure SQL** – Cloud-hosted Relational Database  
- **OpenLayers** – Interactive map visualization  
- **Bootstrap 5** – Responsive frontend design  
- **SweetAlert2** – User-friendly notifications  
- **Docker** – Containerized deployment on VPS  
- **Role-based Authorization** – Admin, Expert, Observer, Volunteer roles  
- **Caching** – Performance optimization for large dataset queries  

## 🌍 Key Features

- **Map-based Visualization:**
  - View observations by clicking on provinces/districts
  - Toggle between map layers (standard/satellite)
  - Display markers only for verified observations (status = 1)

- **Observation Submission:**
  - Volunteers can submit observations via a form with image upload
  - Observations require expert/admin verification before visibility

- **Species Management:**
  - Admin panel for adding, editing, and deleting species
  - Species categorized by kingdom (Animalia / Plantae)

- **Downloadable Reports:**
  - Export data to Excel or CSV with dynamic filtering
  - Observer role is restricted from downloading

- **Endemic Species Protection:**
  - Endemic records (status = 1) are visible only to Admin and Expert roles

## 🔒 Roles & Permissions

| Role       | Add Obs. | View Map | View Endemics | Download | Approve/Delete |
|------------|----------|----------|----------------|----------|----------------|
| Volunteer  | ✅       | ✅       | ❌             | ❌       | ❌             |
| Observer   | ✅       | ✅       | ❌             | ❌       | ✅             |
| Expert     | ✅       | ✅       | ✅             | ✅       | ✅             |
| Admin      | ✅       | ✅       | ✅             | ✅       | ✅             |

## 🚀 Deployment

EcoSphere is deployed on a VPS using Docker and connects to an **Azure SQL** database instance. Static files are served securely with appropriate MIME types, and all configurations (firewall, reverse proxy, domain) are managed to ensure smooth access via `ecosphere.org.tr`.

## 📊 Screenshots (optional)

_Add here if needed: form view, map page, species management panel, download UI_

## 📁 Project Structure

- `/Controllers` – Backend logic
- `/Views` – Razor pages for frontend
- `/wwwroot/uploads` – Image files
- `/Models` – Database models
- `/site.js` – Frontend interactivity (map, counters, UI)

## 🤝 Acknowledgments

We thank the nature conservation professionals who provided real-world insights during the design phase. This project aims to make biodiversity tracking more efficient, accessible, and secure.

---

Feel free to contribute, suggest improvements, or reach out if you're working on similar conservation tech!
