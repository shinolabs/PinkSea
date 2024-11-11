<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import PostViewOekakiImageContainer from '@/components/oekaki/PostViewOekakiImageContainer.vue'
import { usePersistedStore } from '@/state/store'

const props = defineProps<{
  oekaki: Oekaki
}>()

const persistedStore = usePersistedStore();

const options: Intl.DateTimeFormatOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric'
}

const authorProfileLink = computed(() => `/${props.oekaki.authorDid}`);
const creationTime = computed(() => {
  return new Date(props.oekaki.creationTime).toLocaleTimeString(persistedStore.lang, options)
})
</script>

<template>
  <div class="oekaki-card">
    <div class="oekaki-child-info">{{ $t("post.response_from_before_handle") }}<b class="oekaki-author"> <RouterLink :to="authorProfileLink" >@{{ props.oekaki.authorHandle }}</RouterLink></b>{{ $t("post.response_from_after_handle") }}{{ $t("post.response_from_at_date") }}{{ creationTime }}</div>
    <PostViewOekakiImageContainer :oekaki="props.oekaki" style="max-height: 400px;"/>
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
  width: calc(100% - 40px);
  margin-left: 30px;
  margin-top: 10px;
  border-left: 0.525em solid #FFB6C1;
  box-sizing: border-box;
  position: relative;
}

.oekaki-card:before {
  content: ""; z-index: 1;
  position: absolute;
  height: 150%; width: 10px;
  border-left: 2px solid #FFB6C1;
  border-bottom: 2px solid #FFB6C1;
  display: block;
  left: -22px; top: -100%;
}

.oekaki-card:nth-of-type(2):before {
  content: ""; z-index: 1;
  position: absolute;
  height: 90%; width: 10px;
  border-left: 2px solid #FFB6C1;
  border-bottom: 2px solid #FFB6C1;
  display: block;
  left: -22px; top: -35%;
}

.oekaki-image-container img {
  max-width: 100%;
  max-height: 100%;
}

.oekaki-child-info {
  border-bottom: 2px dashed #FFB6C1;
  padding: 10px;
}

.oekaki-child-info, .oekaki-child-info * {
  font-size: small;
}

@media (max-width: 768px) {
  .oekaki-card {
    display: inline-block;
    border: 2px solid #FFB6C1;
    width: calc(100% - 40px);
    margin-left: 20px;
    margin-top: 10px;
    border-left: 0.525em solid #FFB6C1;
    box-sizing: border-box;
    position: relative;
  }
}
</style>
