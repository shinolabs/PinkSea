<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import { useRouter } from 'vue-router'
import TagContainer from '@/components/TagContainer.vue'
import { usePersistedStore } from '@/state/store'

const router = useRouter();
const persistedStore = usePersistedStore();

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
  return new Date(props.oekaki.creationTime).toLocaleTimeString(persistedStore.lang, options)
})
const altText = computed(() => props.oekaki.alt ?? "");

const navigateToPost = () => {
  router.push(`/${props.oekaki.authorDid}/oekaki/${props.oekaki.oekakiRecordKey}`);
};

const openInNewTab = () => {
  window.open(`/${props.oekaki.authorDid}/oekaki/${props.oekaki.oekakiRecordKey}`, "_blank");
};
</script>

<template>
  <div class="oekaki-card" v-if="!props.oekaki.nsfw || (props.oekaki.nsfw && !persistedStore.hideNsfw)">
    <div class="oekaki-image" v-on:click.prevent="navigateToPost" v-on:mousedown.middle.stop.prevent="openInNewTab" :title="altText">
      <div class="oekaki-nsfw-blur" v-if="props.oekaki.nsfw && persistedStore.blurNsfw">NSFW</div>
    </div>
    <div class="oekaki-meta">
      <span>{{ $t("timeline.by_before_handle") }}<b class="oekaki-author"> <RouterLink :to="authorProfileLink" >@{{ props.oekaki.authorHandle }}</RouterLink></b>{{ $t("timeline.by_after_handle") }}</span><br>
      <span>{{ creationTime }}</span><br>
      <TagContainer v-if="props.oekaki.tags !== undefined && props.oekaki.tags.length > 0" :tags="props.oekaki.tags" />
      <div class="oekaki-tag-container-substitute" v-else>.</div>
    </div>
  </div>
</template>

<style scoped>
.oekaki-tag-container-substitute {
  margin-top: 10px;
  padding: 5px;
  visibility: hidden;
}

.oekaki-author {
  text-decoration: underline dotted;
}

.oekaki-author:hover {
  text-decoration: underline;
  cursor: pointer;
}

.oekaki-nsfw-blur {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1FF 0, #FFB6C1FF 0.8px, #FFFFFFFF 0, #FFFFFFFF 50%);
  opacity: 1;
  transition: backdrop-filter 0.1s, opacity 0.1s;
  font-weight: bold;
  overflow: clip;
}

.oekaki-nsfw-blur:hover {
  transition: backdrop-filter 0.1s, opacity 0.1s;
  opacity: 0;
  background-color: transparent;
  background-image: none;
  backdrop-filter: none;
  font-size: 0;
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
  overflow: clip;
}

.oekaki-image:hover {
  cursor: pointer;
}

.oekaki-meta {
  font-size: small;
  padding: 10px;
  color: #2f4858;
  border-top: 2px dashed #FFB6C1;
  border-left: 0.525em solid #FFB6C1;
}

@-moz-document url-prefix() {
  .oekaki-nsfw-blur {
    backdrop-filter: blur(30px);
    background-image: none;
  }
}

@media (max-width: 768px) {
  .oekaki-card {
    display: inline-block;
    width: calc(100% - 30px);
  }
}
</style>
