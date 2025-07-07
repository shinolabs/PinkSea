<script setup lang="ts">

import CardWithImage from '@/components/CardWithImage.vue'
import type { Author } from '@/models/author'
import { computed } from 'vue';

const props = defineProps<{
  profile: Author
}>();

const nickname = computed(() => {
  return props.profile.nickname ?? props.profile.handle
})

const avatar = computed(() => {
  return props.profile.avatar ?? "/assets/img/blank_avatar.png"
})
</script>

<template>
  <CardWithImage :image-url="avatar">
    <div style="font-weight: bold">
      {{ nickname }}
    </div>
    <div>
      @{{ props.profile.handle }}
    </div>
  </CardWithImage>
</template>

<style scoped></style>
