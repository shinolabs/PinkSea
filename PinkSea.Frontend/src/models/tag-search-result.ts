import type { Oekaki } from '@/models/oekaki'

export interface TagSearchResult {
  tag: string,
  oekaki: Oekaki,
  count: number
}
