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
import PostViewOekakiTombstoneCard from '@/components/ErrorCard.vue'
import type { OekakiTombstone } from '@/models/oekaki-tombstone'
import ErrorCard from '@/components/ErrorCard.vue'
import { getRecordKeyFromAtUri } from '@/api/atproto/helpers'

const route = useRoute();

const parent = ref<Oekaki | OekakiTombstone | null>(null);
const children = ref<Oekaki[] | null>(null);

const parentIsTombstone = computed(() => {
  return parent.value !== null && 'formerAt' in parent.value;
});

const fetchOekaki = async (did: string, rkey: string) => {
  try {
    const { data } = await xrpc.get("com.shinolabs.pinksea.getOekaki", {
      params: {
        did: did,
        rkey: rkey
      }
    });

    parent.value = data.parent;
    children.value = data.children;
  } catch {
    parent.value = {
      formerAt: 'at://did:web:example.com/com.shinolabs.pinksea.oekaki/fakefake'
    } as OekakiTombstone;

    children.value = [];
  }
};

onBeforeMount(async () => {
  let did = route.params.did as string
  let rkey = route.params.rkey as string

  let scrollId = undefined

  try {
    const { data } = await xrpc.get("com.shinolabs.pinksea.getParentForReply", {
      params: {
        did: did,
        rkey: rkey!
      }
    });

    if (data.rkey !== rkey) {
      scrollId = `${did}-${rkey}`;
      did = data.did;
      rkey = data.rkey;
    }
  } catch {

  }

  await fetchOekaki(did, rkey);

  if (scrollId !== undefined) {
    const element = document.getElementById(scrollId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  }
});

</script>

<template>
  <PanelLayout>
    <Loader v-if="parent === null" />
    <div v-else>
      <ErrorCard v-if="parentIsTombstone" image="/assets/img/missing.png" i18n-key="post.this_post_no_longer_exists" />
      <PostViewOekakiParentCard v-else :oekaki="parent as Oekaki" />
      <br />
      <PostViewOekakiChildCard v-for="child of children" v-bind:key="child.at" :oekaki="child"
        :id="`${child.author.did}-${getRecordKeyFromAtUri(child.at)}`" />

      <RespondBox v-if="!parentIsTombstone" :parent="parent as Oekaki" />
    </div>
  </PanelLayout>
</template>

<style scoped></style>
