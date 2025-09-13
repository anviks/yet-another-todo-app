import type { AxiosRequestConfig } from "axios";
import apiClient from "./client";

async function apiRequest<T>(config: AxiosRequestConfig): Promise<T> {
  const response = await apiClient.request<T>(config);
  return response.data;
}

export default apiRequest;
