<script setup lang="ts">

import TimeLine from '@/components/TimeLine.vue'
import PanelLayout from '@/layouts/PanelLayout.vue'
import { computed, onBeforeMount, ref } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute } from 'vue-router'

const handle = ref<string>("");
const route = useRoute();

onBeforeMount(async () => {
  const { data } = await xrpc.get("com.shinolabs.pinksea.getHandleFromDid", { params: { did: route.params.did as string }});
  handle.value = data.handle;
});

const url = computed(() => {
  return `https://bsky.app/profile/${route.params.did}`;
});

</script>

<template>
  <PanelLayout>
    <div class="user-card">
      <div>{{ $t("breadcrumb.user_profile", { handle: handle }) }}</div>
      <div><a :href="url" target="_blank">{{ $t("profile.bluesky_profile") }}</a></div>
    </div>
    <TimeLine endpoint="com.shinolabs.pinksea.getAuthorFeed" :xrpc-params="{ did: $route.params.did }" />
  </PanelLayout>
</template>

<style scoped>
  .user-card {
    margin: 20px;
    display: inline-block;
    border: 2px solid #FFB6C1;
    width: calc(100% - 40px);
    border-left: 0.525em solid #FFB6C1;
    box-sizing: border-box;
    position: relative;
    padding: 10px;
  }
</style>
