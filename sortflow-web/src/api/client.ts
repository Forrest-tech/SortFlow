const API_BASE = 'http://localhost:5000';

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

async function fetchApi<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getToken();
  const res = await fetch(`${API_BASE}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...init?.headers,
    },
  });
  if (res.status === 401) {
    clearToken();
    window.location.href = '/';
    throw new Error('Unauthorized');
  }
  if (!res.ok) throw new Error(await res.text().catch(() => res.statusText));
  return res.json();
}

export async function login(): Promise<{ token: string }> {
  return fetchApi<{ token: string }>('/api/auth/token', { method: 'POST' });
}

export interface DashboardSummary {
  totalEventsLastHour: number;
  successfulEventsLastHour: number;
  exceptionsLastHour: number;
  itemsPerMinute: number;
  itemsPerHour: number;
  generatedAtUtc: string;
}

export async function getDashboardSummary(): Promise<DashboardSummary> {
  return fetchApi<DashboardSummary>('/api/dashboard/summary');
}

export interface ExceptionItem {
  id: string;
  exceptionType: string;
  details: string;
  itemId: string;
  stationName: string;
  createdAtUtc: string;
}

export async function getExceptions(limit = 25): Promise<ExceptionItem[]> {
  return fetchApi<ExceptionItem[]>(`/api/exceptions?limit=${limit}`);
}

export function getSignalRHubUrl(): string {
  return `${API_BASE}/hubs/dashboard`;
}

export function getTokenForSignalR(): string | null {
  return getToken();
}
