<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'

const props = defineProps<{
  oekaki: Oekaki
}>()

const options: Intl.DateTimeFormatOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric'
}

const imageLink = computed(() => `url(${props.oekaki.imageLink})`)
const authorProfileLink = computed(() => `/${props.oekaki.authorDid}`);
const creationTime = computed(() => {
  console.log(props.oekaki.creationTime)
  return new Date(props.oekaki.creationTime).toLocaleTimeString(undefined, options)
})
</script>

<template>
  <div class="oekaki-card">
    <div class="oekaki-image"></div>
    <div class="oekaki-meta">
      <span>By <b class="oekaki-author"> <RouterLink :to="authorProfileLink" >@{{ props.oekaki.authorHandle }}</RouterLink></b></span><br>
      <span>{{ creationTime }}</span><br>
      <div class="oekaki-tag-container" v-if="props.oekaki.tags !== undefined">
        <span class="oekaki-tag" v-for="tag of props.oekaki.tags" v-bind:key="tag">#{{ tag }}</span>
      </div>
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
  width: 300px;
  border: 2px solid #FFB6C1;
}

.oekaki-image {
  width: auto;
  height: 200px;
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
  margin: 2px;
  background-image: v-bind(imageLink);
}

.oekaki-meta {
  font-size: small;
  padding: 10px;
  color: #2f4858;
  border-top: 2px dashed #FFB6C1;
  border-left: 0.525em solid #FFB6C1;
}

.oekaki-tag-container {
  display: flex;
  justify-content: left;
  margin-top: 10px;
}

.oekaki-tag {
  margin-right: 5px;
  padding: 5px;
  background-color: #FFB6C1;
  border-radius: 4px;
  color: #263B48;
}
</style>
