<script setup lang="ts">

import { useIdentityStore } from '@/state/store'
import type { Oekaki } from '@/models/oekaki'

const props = defineProps<{
  oekaki: Oekaki
}>();

const identityStore = useIdentityStore();
</script>

<template>
<div v-if="props.oekaki.author.did == identityStore.did">
  <a href="">[delete]</a>
</div>
</template>

<style scoped>

</style>
