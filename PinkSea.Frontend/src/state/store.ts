import { UsernameDisplayType } from '@/models/username-display-type';
import { defineStore } from 'pinia'

export const usePersistedStore = defineStore(
  'persistedStore',
  {
    state: () => {
      return {
        token: null as (string | null),
        blurNsfw: true,
        hideNsfw: false,
        usernameDisplayType: UsernameDisplayType.NicknameWithHandle,

        lang: null as (string | null)
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

export const useImageStore = defineStore(
  'imageStore',
  {
    state: () => {
      return {
        lastDoneImage: null as (string | null),
        restartPainting: false,
        lastUploadErrored: false,

        lastDoneReply: null as (string | null),
        lastReplyErrored: false,
        lastReplyId: ""
      }
    },
    persist: true
  });
