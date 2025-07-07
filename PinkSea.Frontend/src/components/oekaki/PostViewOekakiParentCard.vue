<script setup lang="ts">
import type { Oekaki } from '@/models/oekaki'
import PostViewOekakiImageContainer from '@/components/oekaki/PostViewOekakiImageContainer.vue'
import { usePersistedStore } from '@/state/store'
import OekakiMetaContainer from './OekakiMetaContainer.vue'

const props = defineProps<{
  oekaki: Oekaki
}>()

const persistedStore = usePersistedStore();
</script>

<template>
  <div class="oekaki-card" v-if="!props.oekaki.nsfw || (props.oekaki.nsfw && !persistedStore.hideNsfw)">
    <PostViewOekakiImageContainer :oekaki="props.oekaki" />
    <OekakiMetaContainer :oekaki="props.oekaki" :show-post-actions="true" />
  </div>
</template>

<style scoped>
.oekaki-author {
  text-decoration: underline dotted;
}

.oekaki-author:hover {
  text-decoration: underline;
  cursor: pointer;
}

.oekaki-card {
  display: inline-block;
  border: 2px solid #FFB6C1;
  width: calc(100% - 20px);
  box-sizing: border-box;
  margin: 10px;
  z-index: 5;
  position: relative;
}
</style>
