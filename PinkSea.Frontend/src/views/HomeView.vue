<script setup lang="ts">

import TimeLine from '@/components/TimeLine.vue'
import PanelLayout from '@/layouts/PanelLayout.vue'

</script>

<template>
  <PanelLayout>
    <TimeLine endpoint="com.shinolabs.pinksea.getRecent"/>
  </PanelLayout>
</template>

<style scoped>

</style>
