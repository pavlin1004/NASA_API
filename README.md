#  NASA REST API

This ASP.NET Core MVC application connects to NASA's Near-Earth Object Web Service (NeoWs).

##  Features

- Date-based asteroid search using NASA's REST API
- Table display of relevant asteroid data
- Excel export functionality
- List Astronomy picture of the day (APOD) with support for past periods

---

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/pavlin1004/NASA_API.git
cd NASA_API
```
### Get a Key
Go to https://api.nasa.gov and sign up for a free API key.

### Configure the API Key
#### ✅ Option 1: Use User Secrets (Recommended for development)
```bash
dotnet user-secrets init
dotnet user-secrets set "NASA:ApiKey" "your_api_key_here"
```
#### ✅ Option 2: Update appsettings.json
```
"NASA": {
    "ApiKey": "your_api_key_here"
  }

