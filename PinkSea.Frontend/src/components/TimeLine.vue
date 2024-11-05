<script setup lang="ts">
import { onBeforeMount, ref } from 'vue'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import type { Oekaki } from '@/models/oekaki'
import type { GenericTimelineQueryOuput, Queries } from '@atcute/client/lexicons'
import { xrpc } from '@/api/atproto/client'

  const props = defineProps<{
    endpoint: keyof Queries
  }>();

  const oekaki = ref<Oekaki[] | null>(null);

  onBeforeMount(async () => {
    const { data } = await xrpc.get(props.endpoint, { });
    oekaki.value = (data as GenericTimelineQueryOuput).oekaki;
  });
</script>

<template>
  <div v-if="oekaki == null">
    <b>loading...</b>
  </div>
  <div class="timeline-container" v-else>
    <TimeLineOekakiCard v-for="oekakiPost of oekaki" v-bind:key="oekakiPost.atProtoLink" v-bind="oekakiPost" :oekaki="oekakiPost" />
  </div>
</template>

<style scoped>
  .timeline-container div {
    margin: 10px;
  }
</style>
