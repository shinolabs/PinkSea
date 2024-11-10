<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import TagContainer from '@/components/TagContainer.vue'
import PostViewOekakiImageContainer from '@/components/oekaki/PostViewOekakiImageContainer.vue'

const props = defineProps<{
  oekaki: Oekaki
}>()

const options: Intl.DateTimeFormatOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric'
}

const authorProfileLink = computed(() => `/${props.oekaki.authorDid}`);
const creationTime = computed(() => {
  return new Date(props.oekaki.creationTime).toLocaleTimeString(undefined, options)
})

</script>

<template>
  <div class="oekaki-card">
    <PostViewOekakiImageContainer :oekaki="props.oekaki" />
    <div class="oekaki-meta">
      <span>By <b class="oekaki-author"> <RouterLink :to="authorProfileLink" >@{{ props.oekaki.authorHandle }}</RouterLink></b></span><br>
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
}
</style>
