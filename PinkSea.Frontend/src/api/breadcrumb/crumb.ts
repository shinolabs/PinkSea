import type CrumbI18nData from '@/api/breadcrumb/crumb-i18n-data'

export default interface Crumb {
  path: string,
  i18n: CrumbI18nData
}
