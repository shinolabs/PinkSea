<script setup lang="ts">

import { useIdentityStore, usePersistedStore } from '@/state/store'
import { xrpc } from '@/api/atproto/client'
import { ref } from 'vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import type { Oekaki } from '@/models/oekaki'

const identityStore = useIdentityStore();
const persistedStore = usePersistedStore();
const image = ref<string | null>(null);
const alt = ref<string>("");
const nsfw = ref<boolean>(false);

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
      tags: [],
      alt: alt.value,
      nsfw: nsfw.value,
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
    <div v-if="identityStore.did === null">{{ $t("response_box.login_to_respond") }}</div>
    <div v-else>
      <div v-if="image === null">
        <p>{{ $t("response_box.click_to_respond") }}</p>
        <button v-on:click.prevent="reply">{{ $t("response_box.open_panel") }}</button>
      </div>
      <div v-else>
        <img :src="image"/>
        <br />
        <input type="text" placeholder="Add a description!" />
        <input type="checkbox" /><span>NSFW</span>
        <button v-on:click.prevent="uploadImage">{{ $t("response_box.reply") }}</button>
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
