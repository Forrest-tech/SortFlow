const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000';

function getToken(): string | null {
  return localStorage.getItem('sortflow_token');
}

export function setToken(token: string): void {
  localStorage.setItem('sortflow_token', token);
}

export function clearToken(): void {
  localStorage.removeItem('sortflow_token');
}

export function isAuthenticated(): boolean {
  return !!getToken();
}

type Query = Record<string, string | number | boolean | undefined | null>;

function toQuery(q: Query): string {
  const p = new URLSearchParams();
  Object.entries(q).forEach(([k, v]) => { if (v != null && v !== '') p.set(k, String(v)); });
  const s = p.toString();
  return s ? `?${s}` : '';
}

async function fetchApi<T>(path: string, init?: RequestInit, params?: Query): Promise<T> {
  const url = `${API_BASE}${path}${params ? toQuery(params) : ''}`;
  const token = getToken();
  const res = await fetch(url, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...(token && { Authorization: `Bearer ${token}` }), ...(init?.headers as object) },
  });
  if (res.status === 401) { clearToken(); window.location.href = '/'; throw new Error('Unauthorized'); }
  if (!res.ok) throw new Error(await res.text().catch(() => res.statusText));
  return res.json();
}

export async function loginDev(): Promise<{ token: string }> {
  return fetchApi<{ token: string }>('/api/auth/token', { method: 'POST' });
}

export async function login(username: string, password: string): Promise<{ token: string }> {
  return fetchApi<{ token: string }>('/api/auth/login', { method: 'POST', body: JSON.stringify({ username, password }) });
}

// Dashboard
export interface DashboardSummary {
  itemsPerMinute: number;
  itemsPerHour: number;
  totalToday: number;
  successRate: number;
  totalEventsLastHour: number;
  successfulEventsLastHour: number;
  exceptionsLastHour: number;
  eventsByCategory?: Record<string, number>;
  generatedAtUtc: string;
}

export async function getDashboardSummary(p?: { windowMinutes?: number; timeFrom?: string; timeTo?: string }): Promise<DashboardSummary> {
  return fetchApi<DashboardSummary>('/api/dashboard/summary', undefined, p as Query);
}

// Events (paged)
export interface EventItem {
  id: string;
  itemId: string;
  postalCode: string;
  processedAtUtc: string;
  isSuccessful: boolean;
  exceptionType: string | null;
  stationName: string;
  zoneName: string;
}

export interface PagedResult<T> { items: T[]; totalCount: number; page: number; pageSize: number; }

export async function getEvents(p?: { page?: number; pageSize?: number; sortBy?: string; sortDir?: string; zoneId?: string; stationId?: string; timeFrom?: string; timeTo?: string; exceptionType?: string; result?: string }): Promise<PagedResult<EventItem>> {
  return fetchApi<PagedResult<EventItem>>('/api/events', undefined, p as Query);
}

// Exceptions (paged)
export interface ExceptionItem {
  id: string;
  exceptionType: string;
  details: string;
  itemId: string;
  stationName: string;
  createdAtUtc: string;
}

export async function getExceptions(p?: { page?: number; pageSize?: number; sortBy?: string; sortDir?: string; zoneId?: string; stationId?: string; timeFrom?: string; timeTo?: string; exceptionType?: string }): Promise<PagedResult<ExceptionItem>> {
  return fetchApi<PagedResult<ExceptionItem>>('/api/exceptions', undefined, p as Query);
}

// History
export interface HistoryItem { period: string; total: number; successful: number; exceptions: number; successRate: number; }

export async function getHistory(p?: { groupBy?: string; from?: string; to?: string }): Promise<HistoryItem[]> {
  return fetchApi<HistoryItem[]>('/api/history', undefined, p as Query);
}

