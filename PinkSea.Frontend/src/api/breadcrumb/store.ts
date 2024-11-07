import { defineStore } from 'pinia'

export const useBreadcrumbBarStore = defineStore(
  'breadcrumbBarStore',
  {
    state: () => {
      return {
        crumbs: [] as string[],
        pop: false,
        push: false
      }
    }
  });
