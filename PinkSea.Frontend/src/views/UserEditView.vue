<script setup lang="ts">
import { xrpc } from '@/api/atproto/client';
import Avatar from '@/components/profile/Avatar.vue';
import SettingsGroup from '@/components/SettingsGroup.vue';
import UserCard from '@/components/UserCard.vue';
import PanelLayout from '@/layouts/PanelLayout.vue';
import type { Oekaki } from '@/models/oekaki';
import type Profile from '@/models/profile';
import { useIdentityStore, usePersistedStore } from '@/state/store';
import { onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';

const identityStore = useIdentityStore()
const persistedStore = usePersistedStore()
const profile = ref<Profile | null>(null);

const linkUrl = ref<string>("");
const linkName = ref<string>("");

const router = useRouter()

const avatarList = ref<Oekaki[]>([]);

const updateProfile = async () => {
    try {
        const { data } = await xrpc.get("com.shinolabs.pinksea.getProfile", { params: { did: identityStore.did as string } });
        profile.value = data;

        console.log(profile.value)

        await fetchPossibleAvatars();
    } catch (e) {
        console.error("Couldn't fetch our own profile, lol. " + e)
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
        ...profile.value!.links!,
        {
            name,
            url
        }
    ];
};

const buildAvatarRef = () => {
    if (profile.value?.avatar === undefined) {
        return undefined
    }

    const avatar = avatarList.value.find(a => a.image == profile.value?.avatar)

    if (avatar === undefined) {
        return undefined
    }

    return {
        uri: avatar!.at,
        cid: avatar!.cid
    }
}

const sendChanges = async () => {
    try {
        await xrpc.call("com.shinolabs.pinksea.putProfile", {
            data: {
                profile: {
                    nickname: profile.value?.nick,
                    bio: profile.value?.description,
                    links: profile.value?.links!.map(l => {
                        return {
                            link: l.url,
                            name: l.name
                        }
                    }),
                    avatar: buildAvatarRef()
                }
            },
            headers: {
                "Authorization": `Bearer ${persistedStore.token}`
            }
        })

        router.push(`/${profile.value?.did}`)
    } catch (e) {
        alert(e)
    }
};

const removeLink = (index: number) => {
    profile.value?.links!.splice(index, 1)
}

watch(identityStore, updateProfile);
onMounted(updateProfile);

</script>

<template>
    <PanelLayout>
        <div v-if="profile === null">
            loading...
        </div>
        <div class="main-container" v-else>
            <SettingsGroup intl-key="profile_edit.how_others_will_see_you">
                <div class="user-card-padding">
                    <UserCard :profile="profile" :show-edit-button="false" />
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="profile_edit.basic_data">
                <table>
                    <tbody>
                        <tr>
                            <td class="settings-row-heading">
                                <div>{{ $t("profile_edit.nickname") }}</div>
                            </td>
                            <td>
                                <input type="text" placeholder="" v-model="profile.nick" maxlength="64" />
                                <div class="settings-description">
                                    {{ $t("profile_edit.nickname_description") }}
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="settings-row-heading">
                                <div>{{ $t("profile_edit.your_bio") }}</div>
                            </td>
                            <td>
                                <textarea placeholder="Your description (256 characters max)"
                                    v-model="profile.description" maxlength="256"></textarea>
                                <div class="settings-description">
                                    {{ $t("profile_edit.your_bio_description") }}
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </SettingsGroup>

            <SettingsGroup intl-key="profile_edit.avatar">
                <div v-if="avatarList.length > 0" class="avatar-list">
                    <div :class="profile.avatar === undefined || profile.avatar === null ? 'selected' : ''">
                        <Avatar image="/assets/img/blank_avatar.png" v-on:click.prevent="profile.avatar = undefined"
                            :size="128" />
                    </div>
                    <div v-for="oekaki of avatarList" :class="oekaki.image === profile.avatar ? 'selected' : ''">
                        <Avatar :image="oekaki.image" v-on:click.prevent="profile.avatar = oekaki.image" :size="128" />
                    </div>
                </div>
                <div class="settings-description">
                    {{ $t("profile_edit.avatar_description") }}
                </div>
            </SettingsGroup>

            <SettingsGroup intl-key="profile_edit.links">
                <table>
                    <tbody>
                        <tr>
                            <td class="settings-row-heading">
                                <div>{{ $t("profile_edit.link_name") }}</div>
                            </td>
                            <td>
                                <input type="text" placeholder="Example" v-model="linkName" maxlength="50" />
                                <div class="settings-description">
                                    {{ $t("profile_edit.link_name_description") }}
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="settings-row-heading">
                                <div>{{ $t("profile_edit.link_url") }}</div>
                            </td>
                            <td>
                                <input type="text" placeholder="https://example.com/..." v-model="linkUrl" />
                                <div class="settings-description">
                                    {{ $t("profile_edit.link_url_description") }}
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <button v-on:click.prevent="addLink(linkName, linkUrl)">{{ $t("profile_edit.link_add")
                                    }}</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div v-for="(link, index) of profile.links" class="link-display">
                    <div class="link-name">
                        {{ link.name }}
                    </div>
                    <div class="link-link">
                        {{ link.url }}
                    </div>
                    <button v-on:click.prevent="removeLink(index)">x</button>
                </div>
            </SettingsGroup>
            <button v-on:click.prevent="sendChanges()">{{ $t("profile_edit.save_changes") }}</button>
        </div>
    </PanelLayout>
</template>

<style scoped>
.main-container {
    padding: 10px;
    overflow: hidden;
}

.main-container>fieldset {
    margin-bottom: 20px;
}

.avatar-list {
    max-height: 200px;
    overflow-y: scroll;
}

.avatar-list>div {
    padding: 5px;
    display: inline-block;
}

.selected {
    background-color: #FFB6C1 !important;
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

.link-display>button {
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

.link-display>div {
    display: inline-block;
}
</style>