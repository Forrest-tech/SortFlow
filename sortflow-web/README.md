# SortFlow Web

React frontend for the SortFlow logistics dashboard.

## Prerequisites

- Node 18+
- SortFlow API running at `http://localhost:5000` (see `../sortflow-api`)

## Run

```bash
npm install
npm run dev
```

Then open **http://localhost:3000**.

### If `npm` fails in PowerShell (scripts disabled)

PowerShell’s execution policy can block `npm.ps1`. Use either:

1. **Batch script (easiest)**  
   Double‑click `install-and-dev.cmd` in this folder, or run it from Command Prompt. It uses `npm.cmd` and does `npm install` then `npm run dev`.  
   If Node is not in `C:\Program Files\nodejs`, edit the `NPM=` line in the script.

2. **Command Prompt**  
   Open **cmd** (not PowerShell), `cd` to this folder, then run `npm install` and `npm run dev`. Cmd uses `npm.cmd`, which is not restricted.

3. **Allow scripts in PowerShell (current user)**  
   In PowerShell (as yourself):  
   `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`  
   Then use `npm` as usual.

## Flow

1. **Login** – "Get dev token & sign in" calls `POST /api/auth/token` and stores the JWT.
2. **Dashboard** – Shows `GET /api/dashboard/summary` and subscribes to SignalR `sortingEventReceived` for live updates.
3. **Events** – Lists `GET /api/events` (sorting activity log).
4. **Exceptions** – Lists `GET /api/exceptions`.
5. **Zones** – Lists `GET /api/zones`.
6. **Stations** – Lists `GET /api/stations`.

## Build

```bash
npm run build
```

Output is in `dist/`.
