<script setup lang="ts">

import PanelLayout from '@/layouts/PanelLayout.vue'
import { onBeforeMount, ref } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute } from 'vue-router'
import type { Oekaki } from '@/models/oekaki'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import { usePersistedStore } from '@/state/store'
import PostViewOekakiParentCard from '@/components/oekaki/PostViewOekakiParentCard.vue'
import PostViewOekakiChildCard from '@/components/oekaki/PostViewOekakiChildCard.vue'
import RespondBox from '@/components/RespondBox.vue'
import Loader from '@/components/Loader.vue'

const route = useRoute();

const parent = ref<Oekaki | null>(null);
const children = ref<Oekaki[] | null>(null);

onBeforeMount(async () => {
  const { data } = await xrpc.get("com.shinolabs.pinksea.getOekaki", {
    params: {
      did: route.params.did as string,
      rkey: route.params.rkey as string
    }
  });

  parent.value = data.parent;
  children.value = data.children;
});

</script>

<template>
  <PanelLayout>
    <Loader v-if="parent === null" />
    <div v-else>
      <PostViewOekakiParentCard :oekaki="parent" />
      <br />
      <PostViewOekakiChildCard v-for="child of children" v-bind:key="child.atProtoLink" :oekaki="child" />

      <RespondBox />
    </div>
  </PanelLayout>
</template>

<style scoped>

</style>
