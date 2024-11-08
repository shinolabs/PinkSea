<script setup lang="ts">
import { onBeforeMount, ref } from 'vue'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import type { Oekaki } from '@/models/oekaki'
import type { GenericTimelineQueryOutput, Queries } from '@atcute/client/lexicons'
import { xrpc } from '@/api/atproto/client'
import Loader from '@/components/Loader.vue'

  const props = defineProps<{
    endpoint: keyof Queries,
    xrpcParams?: unknown
  }>();

  const oekaki = ref<Oekaki[] | null>(null);

  onBeforeMount(async () => {
    const opts = props.xrpcParams !== undefined
      ? { params: props.xrpcParams }
      : {};

    // @ts-expect-error I do not understand TypeScript but xrpcParams is of the params type of the endpoint.
    const { data } = await xrpc.get(props.endpoint, opts);
    oekaki.value = (data as GenericTimelineQueryOutput).oekaki;
  });
</script>

<template>
  <Loader v-if="oekaki == null" />
  <div class="timeline-container" v-else>
    <TimeLineOekakiCard v-for="oekakiPost of oekaki" v-bind:key="oekakiPost.atProtoLink" v-bind="oekakiPost" :oekaki="oekakiPost" />
  </div>
</template>

<style scoped>
  .timeline-container {
    margin: 10px;
  }
  .timeline-container div {
    margin: 10px;
  }
</style>
