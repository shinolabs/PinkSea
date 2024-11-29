<script setup lang="ts">

import TimeLine from '@/components/TimeLine.vue'
import PanelLayout from '@/layouts/PanelLayout.vue'
import { computed, ref, watch } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute } from 'vue-router'
import { UserProfileTab } from '@/models/user-profile-tab'

const handle = ref<string>("");
const route = useRoute();

const currentTab = ref<UserProfileTab>(UserProfileTab.Posts);

watch(() => route.params.did, async () => {
  const { data } = await xrpc.get("com.shinolabs.pinksea.getHandleFromDid", { params: { did: route.params.did as string }});
  handle.value = data.handle;
}, {immediate: true});

const bskyUrl = computed(() => {
  return `https://bsky.app/profile/${route.params.did}`;
});

const domainUrl = computed(() => {
  return `http://${handle.value}`;
});

</script>

<template>
  <PanelLayout>
    <div class="user-card">
      <h2>{{ $t("breadcrumb.user_profile", { handle: handle }) }}</h2>
      <div><a class="bluesky-link" :href="bskyUrl" target="_blank">{{ $t("profile.bluesky_profile") }}</a></div>
      <div><a class="domain-link" :href="domainUrl" target="_blank">{{ $t("profile.domain") }}</a></div>
    </div>
    <div id="profile-tabs">
      <a id="selected" href="#artwork" v-on:click.prevent="currentTab = UserProfileTab.Posts">Artwork</a>
      <a href="#replies" v-on:click.prevent="currentTab = UserProfileTab.Replies">Replies</a>
    </div>
    <TimeLine v-if="currentTab == UserProfileTab.Posts" endpoint="com.shinolabs.pinksea.getAuthorFeed" :xrpc-params="{ did: $route.params.did }" />
    <TimeLine v-if="currentTab == UserProfileTab.Replies" endpoint="com.shinolabs.pinksea.getAuthorReplies" :xrpc-params="{ did: $route.params.did }" />
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
    width: 50%; height: 20px;
    padding-top: 3px;
    border-bottom: 1px solid #ffb6c1;
  }

  #profile-tabs a#selected {
    background: #ffb6c1; color: #263b48;
    font-weight: bold;
  }

  #profile-tabs a:hover:not(#selected) {
    background: #ffb6c140;
  }

  .user-card {
    display: block; text-align: left;
    margin: 6px 0px -12px 20px;
    box-sizing: border-box;
    position: relative;
    padding: 10px;
    border-left: 6px double #FFB6C1;
  }
  .user-card a {
    color: #0085FF;
    text-decoration: 1px dotted underline;
  }
  .user-card .bluesky-link:before {
    content: "☁ ";
  }

  .user-card .domain-link:before {
    content: "⌂ ";
  }

  .user-card a:hover {
    color: #FFFFFF;
    background: #0085FF;
    text-decoration: none;
  }

  .user-card h2 {
    margin: 0px;
    font-size: 16pt; font-weight: normal;
    padding-bottom: 5px;
  }

  .user-card h2 span {
    font-size: 16pt;
    font-weight: bold;
    background-color: #FFB6C1;
    color: #263b48;
    padding: 1px 4px 1px 4px;
    margin-right: 1px;
    border-radius: 4px;
  }
</style>
