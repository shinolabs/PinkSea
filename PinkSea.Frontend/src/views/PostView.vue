<script setup lang="ts">

import PanelLayout from '@/layouts/PanelLayout.vue'
import { onBeforeMount, ref } from 'vue'
import { xrpc } from '@/api/atproto/client'
import { useRoute } from 'vue-router'
import type { Oekaki } from '@/models/oekaki'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import { usePersistedStore } from '@/state/store'

const route = useRoute();
const persistedStore = usePersistedStore();

const parent = ref<Oekaki | null>(null);
const children = ref<Oekaki[] | null>(null);
const image = ref<string | null>(null);

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

const reply = () => {
  Tegaki.open({
    onDone: () => {
      image.value = Tegaki.flatten().toDataURL("image/png");
    },

    width: 380,
    height: 380
  });
};

const uploadImage = async () => {
  await xrpc.call("com.shinolabs.pinksea.putOekaki", {
    data: {
      data: image.value as string,
      tags: ["#test", "#test2"],
      parent: parent.value?.atProtoLink
    },
    headers: {
      "Authorization": `Bearer ${persistedStore.token}`
    }
  });

  window.location.reload();
};
</script>

<template>
  <PanelLayout>
    <div v-if="parent === null">
      loading...
    </div>
    <div v-else>
      <TimeLineOekakiCard :oekaki="parent" />
      <br />
      <TimeLineOekakiCard v-for="child of children" v-bind:key="child.atProtoLink" :oekaki="child" />

      <div v-if="image === null">
        <button v-on:click.prevent="reply">Reply</button>
      </div>
      <div v-else>
        <img :src="image" />
        <button v-on:click.prevent="uploadImage">Upload</button>
      </div>
    </div>
  </PanelLayout>
</template>

<style scoped>

</style>