export async function getHistoryExportCsv(from?: string, to?: string): Promise<Blob> {
  const p = new URLSearchParams();
  if (from) p.set('from', from); if (to) p.set('to', to);
  const q = p.toString();
  const token = getToken();
  const res = await fetch(`${API_BASE}/api/history/export${q ? '?' + q : ''}`, { headers: token ? { Authorization: `Bearer ${token}` } : {} });
  if (!res.ok) throw new Error(await res.text());
  return res.blob();
}

// Settings
export interface AppSettings {
  id: string;
  generatorRatePerSecond: number;
  addressMismatchProbability: number;
  invalidPostalProbability: number;
  damagedLabelProbability: number;
  dashboardWindowMinutes: number;
  enableModules: string;
  updatedAt: string;
}

export async function getSettings(): Promise<AppSettings | null> {
  return fetchApi<AppSettings | null>('/api/settings').catch(() => null);
}

export async function putSettings(s: Partial<AppSettings>): Promise<AppSettings> {
  return fetchApi<AppSettings>('/api/settings', { method: 'PUT', body: JSON.stringify(s) });
}

// Zones
export interface ZoneItem { id: string; name: string; code: string; isActive: boolean; stationCount: number; }

export async function getZones(): Promise<ZoneItem[]> {
  return fetchApi<ZoneItem[]>('/api/zones');
}

export async function getZone(id: string): Promise<ZoneItem | null> {
  return fetchApi<ZoneItem | null>(`/api/zones/${id}`).catch(() => null);
}

export async function createZone(z: { name: string; code: string; isActive?: boolean }): Promise<ZoneItem> {
  return fetchApi<ZoneItem>('/api/zones', { method: 'POST', body: JSON.stringify(z) });
}

export async function updateZone(id: string, z: { name: string; code: string; isActive?: boolean }): Promise<ZoneItem> {
  return fetchApi<ZoneItem>(`/api/zones/${id}`, { method: 'PUT', body: JSON.stringify(z) });
}

export async function deleteZone(id: string): Promise<void> {
  const token = getToken();
  const res = await fetch(`${API_BASE}/api/zones/${id}`, { method: 'DELETE', headers: token ? { Authorization: `Bearer ${token}` } : {} });
  if (!res.ok) throw new Error(await res.text());
}

// Stations
export interface StationItem { id: string; name: string; stationCode: string; isActive: boolean; zoneName: string; zoneId: string; }

export async function getStations(): Promise<StationItem[]> {
  return fetchApi<StationItem[]>('/api/stations');
}

export async function getStation(id: string): Promise<StationItem | null> {
  return fetchApi<StationItem | null>(`/api/stations/${id}`).catch(() => null);
}

export async function createStation(s: { name: string; stationCode?: string; isActive?: boolean; zoneId: string }): Promise<StationItem> {
  return fetchApi<StationItem>('/api/stations', { method: 'POST', body: JSON.stringify(s) });
}

export async function updateStation(id: string, s: { name: string; stationCode?: string; isActive?: boolean; zoneId: string }): Promise<StationItem> {
  return fetchApi<StationItem>(`/api/stations/${id}`, { method: 'PUT', body: JSON.stringify(s) });
}

export async function deleteStation(id: string): Promise<void> {
  const token = getToken();
  const res = await fetch(`${API_BASE}/api/stations/${id}`, { method: 'DELETE', headers: token ? { Authorization: `Bearer ${token}` } : {} });
  if (!res.ok) throw new Error(await res.text());
}

// Admin
export async function generatorStart(): Promise<{ status: string }> {
  return fetchApi<{ status: string }>('/api/admin/generator/start', { method: 'POST' });
}

export async function generatorStop(): Promise<{ status: string }> {
  return fetchApi<{ status: string }>('/api/admin/generator/stop', { method: 'POST' });
}

export async function generatorStatus(): Promise<{ isRunning: boolean; ratePerSecond: number }> {
  return fetchApi<{ isRunning: boolean; ratePerSecond: number }>('/api/admin/generator/status');
}

// SignalR
export function getSignalRHubUrl(): string { return `${API_BASE}/hubs/dashboard`; }
export function getTokenForSignalR(): string | null { return getToken(); }
