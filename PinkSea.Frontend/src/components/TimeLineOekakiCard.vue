<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import { useRouter } from 'vue-router'
import { usePersistedStore } from '@/state/store'
import { buildOekakiUrlFromOekakiObject } from '@/api/atproto/helpers'
import OekakiMetaContainer from './oekaki/OekakiMetaContainer.vue'

const router = useRouter();
const persistedStore = usePersistedStore();

const props = defineProps<{
  oekaki: Oekaki
}>()

const imageLink = computed(() => `url(${props.oekaki.image})`)
const altText = computed(() => props.oekaki.alt ?? "");

const navigateToPost = () => {
  const url = buildOekakiUrlFromOekakiObject(props.oekaki);
  router.push(url);
};

const openInNewTab = () => {
  const url = buildOekakiUrlFromOekakiObject(props.oekaki);
  window.open(url, "_blank");
};


</script>

<template>
  <div class="oekaki-card" v-if="!props.oekaki.nsfw || (props.oekaki.nsfw && !persistedStore.hideNsfw)">
    <div class="oekaki-image" v-on:click.prevent="navigateToPost" v-on:mousedown.middle.stop.prevent="openInNewTab"
      :title="altText">
      <div class="oekaki-nsfw-blur" v-if="props.oekaki.nsfw && persistedStore.blurNsfw">NSFW</div>
    </div>
    <OekakiMetaContainer :oekaki="props.oekaki" :show-substitute-on-no-tags="true" />
  </div>
</template>

<style scoped>
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

@media (min-width: 768px) and (max-width: 932px) {
  .oekaki-card {
    width: calc((100% / 2) - 25px);
  }
}
</style>
