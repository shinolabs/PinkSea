<script setup lang="ts">

import PanelLayout from '@/layouts/PanelLayout.vue'
import { computed, onBeforeMount, ref } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute } from 'vue-router'
import type { Oekaki } from '@/models/oekaki'
import PostViewOekakiParentCard from '@/components/oekaki/PostViewOekakiParentCard.vue'
import PostViewOekakiChildCard from '@/components/oekaki/PostViewOekakiChildCard.vue'
import RespondBox from '@/components/RespondBox.vue'
import Loader from '@/components/Loader.vue'
import PostViewOekakiTombstoneCard from '@/components/oekaki/PostViewOekakiTombstoneCard.vue'

const route = useRoute();

const parent = ref<Oekaki | null>(null);
const children = ref<Oekaki[] | null>(null);

const parentIsTombstone = computed(() => {
  return parent.value !== null && 'formerAt' in parent.value;
});

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
      <PostViewOekakiTombstoneCard v-if="parentIsTombstone" />
      <PostViewOekakiParentCard v-else :oekaki="parent" />
      <br />
      <PostViewOekakiChildCard v-for="child of children" v-bind:key="child.at" :oekaki="child" />

      <RespondBox v-if="!parentIsTombstone" :parent="parent" />
    </div>
  </PanelLayout>
</template>

<style scoped>

</style>
