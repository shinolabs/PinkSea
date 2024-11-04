<script setup lang="ts">
  import { onMounted, ref } from 'vue'
  import { Tegaki } from '@/tegaki/tegaki';
  import { useRouter } from 'vue-router'
  import PanelLayout from '@/layouts/PanelLayout.vue'
  import { usePersistedStore } from '@/state/store'

  const persistedStore = usePersistedStore();
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

  const uploadImage = () => {
    fetch("http://localhost:5084/xrpc/com.shinolabs.pinksea.putOekaki", {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${persistedStore.token}`
      },
      body: JSON.stringify({
        data: image.value,
        tags: ['#test', '#whatever']
      })
    })
      .then(_ => router.push('/'));
  };
</script>

<template>
  <PanelLayout>
    <img v-bind:src="image" />
    <button v-on:click="uploadImage">Upload!</button>
  </PanelLayout>
</template>

<style scoped>

</style>
