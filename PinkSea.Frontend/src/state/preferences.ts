import { defineStore } from 'pinia'
import { xrpc } from '@/api/atproto/client';
import { usePersistedStore } from './store';
import type { Preferences } from '@/models/preferences';

type FrontendPreferences = {
    editorDarkMode: boolean
}

export const usePdsPreferencesStore = defineStore(
    'preferencesStore',
    {
        state: () => {
            return {
                editorDarkMode: false
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
            }
        }
    });