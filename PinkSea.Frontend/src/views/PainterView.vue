<script setup lang="ts">
  import { onMounted, ref } from 'vue'
  import { Tegaki } from '@/api/tegaki/tegaki';
  import { useRouter } from 'vue-router'
  import PanelLayout from '@/layouts/PanelLayout.vue'
  import { useIdentityStore, usePersistedStore } from '@/state/store'
  import { xrpc } from '@/api/atproto/client'

  const persistedStore = usePersistedStore();
  const identityStore = useIdentityStore();
  const image = ref<string>("");
  const router = useRouter();

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
        tags: ["#test", "#test2"]
      },
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }
    });

    await router.push(`/${identityStore.did}/oekaki/${data.rkey}`);
  };
</script>

<template>
  <PanelLayout>
    <img v-bind:src="image" />
    <input type="checkbox" value="nsfw"><span>NSFW</span>
    <button v-on:click="uploadImage">Upload!</button>
  </PanelLayout>
</template>

<style scoped>

</style>
