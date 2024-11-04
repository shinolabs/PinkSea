import { defineStore } from 'pinia'

export const usePersistedStore = defineStore(
  'persistedStore',
  {
    state: () => {
      return {
        token: null as (string | null)
      }
    },
    persist: true
  });
