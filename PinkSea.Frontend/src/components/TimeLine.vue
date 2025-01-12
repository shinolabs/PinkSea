<script setup lang="ts">
import { onBeforeMount, ref, watch } from 'vue'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import type { Oekaki } from '@/models/oekaki'
import type { GenericTimelineQueryOutput, Queries } from '@atcute/client/lexicons'
import { xrpc } from '@/api/atproto/client'
import Loader from '@/components/Loader.vue'
import Intersector from '@/components/Intersector.vue'
import PostViewOekakiChildCard from '@/components/oekaki/PostViewOekakiChildCard.vue'

  const props = defineProps<{
    endpoint: keyof Queries,
    xrpcParams?: unknown,
    showAsReplies?: boolean
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
  <div class="timeline-container" v-else-if="oekaki.length > 0">
    <span v-for="oekakiPost of oekaki" v-bind:key="oekakiPost.at" v-bind="oekakiPost">
      <TimeLineOekakiCard v-if="!props.showAsReplies" :oekaki="oekakiPost"/>
      <PostViewOekakiChildCard v-else :oekaki="oekakiPost" :hide-line-bar="true" />
    </span>
    <Intersector @intersected="loadMore" />
  </div>
  <div class="timeline-container timeline-centered" v-else>
    {{ $t("timeline.nothing_here") }}
  </div>
</template>

<style scoped>
  .timeline-container {
    margin: 10px;
  }
  .timeline-container div {
    margin: 10px;
  }
  .timeline-centered {
    text-align: center;
  }
</style>
