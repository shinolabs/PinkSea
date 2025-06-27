<script setup lang="ts">
import { xrpc } from '@/api/atproto/client';
import SettingsGroup from '@/components/SettingsGroup.vue';
import UserCard from '@/components/UserCard.vue';
import PanelLayout from '@/layouts/PanelLayout.vue';
import type { Oekaki } from '@/models/oekaki';
import type Profile from '@/models/profile';
import { useIdentityStore } from '@/state/store';
import { onMounted, ref, watch } from 'vue';

const identityStore = useIdentityStore()
const profile = ref<Profile | null>(null);

const avatarList = ref<Oekaki[]>([]);

const updateProfile = async () => {
    try {
        const { data } = await xrpc.get("com.shinolabs.pinksea.unspecced.getProfile", { params: { did: identityStore.did as string } });
        profile.value = data;

        await fetchPossibleAvatars();
    } catch (e) {
        console.error("Couldn't fetch our own profile, lol.")
    }
};

const fetchPossibleAvatars = async () => {
    avatarList.value = [];

    let lastCursor: Date | undefined = undefined;
    while (true) {
        try {
            const { data } = await xrpc.get("com.shinolabs.pinksea.getAuthorFeed", { params: { did: identityStore.did as string, since: lastCursor } });
            avatarList.value = [
                ...avatarList.value,
                ...data.oekaki
            ];

            if (data.oekaki.length < 1) {
                break;
            }

            lastCursor = data.oekaki[data.oekaki.length - 1].creationTime;
        } catch (e) {
            console.error("Couldn't fetch our own profile, lol.")
        }
    }
};

watch(identityStore, updateProfile);
onMounted(updateProfile);

</script>

<template>
    <PanelLayout>
        <div v-if="profile === null">
            loading...
        </div>
        <div class="main-container" v-else>
            <SettingsGroup intl-key="This is how others will see you">
                <UserCard :profile="profile" />
            </SettingsGroup>

            <SettingsGroup intl-key="Basic data">
                <div>
                    <input type="text" placeholder="" v-model="profile.nick" />
                </div>
                <div>
                    <textarea placeholder="Your description (256 characters max)"
                        v-model="profile.description"></textarea>
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="Avatar">
                <div v-if="avatarList.length > 0" class="avatar-list">
                    <img v-for="oekaki of avatarList" :src="oekaki.image"
                        v-on:click.prevent="profile.avatar = oekaki.image" />
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="Links">
                <div v-for="link of profile.links">
                    {{ link.name }} - {{ link.url }}
                </div>
            </SettingsGroup>
        </div>
    </PanelLayout>
</template>

<style scoped>
.main-container {
    padding: 10px;
    overflow: hidden;
}

.avatar-list {
    display: flex;
    max-width: inherit;
    overflow-x: scroll;
}

.avatar-list>img {
    display: inline;
    max-width: 120px;
    padding: 5px;
}

.selected {
    background-color: red;
}
</style>