<script setup lang="ts">
  import { onMounted, ref } from 'vue'
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

  const nsfw = ref<boolean>(false);
  const currentTag = ref<string>("");
  const tags = ref<string[]>([]);

  onMounted(() => {
    Tegaki.open({
      onDone: () => {
        image.value = Tegaki.flatten().toDataURL("image/png");
      },

      onCancel: () => {
        router.push('/');
      },

      width: 380,
      height: 380
    });
  });

  const uploadImage = async () => {
    const { data } = await xrpc.call("com.shinolabs.pinksea.putOekaki", {
      data: {
        data: image.value,
        tags: tags.value,
        nsfw: nsfw.value,
        parent: undefined
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
    <img v-bind:src="image" />
    <br />
    <input type="text" placeholder="Add a description!" />
    <div class="tag-input">
      <TagContainer :tags="tags" />
      <input type="text" placeholder="Tag" v-model="currentTag" v-on:keyup.delete="removeTag" v-on:keyup.space="addTag" v-on:keyup.enter="addTag"/>
    </div>
    <input type="checkbox" value="nsfw" v-model="nsfw"><span>NSFW</span>
    <button v-on:click="uploadImage">Upload!</button>
  </PanelLayout>
</template>

<style scoped>
.tag-input {
  border: 1px solid black;
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
</style>
