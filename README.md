# SortFlow

Logistics sorting dashboard: .NET 8 API + React (Vite) frontend.

| Folder         | Description                          |
|----------------|--------------------------------------|
| **sortflow-api**  | .NET 8 Web API, PostgreSQL, JWT, SignalR |
| **sortflow-web**  | React frontend (Vite, TypeScript)       |

## Run locally

1. **PostgreSQL** – running on `localhost:5432` (or adjust `appsettings.json`).
2. **API:** `cd sortflow-api/src/SortFlow.Api` → `dotnet run` → http://localhost:5000
3. **Web:** `cd sortflow-web` → `install-and-dev.cmd` or `npm run dev` → http://localhost:3000

See `sortflow-api/README.md` and `sortflow-web/README.md` for details.

---

## Upload to GitHub

**1. Install Git** (if `git` is not recognized):  
[https://git-scm.com/download/win](https://git-scm.com/download/win) — then restart the terminal.

**2. In Command Prompt or PowerShell, from this folder (`SortFlow`):**

```bash
cd C:\Users\Jadenfly\Documents\SortFlow

git init
git add .
git commit -m "Initial commit: SortFlow API and web app"
```

**3. Create a new repo on GitHub**

- Go to [github.com/new](https://github.com/new)
- Name it e.g. `SortFlow` (or `sortflow`)
- Leave “Add a README” **unchecked** (you already have one)
- Create the repository

**4. Connect and push**

Replace `YOUR_USERNAME` and `YOUR_REPO` with your GitHub user and repo name:

```bash
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git
git branch -M main
git push -u origin main
```

If GitHub prompts for login, use a [Personal Access Token](https://github.com/settings/tokens) as the password when using HTTPS.

---

**Before making the repo public:**  
`sortflow-api/src/SortFlow.Api/appsettings.json` contains a DB password and a dev JWT key. Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables in production, and consider replacing or removing secrets before publishing.
