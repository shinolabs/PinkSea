<script setup lang="ts">
import { onBeforeMount, ref, watch } from 'vue'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import type { Oekaki } from '@/models/oekaki'
import type { GenericTimelineQueryOutput, Queries } from '@atcute/client/lexicons'
import { xrpc } from '@/api/atproto/client'
import Loader from '@/components/Loader.vue'
import Intersector from '@/components/Intersector.vue'

  const props = defineProps<{
    endpoint: keyof Queries,
    xrpcParams?: unknown
  }>();

  const keepLoading = ref<boolean>(true);

  const oekaki = ref<Oekaki[] | null>(null);

  watch(() => props.xrpcParams,(async () => {
    const opts = props.xrpcParams !== undefined
      ? { params: { ...props.xrpcParams } }
      : {};

    // @ts-expect-error I do not understand TypeScript but xrpcParams is of the params type of the endpoint.
    const { data } = await xrpc.get(props.endpoint, opts);
    oekaki.value = (data as GenericTimelineQueryOutput).oekaki;
  }), { immediate: true });

  const loadMore = async () => {
    if (oekaki.value === null || oekaki.value.length < 1 || !keepLoading.value) {
      return;
    }
    const since = oekaki.value[oekaki.value.length - 1].creationTime;

    const opts = props.xrpcParams !== undefined
      ? { params: { since: since, ...props.xrpcParams } }
      : { params: { since: since } };

    const { data } = await xrpc.get(props.endpoint, opts);

    const resp = (data as GenericTimelineQueryOutput).oekaki;
    if (resp.length < 1)
    {
      keepLoading.value = false;
      return;
    }

    oekaki.value = oekaki.value.concat(resp);
  };
</script>

<template>
  <Loader v-if="oekaki == null" />
  <div class="timeline-container" v-else>
    <TimeLineOekakiCard v-for="oekakiPost of oekaki" v-bind:key="oekakiPost.atProtoLink" v-bind="oekakiPost" :oekaki="oekakiPost" />
    <Intersector @intersected="loadMore" />
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
