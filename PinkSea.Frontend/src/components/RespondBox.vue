<script setup lang="ts">

import { useIdentityStore, usePersistedStore } from '@/state/store'
import { xrpc } from '@/api/atproto/client'
import { ref } from 'vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import type { Oekaki } from '@/models/oekaki'

const identityStore = useIdentityStore();
const persistedStore = usePersistedStore();
const image = ref<string | null>(null);

const props = defineProps<{
  parent: Oekaki
}>();

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
      parent: props.parent.atProtoLink
    },
    headers: {
      "Authorization": `Bearer ${persistedStore.token}`
    }
  });

  window.location.reload();
};
</script>

<template>
  <div class="respond-box">
    <div v-if="identityStore.did === null">Login to respond!</div>
    <div v-else>
      <div v-if="image === null">
        <p>Click to open the drawing panel</p>
        <button v-on:click.prevent="reply">Open painter</button>
      </div>
      <div v-else>
        <img :src="image" />
        <button v-on:click.prevent="uploadImage">Reply!</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.respond-box {
  border: 2px dashed #FFB6C1;
  margin: 20px;
  padding: 10px;
  display: flex;
  justify-content: center;
}

.respond-box button {
  width: 100%;
}
</style>
