<script setup lang="ts">
import { computed } from 'vue'
import type { Oekaki } from '@/models/oekaki'
import PostViewOekakiImageContainer from '@/components/oekaki/PostViewOekakiImageContainer.vue'
import { usePersistedStore } from '@/state/store'
import { useRouter } from 'vue-router'
import { xrpc } from '@/api/atproto/client'
import { formatDate, getRecordKeyFromAtUri } from '@/api/atproto/helpers'
import PostActions from '@/components/PostActions.vue'
import Username from '../profile/Username.vue'
import OekakiMetaContainer from './OekakiMetaContainer.vue'
import Avatar from '../profile/Avatar.vue'

const props = defineProps<{
  oekaki: Oekaki,
  hideLineBar?: boolean
}>()

const router = useRouter();
const persistedStore = usePersistedStore();

const authorProfileLink = computed(() => `/${props.oekaki.author.did}`);
const creationTime = computed(() => {
  return formatDate(props.oekaki.creationTime)
})

const classList = computed(() => {
  if (!props.hideLineBar)
    return "oekaki-card line-bar-element"
  return "oekaki-card"
})

const redirectToParent = async () => {
  const rkey = getRecordKeyFromAtUri(props.oekaki.at);

  await router.push(`/${props.oekaki.author.did}/oekaki/${rkey}`);
};
</script>

<template>
  <div :class="classList" v-if="!props.oekaki.nsfw || (props.oekaki.nsfw && !persistedStore.hideNsfw)">
    <div class="oekaki-child-info">
      <Avatar :image="props.oekaki.author.avatar" :size="20" />
      <div class="oekaki-info-text">
        {{ $t("post.response_from_before_handle") }}
        <b class="oekaki-author">
          <RouterLink :to="authorProfileLink">
            <Username :author='props.oekaki.author' />
          </RouterLink>
        </b>
        {{ $t("post.response_from_after_handle") }}{{ $t("post.response_from_at_date") }}{{ creationTime }}
      </div>
      <PostActions :oekaki="props.oekaki" />
    </div>
    <PostViewOekakiImageContainer :oekaki="props.oekaki" v-on:click="redirectToParent"
      style="max-height: 400px; cursor: pointer;" />
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

.line-bar-element:before {
  content: "";
  z-index: 1;
  position: absolute;
  height: 150%;
  width: 10px;
  border-left: 2px solid #FFB6C1;
  border-bottom: 2px solid #FFB6C1;
  display: block;
  left: -22px;
  top: -100%;
}

.line-bar-element:nth-of-type(2):before {
  content: "";
  z-index: 1;
  position: absolute;
  height: 90%;
  width: 10px;
  border-left: 2px solid #FFB6C1;
  border-bottom: 2px solid #FFB6C1;
  display: block;
  left: -22px;
  top: -35%;
}

.oekaki-image-container img {
  max-width: 100%;
  max-height: 100%;
}

.oekaki-child-info {
  border-bottom: 2px dashed #FFB6C1;
  padding: 10px;
  display: flex;
  align-items: center;
}

.oekaki-child-info>* {
  margin-right: 5px;
}

.oekaki-child-info,
.oekaki-child-info * {
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
