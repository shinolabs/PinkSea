import { defineStore } from 'pinia'
import { xrpc } from '@/api/atproto/client';
import { usePersistedStore } from './store';
import type { Preferences } from '@/models/preferences';
import { UsernameDisplayType } from '@/models/username-display-type';
import { watch } from 'vue';

type FrontendPreferences = {
    editorDarkMode: boolean,
    blurNsfw: boolean,
    hideNsfw: boolean,
    usernameDisplayType: UsernameDisplayType
}

export const usePdsPreferencesStore = defineStore(
    'preferencesStore',
    {
        state: () => {
            return {
                editorDarkMode: false,
                blurNsfw: true,
                hideNsfw: false,
                usernameDisplayType: UsernameDisplayType.NicknameWithHandle
            } as FrontendPreferences
        },
        actions: {
            async fetch() {
                const persistedStore = usePersistedStore()
                const { data } = await xrpc.get("com.shinolabs.pinksea.getPreferences", {
                    params: {},
                    headers: {
                        "Authorization": `Bearer ${persistedStore.token}`
                    }
                });

                this.parse({ values: data.preferences })
            },

            parse(preferences: Preferences) {
                preferences.values.forEach(({ key, value }) => {
                    const strippedKey = key.replace("frontend.", "")
                    if (strippedKey in this) {
                        const typedKey = strippedKey as keyof FrontendPreferences;
                        let parsedValue: any = value;

                        if (this[typedKey] === true || this[typedKey] === false) {
                            parsedValue = value === 'true';
                        } else if (typeof this[typedKey] === 'number') {
                            parsedValue = Number(value);
                        } else {
                            parsedValue = value;
                        }

                        (this as any)[strippedKey] = parsedValue;
                    }
                });
            },

            async set<K extends keyof FrontendPreferences>(key: K, value: FrontendPreferences[K]) {
                (this as any)[key] = value

                const persistedStore = usePersistedStore()
                await xrpc.call("com.shinolabs.pinksea.putPreference", {
                    data: {
                        key: `frontend.${key}`,
                        value: `${value}`
                    },
                    headers: {
                        "Authorization": `Bearer ${persistedStore.token}`
                    }
                });
            },

            autoSync() {
                watch(
                    () => JSON.parse(JSON.stringify(this.$state)),
                    (newState, oldState) => {
                        for (const key in newState) {
                            if (newState[key] !== oldState?.[key]) {
                                this.set(key as keyof typeof newState, newState[key])
                            }
                        }
                    },
                    { deep: true }
                )
            }
        }
    });