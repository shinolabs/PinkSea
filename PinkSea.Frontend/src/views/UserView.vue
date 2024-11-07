<script setup lang="ts">

import TimeLine from '@/components/TimeLine.vue'
import PanelLayout from '@/layouts/PanelLayout.vue'

</script>

<template>
  <PanelLayout>
    <TimeLine endpoint="com.shinolabs.pinksea.getAuthorFeed" :xrpc-params="{ did: $route.params.did }" />
  </PanelLayout>
</template>

<style scoped>

</style>
