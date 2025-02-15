<template>
  <div class="container mx-auto p-4">
    <div class="mb-8">
      <h1 class="text-2xl font-bold mb-4">Azure Table Storage Management</h1>
      <div class="flex gap-4 mb-4">
        <div class="flex-1">
          <input
            v-model="newTableName"
            type="text"
            placeholder="Enter table name"
            class="w-full px-4 py-2 border rounded focus:outline-none focus:border-blue-500"
          />
        </div>
        <button
          @click="createTable"
          class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
          :disabled="store.loading"
        >
          Create Table
        </button>
      </div>

      <!-- Tables List -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          v-for="table in store.tables"
          :key="table"
          class="border rounded p-4 flex justify-between items-center"
          :class="{ 'bg-blue-50': store.currentTable === table }"
        >
          <span class="font-medium">{{ table }}</span>
          <div class="flex gap-2">
            <button
              @click="selectTable(table)"
              class="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
            >
              View
            </button>
            <button
              @click="confirmDeleteTable(table)"
              class="px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600 text-sm"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Entity Management -->
    <div v-if="store.currentTable" class="mb-8">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-bold">Entities in {{ store.currentTable }}</h2>
        <button
          @click="showCreateEntity = true"
          class="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600"
        >
          Add Entity
        </button>
      </div>

      <!-- Search/Filter -->
      <div class="mb-4">
        <input
          v-model="filter"
          type="text"
          placeholder="Filter entities (OData format)"
          class="w-full px-4 py-2 border rounded focus:outline-none focus:border-blue-500"
          @keyup.enter="applyFilter"
        />
      </div>

      <!-- Entities Table -->
      <div class="overflow-x-auto">
        <table class="min-w-full border-collapse border">
          <thead>
            <tr class="bg-gray-100">
              <th class="border p-2">Partition Key</th>
              <th class="border p-2">Row Key</th>
              <th class="border p-2">Properties</th>
              <th class="border p-2">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entity in store.entities" :key="entity.partitionKey + '_' + entity.rowKey">
              <td class="border p-2">{{ entity.partitionKey }}</td>
              <td class="border p-2">{{ entity.rowKey }}</td>
              <td class="border p-2">
                <pre class="whitespace-pre-wrap">{{ JSON.stringify(entity.properties, null, 2) }}</pre>
              </td>
              <td class="border p-2">
                <div class="flex gap-2">
                  <button
                    @click="editEntity(entity)"
                    class="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-sm"
                  >
                    Edit
                  </button>
                  <button
                    @click="confirmDeleteEntity(entity)"
                    class="px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600 text-sm"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="store.continuationToken" class="mt-4 flex justify-center">
        <button
          @click="loadMore"
          class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
          :disabled="store.loading"
        >
          Load More
        </button>
      </div>
    </div>

    <!-- Entity Form Modal -->
    <div v-if="showCreateEntity || entityToEdit" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-white rounded-lg p-6 max-w-2xl w-full mx-4">
        <h3 class="text-xl font-bold mb-4">{{ entityToEdit ? 'Edit Entity' : 'Create Entity' }}</h3>
        <div class="space-y-4">
          <div>
            <label class="block mb-1">Partition Key</label>
            <input
              v-model="entityForm.partitionKey"
              type="text"
              class="w-full px-4 py-2 border rounded"
              :disabled="!!entityToEdit"
            />
          </div>
          <div>
            <label class="block mb-1">Row Key</label>
            <input
              v-model="entityForm.rowKey"
              type="text"
              class="w-full px-4 py-2 border rounded"
              :disabled="!!entityToEdit"
            />
          </div>
          <div>
            <label class="block mb-1">Properties (JSON)</label>
            <textarea
              v-model="entityForm.propertiesJson"
              class="w-full px-4 py-2 border rounded h-40 font-mono"
              @input="validateJson"
            ></textarea>
            <p v-if="jsonError" class="text-red-500 text-sm mt-1">{{ jsonError }}</p>
          </div>
        </div>
        <div class="flex justify-end gap-4 mt-6">
          <button
            @click="closeEntityForm"
            class="px-4 py-2 border rounded hover:bg-gray-100"
          >
            Cancel
          </button>
          <button
            @click="saveEntity"
            class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
            :disabled="store.loading || !!jsonError"
          >
            Save
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useTableStorageStore } from '../stores/tableStorage';

const store = useTableStorageStore();

const newTableName = ref('');
const filter = ref('');
const showCreateEntity = ref(false);
const entityToEdit = ref<any>(null);
const jsonError = ref<string | null>(null);
const entityForm = ref({
  partitionKey: '',
  rowKey: '',
  propertiesJson: '{}'
});

onMounted(async () => {
  try {
    await store.fetchTables();
  } catch (error) {
    console.error('Failed to fetch tables:', error);
  }
});

const createTable = async () => {
  if (!newTableName.value) return;
  try {
    await store.createTable(newTableName.value);
    newTableName.value = '';
  } catch (error) {
    console.error('Failed to create table:', error);
  }
};

const selectTable = async (tableName: string) => {
  try {
    await store.fetchEntities(tableName);
  } catch (error) {
    console.error('Failed to fetch entities:', error);
  }
};

const confirmDeleteTable = async (tableName: string) => {
  if (confirm(`Are you sure you want to delete table "${tableName}"?`)) {
    try {
      await store.deleteTable(tableName);
    } catch (error) {
      console.error('Failed to delete table:', error);
    }
  }
};

const applyFilter = async () => {
  if (!store.currentTable) return;
  try {
    if (filter.value) {
      await store.queryEntities(store.currentTable, filter.value);
    } else {
      await store.fetchEntities(store.currentTable);
    }
  } catch (error) {
    console.error('Failed to apply filter:', error);
  }
};

const loadMore = async () => {
  if (!store.currentTable || !store.continuationToken) return;
  try {
    await store.fetchEntities(store.currentTable, filter.value, undefined, store.continuationToken);
  } catch (error) {
    console.error('Failed to load more entities:', error);
  }
};

const validateJson = () => {
  try {
    JSON.parse(entityForm.value.propertiesJson);
    jsonError.value = null;
  } catch (error) {
    jsonError.value = 'Invalid JSON format';
  }
};

const editEntity = (entity: any) => {
  entityToEdit.value = entity;
  entityForm.value = {
    partitionKey: entity.partitionKey,
    rowKey: entity.rowKey,
    propertiesJson: JSON.stringify(entity.properties, null, 2)
  };
  showCreateEntity.value = true;
};

const saveEntity = async () => {
  if (!store.currentTable || jsonError.value) return;
  
  try {
    const properties = JSON.parse(entityForm.value.propertiesJson);
    await store.upsertEntity(store.currentTable, {
      partitionKey: entityForm.value.partitionKey,
      rowKey: entityForm.value.rowKey,
      properties
    });
    closeEntityForm();
  } catch (error) {
    console.error('Failed to save entity:', error);
  }
};

const confirmDeleteEntity = async (entity: any) => {
  if (!store.currentTable) return;
  
  if (confirm(`Are you sure you want to delete this entity?`)) {
    try {
      await store.deleteEntity(store.currentTable, entity.partitionKey, entity.rowKey);
    } catch (error) {
      console.error('Failed to delete entity:', error);
    }
  }
};

const closeEntityForm = () => {
  showCreateEntity.value = false;
  entityToEdit.value = null;
  entityForm.value = {
    partitionKey: '',
    rowKey: '',
    propertiesJson: '{}'
  };
  jsonError.value = null;
};
</script>