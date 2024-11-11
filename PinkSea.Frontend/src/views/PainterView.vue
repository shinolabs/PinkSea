<script setup lang="ts">
import { onMounted, ref, useTemplateRef } from 'vue'
  import { Tegaki } from '@/api/tegaki/tegaki';
  import { useRouter } from 'vue-router'
  import PanelLayout from '@/layouts/PanelLayout.vue'
  import { useIdentityStore, usePersistedStore } from '@/state/store'
  import { xrpc } from '@/api/atproto/client'
  import TagContainer from '@/components/TagContainer.vue'

  const persistedStore = usePersistedStore();
  const identityStore = useIdentityStore();
  const image = ref<string>("");
  const router = useRouter();

  const bsky = ref<boolean>(false);
  const nsfw = ref<boolean>(false);
  const currentTag = ref<string>("");
  const alt = ref<string>("");
  const tags = ref<string[]>([]);

  const button = useTemplateRef<HTMLButtonElement>("upload-button");

  onMounted(() => {
    Tegaki.open({
      onDone: () => {
        image.value = Tegaki.flatten().toDataURL("image/png");
      },

      onCancel: () => {
        router.push('/');
      },

      width: 400,
      height: 400
    });
  });

  const uploadImage = async () => {
    button.value!.disabled = true;

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

    await router.push(`/${identityStore.did}/oekaki/${data.rkey}`);
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
        <input type="text" v-model="alt" placeholder="Add a description!" />
        <span><input type="checkbox" value="nsfw" v-model="nsfw"><span>NSFW</span></span>
      </div>

      <div class="tag-input">
        <TagContainer :tags="tags" :disableNavigation="true" />
        <input type="text" placeholder="Tag" v-model="currentTag" v-on:keyup.delete="removeTag" v-on:keyup.space="addTag" v-on:keyup.enter="addTag"/>
      </div>

      <div class="response-extra">
        <button v-on:click="uploadImage" ref="upload-button">Upload!</button>
        <span><input type="checkbox" value="bsky" v-model="bsky"><span>Cross-post to BlueSky</span></span>
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
