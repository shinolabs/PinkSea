<script setup lang="ts">
import { useRouter } from 'vue-router'
import { onBeforeMount, ref } from 'vue'
import { usePersistedStore } from '@/state/store'

const router = useRouter();
const temp = ref<string | null>();

const persistedStore = usePersistedStore()

onBeforeMount(async () => {
  try {
    const urlParams = new URLSearchParams(window.location.search);
    persistedStore.token = urlParams.get('code');
  } catch {
    persistedStore.token = null;
  } finally {
    await router.replace("/");
  }
});
</script>

<template>
  <div>Your code: {{ temp }}</div>
</template>

<style scoped>

</style>
