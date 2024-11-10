export interface Oekaki {
  oekakiRecordKey: string,
  authorDid: string,
  authorHandle: string,
  imageLink: string,
  atProtoLink: string,
  oekakiCid: string,
  creationTime: Date,
  tags: string[] | undefined,
  nsfw: boolean,
  alt: string | undefined
}
