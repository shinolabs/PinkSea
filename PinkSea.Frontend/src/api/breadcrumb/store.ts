import { defineStore } from 'pinia'
import type Crumb from '@/api/breadcrumb/crumb'

export const useBreadcrumbBarStore = defineStore(
  'breadcrumbBarStore',
  {
    state: () => {
      return {
        crumbs: [] as Crumb[],
        pop: false,
        push: false
      }
    }
  });
