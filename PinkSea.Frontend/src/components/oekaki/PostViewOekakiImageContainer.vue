<script setup lang="ts">
import { computed, useTemplateRef } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import { usePersistedStore } from '@/state/store'

const props = defineProps<{
  oekaki: Oekaki
}>()

const persistedStore = usePersistedStore();
const nsfwRef = useTemplateRef<HTMLDivElement>("nsfw-cover");
const imageLink = computed(() => `url(${props.oekaki.image})`)
const altText = computed(() => props.oekaki.alt ?? "");

const hideNSFWBlur = () => {
  nsfwRef.value!.style.display = "none";
};
</script>

<template>
  <div class="oekaki-image-container">
    <div class="oekaki-image">
      <img :src="props.oekaki.image" :alt="altText" :title="altText" />
      <div class="oekaki-nsfw-cover" ref="nsfw-cover" :title="altText" v-if="props.oekaki.nsfw && persistedStore.blurNsfw">
        <div class="oekaki-nsfw-blur" v-on:click.prevent="hideNSFWBlur">
          NSFW
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.oekaki-image-container {
  width: 100%;
  display: flex;
  justify-content: center;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
}

.oekaki-image {
  position: relative;
  max-height: inherit;
}

.oekaki-image-container .oekaki-image, .oekaki-image-container .oekaki-image img {
  max-width: 100%;
}

.oekaki-nsfw-cover {
  position: absolute;
  top: 0;
  width: 100%;
  height: 100%;
  background-image: v-bind(imageLink);
  cursor: pointer;
}

.oekaki-nsfw-blur {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: white;
  transition: backdrop-filter 0.1s;
  font-weight: bold;
  overflow: clip;
}

img {
  max-height: inherit;
}

@-moz-document url-prefix() {
  .oekaki-nsfw-blur {
    backdrop-filter: blur(30px);
    background-color: transparent;
  }
}
</style>
