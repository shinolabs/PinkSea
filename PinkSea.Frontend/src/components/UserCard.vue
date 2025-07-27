<script setup lang="ts">
import type Profile from '@/models/profile';
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import Avatar from './profile/Avatar.vue';
import i18next from 'i18next';

const router = useRouter();

const props = defineProps<{
    profile: Profile,
    showEditButton: boolean
}>();

const description = computed(() => {
    return props.profile.description ?? i18next.t("profile.this_user_has_no_description")
});

const nickname = computed(() => {
    return props.profile.nick ?? props.profile.handle
});

</script>

<template>
    <div class="user-card">
        <div class="user-card-avatar">
            <Avatar :image="props.profile.avatar" :size="100" />
        </div>
        <div class="user-card-data-container">
            <div class="user-card-nickname-container">
                <h2>{{ nickname }}</h2>
                <h3>@{{ props.profile.handle }}</h3>
            </div>
            <div class="user-card-description">
                {{ description }}
            </div>
            <div>
                <a v-for="link of props.profile.links" class="link" :href="link.url" target="_blank">{{ link.name }}</a>
            </div>
        </div>
        <div class="button-container" v-if="props.showEditButton">
            <button v-on:click.prevent="router.push('/settings/profile')">{{ $t("profile.edit_profile") }}</button>
        </div>
    </div>
</template>

<style scoped>
.user-card {
    display: flex;
    text-align: left;
    margin: 6px 0px -12px 20px;
    box-sizing: border-box;
    position: relative;
    padding: 10px;
    border-left: 6px double #FFB6C1;
}

.user-card>div {
    margin-right: 10px;
}

.user-card-nickname-container {
    line-height: 12pt;
    margin-bottom: 10pt;
}

.user-card-data-container {
    display: flex;
    flex-direction: column;
    flex: 1;
}

.user-card-description {
    margin-bottom: 10pt;
    word-wrap: break-word;
    word-break: break-all;
    flex: 1;
}

.user-card a {
    color: #0085FF;
    text-decoration: 1px dotted underline;
    margin-right: 10px;
}

.user-card a:hover {
    color: #FFFFFF;
    background: #0085FF;
    text-decoration: none;
}

.user-card h2 {
    margin: 0px;
    font-size: 16pt;
    font-weight: bold;
    padding-bottom: 5px;
}

.user-card h3 {
    margin: 0px;
    font-size: 12pt;
    font-weight: normal;
    padding-bottom: 5px;
}
</style>