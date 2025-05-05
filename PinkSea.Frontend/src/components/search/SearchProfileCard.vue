<script setup lang="ts">

import CardWithImage from '@/components/CardWithImage.vue'
import type { Author } from '@/models/author'

const props = defineProps<{
  profile: Author
}>();
</script>

<template>
  <CardWithImage image-url="https://sea.ata.moe/blank_avatar.png">
    <div style="font-weight: bold">
      {{ props.profile.handle }}
    </div>
    <div>
      @{{ props.profile.handle }}
    </div>
  </CardWithImage>
</template>

<style scoped>

</style>
