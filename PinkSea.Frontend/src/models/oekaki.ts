export interface Oekaki {
  did: string,
  handle: string,
  image: string,
  at: string,
  cid: string,
  creationTime: Date,
  tags: string[] | undefined,
  nsfw: boolean,
  alt: string | undefined
}
