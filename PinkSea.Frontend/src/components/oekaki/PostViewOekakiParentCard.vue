<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import TagContainer from '@/components/TagContainer.vue'
import PostViewOekakiImageContainer from '@/components/oekaki/PostViewOekakiImageContainer.vue'
import { usePersistedStore } from '@/state/store'
import { formatDate } from '@/api/atproto/helpers'

const props = defineProps<{
  oekaki: Oekaki
}>()

const persistedStore = usePersistedStore();

const authorProfileLink = computed(() => `/${props.oekaki.did}`);
const creationTime = computed(() => {
  return formatDate(props.oekaki.creationTime)
})

</script>

<template>
  <div class="oekaki-card" v-if="!props.oekaki.nsfw || (props.oekaki.nsfw && !persistedStore.hideNsfw)">
    <PostViewOekakiImageContainer :oekaki="props.oekaki" />
    <div class="oekaki-meta">
      <span>{{ $t("timeline.by_before_handle" )}}<b class="oekaki-author"> <RouterLink :to="authorProfileLink" >@{{ props.oekaki.handle }}</RouterLink></b>{{ $t("timeline.by_after_handle" )}}</span><br>
      <span>{{ creationTime }}</span><br>
      <TagContainer v-if="props.oekaki.tags !== undefined" :tags="props.oekaki.tags" />
    </div>
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

.oekaki-meta {
  font-size: small;
  padding: 10px;
  color: #2f4858;
  border-top: 2px dashed #FFB6C1;
  border-left: 0.525em solid #FFB6C1;
  background-color: white;
}
</style>
