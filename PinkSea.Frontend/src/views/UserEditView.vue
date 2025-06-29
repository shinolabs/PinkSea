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

const linkUrl = ref<string>("");
const linkName = ref<string>("");

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

const addLink = (name: string, url: string) => {
    profile.value!.links = [
        ...profile.value!.links,
        {
            name,
            url
        }
    ];
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
                <div class="user-card-padding">
                    <UserCard :profile="profile" :show-edit-button="false" />
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="Basic data">
                <table>
                    <tbody>
                        <tr>
                            <td class="settings-row-heading">
                                <div>Nickname</div>
                            </td>
                            <td>
                                <input type="text" placeholder="" v-model="profile.nick" />
                                <div class="settings-description">
                                    This is the name others will identify you by. By default, it's equal to your handle.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="settings-row-heading">
                                <div>Your bio</div>
                            </td>
                            <td>
                                <textarea placeholder="Your description (256 characters max)"
                                    v-model="profile.description"></textarea>
                                <div class="settings-description">
                                    This is your bio. It's a short piece of text that's visible on your profile. By default, it's empty.
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </SettingsGroup>

            <SettingsGroup intl-key="Avatar">
                <div v-if="avatarList.length > 0" class="avatar-list">
                    <img v-for="oekaki of avatarList" :src="oekaki.image"
                        v-on:click.prevent="profile.avatar = oekaki.image"
                        :class="oekaki.image === profile.avatar ? 'selected' : ''" />
                </div>
                <div class="settings-description">
                    Select an avatar from the list of oekaki you've drawn!
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="Links">
                <table>
                    <tbody>
                        <tr>
                            <td class="settings-row-heading">
                                <div>Name</div>
                            </td>
                            <td>
                                <input type="text" placeholder="Example" v-model="linkName" />
                                <div class="settings-description">
                                    This is the name of the link you're creating.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="settings-row-heading">
                                <div>Url</div>
                            </td>
                            <td>
                                <input type="text" placeholder="https://example.com/..." v-model="linkUrl" />
                                <div class="settings-description">
                                    This is the url of the link you're creating.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <button v-on:click.prevent="addLink(linkName, linkUrl)">Add</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div v-for="link of profile.links" class="link-display">
                    <div class="link-name">
                        {{ link.name }}
                    </div>
                    <div class="link-link">
                        {{ link.url }}
                    </div>
                    <button>x</button>
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

.main-container > fieldset {
    margin-bottom: 20px;
}

.avatar-list {
    max-height: 200px;
    overflow-y: scroll;
}

.avatar-list>img {
    display: inline;
    max-width: 120px;
    padding: 5px;
}

.selected {
    background-color: #FFB6C1;
}

.settings-row-heading {
    vertical-align: top;
    font-weight: bold;
}

.settings-description {
    font-size: 10pt;
}

textarea {
    width: 100%;
    border: 1px solid #FFB6C1;
}

.user-card-padding {
    margin: 10px;
}

.link-display {
    background-color: #FFE5EA;
    padding: 10px;
    margin-bottom: 5px;
}

.link-display > button {
    float: right;
}

.link-name {
    font-weight: bold;
    font-size: 14pt;
    margin-right: 15px;
}

.link-link {
    font-size: 10pt;
}

.link-display > div {
    display: inline-block;
} 

</style>