import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import type { CreateEventRequest, CreateEventResponse } from "../types/event";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:8090";

const DEMO_USERNAME = import.meta.env.VITE_DEMO_USERNAME ?? "admin";
const DEMO_ROLE = import.meta.env.VITE_DEMO_ROLE ?? "Admin";

const STATIC_TOKEN = import.meta.env.VITE_DEMO_TOKEN ?? "";

interface CachedToken {
  value: string;

  usableUntil: number;
}

const SAFETY_MARGIN_MS = 60_000;

let cache: CachedToken | null = null;

let inFlight: Promise<string> | null = null;

async function requestFreshToken(): Promise<string> {
  const { data } = await axios.post<{ accessToken: string; expiresAt: string }>(
    `${API_URL}/auth/token`,
    { username: DEMO_USERNAME, role: DEMO_ROLE }
  );
  cache = {
    value: data.accessToken,
    usableUntil: new Date(data.expiresAt).getTime() - SAFETY_MARGIN_MS,
  };
  return data.accessToken;
}

async function getToken(): Promise<string> {
  if (STATIC_TOKEN) return STATIC_TOKEN;
  if (cache && Date.now() < cache.usableUntil) return cache.value;
  if (!inFlight) {
    inFlight = requestFreshToken().finally(() => {
      inFlight = null;
    });
  }
  return inFlight;
}

const eventClient = axios.create({ baseURL: API_URL });

eventClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    config.headers.Authorization = `Bearer ${await getToken()}`;
    return config;
  }
);

eventClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const config = error.config as
      | (InternalAxiosRequestConfig & { _retried?: boolean })
      | undefined;

    if (
      error.response?.status === 401 &&
      config &&
      !config._retried &&
      !STATIC_TOKEN
    ) {
      config._retried = true;
      cache = null;
      config.headers.Authorization = `Bearer ${await getToken()}`;
      return eventClient.request(config);
    }

    return Promise.reject(error);
  }
);

export async function createEvent(
  payload: CreateEventRequest
): Promise<CreateEventResponse> {
  const { data } = await eventClient.post<CreateEventResponse>(
    "/events",
    payload
  );
  return data;
}

export default eventClient;
