<script setup lang="ts">

import { useIdentityStore, useImageStore, usePersistedStore } from '@/state/store'
import { xrpc } from '@/api/atproto/client'
import { onBeforeMount, onMounted, ref, useTemplateRef } from 'vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import type { Oekaki } from '@/models/oekaki'
import i18next from 'i18next'
import i18n from '@/intl/i18n'

const identityStore = useIdentityStore();
const persistedStore = usePersistedStore();
const image = ref<string | null>(null);
const alt = ref<string>("");
const nsfw = ref<boolean>(false);

const button = useTemplateRef<HTMLButtonElement>("upload-button");

const imageStore = useImageStore();

const props = defineProps<{
  parent: Oekaki
}>();

const reply = () => {
  try {
    Tegaki.destroy();
  } catch {

  } finally {
    Tegaki.open({
      onDone: () => {
        image.value = Tegaki.flatten().toDataURL("image/png");
      },

      width: 480,
      height: 160
    });
  }
};

const cancel = () => {
  image.value = null;

  imageStore.lastDoneReply = null;
  imageStore.lastReplyErrored = false;
  imageStore.lastReplyId = "";
};

const uploadImage = async () => {
  try {
    button.value!.disabled = true;

    // Force refresh the session, just to be sure.
    await xrpc.call("com.shinolabs.pinksea.refreshSession", {
      data: {},
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }
    });

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

    imageStore.lastDoneReply = null;
    imageStore.lastReplyErrored = false;
    imageStore.lastReplyId = "";

    window.location.reload();
  } catch {
    button.value!.disabled = false;

    imageStore.lastDoneReply = image.value;
    imageStore.lastReplyErrored = true;
    imageStore.lastReplyId = props.parent.oekakiRecordKey;

    alert(i18next.t("painter.could_not_send_post"));
  }
};

onBeforeMount(() => {
  if (imageStore.lastReplyId == props.parent.oekakiRecordKey
    && imageStore.lastDoneReply !== null && imageStore.lastReplyErrored)
  {
    image.value = imageStore.lastDoneReply;
  }
})
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
          <input type="text" :placeholder="i18next.t('painter.add_a_description')" v-model="alt" />
          <span><input type="checkbox" v-model="nsfw" /><span>NSFW</span></span>
        </div>
        <div class="two-buttons">
          <button v-on:click.prevent="cancel">{{ $t("response_box.cancel") }}</button>
          <button v-on:click.prevent="uploadImage" ref="upload-button">{{ $t("response_box.reply") }}</button>
        </div>
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

.two-buttons {
    display: flex;
}
</style>
