<script setup lang="ts">

import type { TagSearchResult } from '@/models/tag-search-result'
import CardWithImage from '@/components/CardWithImage.vue'

const props = defineProps<{
  tag: TagSearchResult
}>();

</script>

<template>
  <CardWithImage :image-url="props.tag.oekaki.image">
    <div style="font-weight: bold">
      #{{ props.tag.tag }}
    </div>
    <div>
      {{ props.tag.count }} posts under this tag.
    </div>
  </CardWithImage>
</template>

<style scoped>

</style>
