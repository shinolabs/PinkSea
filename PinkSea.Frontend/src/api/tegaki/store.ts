import { defineStore } from 'pinia'

export const useTegakiViewStore = defineStore(
  'tegakiViewStore',
  {
    state: () => {
      return {
        pop: false
      }
    }
  });
