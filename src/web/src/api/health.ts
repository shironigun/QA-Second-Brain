import { apiGet } from './client';

export interface Health {
  status: 'ok' | 'degraded';
  db: 'ok' | 'unreachable';
}

export const getHealth = (signal?: AbortSignal) => apiGet<Health>('/health', signal);
