import { defineStore } from 'pinia';
import axios from 'axios';
import { ref } from 'vue';
import type { Ref } from 'vue';

interface TableEntity {
  partitionKey: string;
  rowKey: string;
  properties: Record<string, any>;
}

interface TableEntitiesResponse {
  entities: TableEntity[];
  continuationToken?: string;
}

export const useTableStorageStore = defineStore('tableStorage', () => {
  const tables = ref<string[]>([]);
  const currentTable = ref<string>('');
  const entities = ref<TableEntity[]>([]);
  const continuationToken = ref<string | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const fetchTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get('/api/TableStorage/tables');
      tables.value = response.data.tables;
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to fetch tables';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const createTable = async (tableName: string) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.post(`/api/TableStorage/tables/${tableName}`);
      await fetchTables();
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to create table';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const deleteTable = async (tableName: string) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.delete(`/api/TableStorage/tables/${tableName}`);
      await fetchTables();
      if (currentTable.value === tableName) {
        currentTable.value = '';
        entities.value = [];
      }
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to delete table';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const fetchEntities = async (tableName: string, filter?: string, pageSize?: number, token?: string) => {
    loading.value = true;
    error.value = null;
    try {
      const params = new URLSearchParams();
      if (filter) params.append('filter', filter);
      if (pageSize) params.append('pageSize', pageSize.toString());
      if (token) params.append('continuationToken', token);

      const response = await axios.get<TableEntitiesResponse>(
        `/api/TableStorage/tables/${tableName}/entities?${params.toString()}`
      );
      
      entities.value = response.data.entities;
      continuationToken.value = response.data.continuationToken || null;
      currentTable.value = tableName;
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to fetch entities';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const getEntity = async (tableName: string, partitionKey: string, rowKey: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `/api/TableStorage/tables/${tableName}/entities/${partitionKey}/${rowKey}`
      );
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to get entity';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const upsertEntity = async (tableName: string, entity: TableEntity) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.post(`/api/TableStorage/tables/${tableName}/entities`, entity);
      await fetchEntities(tableName);
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to upsert entity';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const deleteEntity = async (tableName: string, partitionKey: string, rowKey: string) => {
    loading.value = true;
    error.value = null;
    try {
      await axios.delete(
        `/api/TableStorage/tables/${tableName}/entities/${partitionKey}/${rowKey}`
      );
      await fetchEntities(tableName);
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to delete entity';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const queryEntities = async (tableName: string, filter: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get<TableEntitiesResponse>(
        `/api/TableStorage/tables/${tableName}/query?filter=${encodeURIComponent(filter)}`
      );
      entities.value = response.data.entities;
      continuationToken.value = null; // Query returns all results without pagination
    } catch (err: any) {
      error.value = err.response?.data || 'Failed to query entities';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  return {
    // State
    tables,
    currentTable,
    entities,
    continuationToken,
    loading,
    error,
    
    // Actions
    fetchTables,
    createTable,
    deleteTable,
    fetchEntities,
    getEntity,
    upsertEntity,
    deleteEntity,
    queryEntities
  };
});