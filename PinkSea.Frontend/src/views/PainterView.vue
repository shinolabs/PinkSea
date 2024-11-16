<script setup lang="ts">
import { onMounted, ref, useTemplateRef, watch } from 'vue'
  import { Tegaki } from '@/api/tegaki/tegaki';
  import { useRouter } from 'vue-router'
  import PanelLayout from '@/layouts/PanelLayout.vue'
  import { useIdentityStore, useImageStore, usePersistedStore } from '@/state/store'
  import { xrpc } from '@/api/atproto/client'
  import TagContainer from '@/components/TagContainer.vue'
  import i18next from 'i18next'

  const persistedStore = usePersistedStore();
  const identityStore = useIdentityStore();
  const imageStore = useImageStore();
  const image = ref<string>(imageStore.lastDoneImage ?? "");
  const router = useRouter();

  const bsky = ref<boolean>(false);
  const nsfw = ref<boolean>(false);
  const currentTag = ref<string>("");
  const alt = ref<string>("");
  const tags = ref<string[]>([]);

  const button = useTemplateRef<HTMLButtonElement>("upload-button");

  onMounted(() => {
    if (image.value !== "" && !imageStore.restartPainting)
      return;

    if (imageStore.lastUploadErrored)
    {
      if (confirm(i18next.t("painter.do_you_want_to_restore")))
        return;
    }

    openTegaki();
  });

  watch(imageStore, () => {
    if (imageStore.restartPainting)
    {
      if (imageStore.lastUploadErrored)
      {
        if (confirm(i18next.t("painter.do_you_want_to_restore")))
          return;
      }

      openTegaki();
    }
  });

  const openTegaki = () => {
    imageStore.restartPainting = false;
    imageStore.lastUploadErrored = false;

    try {
      Tegaki.destroy();
    } catch {

    } finally {
      Tegaki.open({
        onDone: () => {
          image.value = Tegaki.flatten().toDataURL("image/png");
          imageStore.lastDoneImage = image.value;
        },

        onCancel: () => {
          router.push('/');
        },

        width: 400,
        height: 400
      });
    }
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

      const { data } = await xrpc.call("com.shinolabs.pinksea.putOekaki", {
        data: {
          data: image.value,
          tags: tags.value,
          nsfw: nsfw.value,
          alt: alt.value,
          parent: undefined,
          bskyCrosspost: bsky.value
        },
        headers: {
          "Authorization": `Bearer ${persistedStore.token}`
        }
      });

      imageStore.lastDoneImage = null;
      imageStore.lastUploadErrored = false;

      await router.push(`/${identityStore.did}/oekaki/${data.rkey}`);
    } catch {
      button.value!.disabled = false;
      imageStore.lastUploadErrored = true;

      alert(i18next.t("painter.could_not_send_post"));
    }
  };

  const addTag = () => {
    if (currentTag.value.trim().length < 1)
      return;

    if (tags.value.length > 5)
      return;

    tags.value.push(currentTag.value);
    currentTag.value = "";
  };

  const removeTag = () => {
    if (currentTag.value.length > 0)
      return;

    if (tags.value.length < 1)
      return;

    tags.value.pop();
  };
</script>

<template>
  <PanelLayout>
    <div class="image-container">
      <img v-bind:src="image" />
    </div>
    <br />
    <div class="response-tools">
      <div class="response-extra">
        <input type="text" v-model="alt" :placeholder="i18next.t('painter.add_a_description')" />
        <span><input type="checkbox" value="nsfw" v-model="nsfw"><span>NSFW</span></span>
      </div>

      <div class="tag-input">
        <TagContainer :tags="tags" :disableNavigation="true" />
        <input type="text" :placeholder="i18next.t('painter.tag')" v-model="currentTag" v-on:keyup.delete="removeTag" v-on:keyup.space="addTag" v-on:keyup.enter="addTag"/>
      </div>

      <div class="response-extra">
        <button v-on:click="uploadImage" ref="upload-button">{{ $t("painter.upload")}}</button>
        <span><input type="checkbox" value="bsky" v-model="bsky"><span>{{ $t("painter.crosspost_to_bluesky")}} </span></span>
      </div>
    </div>
  </PanelLayout>
</template>

<style scoped>
.image-container {
  display: flex;
  justify-content: center;
  width: 100%;
  max-height: 400px;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
}

.image-container img {
  max-height: 400px;
}

.response-extra {
  width: 100%;
  display: flex;
}

.response-extra button {
  flex: 1;
}

.response-extra input[type=text] {
  flex: 1;
}

.response-tools {
  padding: 10px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.response-tools * {
  margin-bottom: 10px;
}

.tag-input {
  border: 1px solid #FFB6C1;
  display: flex;
  flex-direction: row;
  align-items: center;
  padding: 5px;
}

.tag-input * {
  margin: 0px 5px 0px 0px !important;
}

.tag-input input {
  flex: 1;
  border: none;
  outline: none;
}

.tag-input input:focus {
  border: none;
  outline: none;
}

button {
  font-size: 14pt;
}
</style>
