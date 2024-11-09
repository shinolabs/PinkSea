import { defineStore } from 'pinia'

export const usePersistedStore = defineStore(
  'persistedStore',
  {
    state: () => {
      return {
        token: null as (string | null),
        hideNsfw: true
      }
    },
    persist: true
  });

export const useIdentityStore = defineStore(
  'identityStore',
  {
    state: () => {
        return {
          did: null as (string | null),
          handle: null as (string | null)
        }
    }
  });
