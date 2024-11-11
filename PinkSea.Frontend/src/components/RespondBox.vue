<script setup lang="ts">

import { useIdentityStore, usePersistedStore } from '@/state/store'
import { xrpc } from '@/api/atproto/client'
import { ref, useTemplateRef } from 'vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import type { Oekaki } from '@/models/oekaki'

const identityStore = useIdentityStore();
const persistedStore = usePersistedStore();
const image = ref<string | null>(null);
const alt = ref<string>("");
const nsfw = ref<boolean>(false);

const button = useTemplateRef<HTMLButtonElement>("upload-button");

const props = defineProps<{
  parent: Oekaki
}>();

const reply = () => {
  Tegaki.open({
    onDone: () => {
      image.value = Tegaki.flatten().toDataURL("image/png");
    },

    width: 480,
    height: 160
  });
};

const uploadImage = async () => {
  button.value!.disabled = true;

  await xrpc.call("com.shinolabs.pinksea.putOekaki", {
    data: {
      data: image.value as string,
      tags: [],
      alt: alt.value,
      nsfw: nsfw.value,
      parent: props.parent.atProtoLink,
      bskyCrosspost: false
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
        <button v-on:click.prevent="reply">{{ $t("response_box.open_painter") }}</button>
      </div>
      <div v-else>
        <img :src="image"/>
        <br />
        <div class="respond-extra">
          <input type="text" placeholder="Add a description!" v-model="alt" />
          <span><input type="checkbox" v-model="nsfw" /><span>NSFW</span></span>
        </div>
        <button v-on:click.prevent="uploadImage" ref="upload-button">{{ $t("response_box.reply") }}</button>
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

img {
  max-height: 300px;
}

.respond-extra {
  width: 100%;
  display: flex;
  margin-bottom: 10px;
}

.respond-extra input[type=text] {
  flex: 1;
}

.respond-box button {
  width: 100%;
}
</style>
