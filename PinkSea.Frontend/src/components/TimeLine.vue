<script setup lang="ts">
import { onBeforeMount, ref } from 'vue'
import TimeLineOekakiCard from '@/components/TimeLineOekakiCard.vue'
import type { Oekaki } from '@/models/oekaki'
import type { TimelineResponse } from '@/models/timeline-response'

  const props = defineProps<{
    endpoint: string
  }>();

  const oekaki = ref<Oekaki[] | null>(null);

  onBeforeMount(async () => {
    await fetch(`http://localhost:5084/xrpc/${props.endpoint}`)
      .then(r => r.json())
      .then(d => {
        console.log(d);
        oekaki.value = (d as TimelineResponse).oekaki;
      });
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
