<script setup lang="ts">

import TimeLine from '@/components/TimeLine.vue'
import PanelLayout from '@/layouts/PanelLayout.vue'
import { computed, ref, watch } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute, useRouter } from 'vue-router'
import { UserProfileTab } from '@/models/user-profile-tab'
import { XRPCError } from '@atcute/client'
import ErrorCard from '@/components/ErrorCard.vue'
import type Profile from '@/models/profile'
import UserCard from '@/components/UserCard.vue'
import { useIdentityStore } from '@/state/store'

const tabs = [
  {
    i18n: "profile.posts_tab",
    id: UserProfileTab.Posts
  },
  {
    i18n: "profile.replies_tab",
    id: UserProfileTab.Replies
  }
];

const ourProfile = computed(() => {
  return profile !== null && identityStore.did == profile.value?.did;
});

const profile = ref<Profile | null>(null);
const route = useRoute();

const exists = ref<boolean | null>(null);
const profileError = ref<string>("");

const currentTab = ref<UserProfileTab>(UserProfileTab.Posts);

const identityStore = useIdentityStore();

watch(() => route.params.did, async () => {
  try {
    const { data } = await xrpc.get("com.shinolabs.pinksea.getProfile", { params: { did: route.params.did as string } });
    profile.value = data;
    exists.value = true;
  } catch (e) {
    if (e instanceof XRPCError) {
      const xrpcError = e as XRPCError;
      profileError.value = xrpcError.description ?? "An unknown error has occurred.";
    } else {
      profileError.value = "Failed to load the profile.";
    }

    exists.value = false;
  }

}, { immediate: true });

</script>

<template>
  <PanelLayout>
    <div v-if="exists == null">
      loading...
    </div>
    <div v-else-if="exists == true">
      <UserCard :profile="profile!" :show-edit-button="ourProfile" />
      <div id="profile-tabs">
        <a v-for="tab in tabs" :class="tab.id == currentTab ? 'selected' : ''" v-on:click.prevent="currentTab = tab.id"
          v-bind:key="tab.id">{{ $t(tab.i18n) }}</a>
      </div>
      <TimeLine v-if="currentTab == UserProfileTab.Posts" endpoint="com.shinolabs.pinksea.getAuthorFeed"
        :xrpc-params="{ did: $route.params.did }" />
      <TimeLine v-if="currentTab == UserProfileTab.Replies" endpoint="com.shinolabs.pinksea.getAuthorReplies"
        :xrpc-params="{ did: $route.params.did }" :show-as-replies="true" />
    </div>
    <div v-else>
      <ErrorCard image="/assets/img/missing.png" :i18n-key="profileError" />
    </div>
  </PanelLayout>
</template>

<style scoped>
#profile-tabs {
  position: relative;
  margin: 20px 0px 0px;
  height: 23px;
  box-shadow: 0px 3px #ffffff, 0px 4px #ffb6c1;
}

#profile-tabs a {
  color: #0085ff;
  vertical-align: middle;
  display: inline-block;
  text-decoration: none;
  text-align: center;
  width: 50%;
  height: 20px;
  padding-top: 3px;
  cursor: pointer;
  border-bottom: 1px solid #ffb6c1;
}

#profile-tabs a.selected {
  background: #ffb6c1;
  color: #263b48;
  font-weight: bold;
}

#profile-tabs a:hover:not(.selected) {
  background: #ffb6c140;
}
</style>
