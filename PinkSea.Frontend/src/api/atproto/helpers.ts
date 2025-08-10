import type { Oekaki } from '@/models/oekaki'
import { usePersistedStore } from '@/state/store'

const uriRegex = /^at:\/\/did:[a-zA-Z0-9:.-]+\/[a-zA-Z0-9.]+\/([a-zA-Z0-9]+)$/;

export const getRecordKeyFromAtUri = (uri: string) => {
  const match = uri.match(uriRegex);
  if (!match) {
    return null;
  } else {
    return match[1];
  }
}

export const buildOekakiUrlFromOekakiObject = (oekaki: Oekaki) => {
  const rkey = getRecordKeyFromAtUri(oekaki.at)
  return `/${oekaki.author.did}/oekaki/${rkey}`
}

export const formatDate = (date: Date) => {
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }

  const persistedStore = usePersistedStore()

  return new Date(date)
    .toLocaleTimeString(persistedStore.lang ?? "en", options)
}
