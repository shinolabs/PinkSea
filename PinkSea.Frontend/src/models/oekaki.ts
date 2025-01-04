import type { Author } from '@/models/author'

export interface Oekaki {
  author: Author,
  image: string,
  at: string,
  cid: string,
  creationTime: Date,
  tags: string[] | undefined,
  nsfw: boolean,
  alt: string | undefined
}
